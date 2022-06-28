using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Serialization;
using Marten.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Infrastructure.Processors
{
    /// <summary>
    /// Data context used to persist instances of <see cref="IProcessManager"/> (also known as Sagas in the CQRS community) using Entity Framework.
    /// </summary>
    /// <typeparam name="T">The entity type to persist.</typeparam>
    /// <remarks>
    /// <para>See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258564">Reference 6</see> for a description of what is a Process Manager.</para>
    /// <para>This is a very basic implementation, and would benefit from several optimizations. 
    /// For example, it would be very valuable to provide asynchronous APIs to avoid blocking I/O calls.
    /// It would also benefit from dispatching commands asynchronously (but in a resilient way), similar to what the EventStoreBusPublisher does.
    /// See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more potential performance and scalability optimizations.</para>
    /// <para>There are a few things that we learnt along the way regarding Process Managers, which we might do differently with the new insights that we
    /// now have. See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258558"> Journey lessons learnt</see> for more information.</para>
    /// </remarks>
    public class SqlProcessManagerDataContext<T> : IProcessManagerDataContext<T> where T : class, IProcessManager
    {
        readonly IBusClient _busClient;
        private readonly DbContext _context;
        private readonly ITextSerializer _serializer;

        public SqlProcessManagerDataContext(Func<DbContext> contextFactory,
            IBusClient busClient,
            ITextSerializer serializer)
        {
            _context = contextFactory.Invoke();
            _serializer = serializer;
            _busClient = busClient;
        }

        public T Find(Guid id)
        {
            return Find(pm => pm.Id == id, true);
        }

        public T Find(Expression<Func<T, bool>> predicate, bool includeCompleted = false)
        {
            T pm = null;
            if (!includeCompleted)
            {
                // first try to get the non-completed, in case the table is indexed by Completed, or there is more
                // than one process manager that fulfills the predicate but only 1 is not completed.
                pm = _context.Set<T>().Where(predicate.And(x => x.Completed == false)).FirstOrDefault();
            }

            if (pm == null)
            {
                pm = _context.Set<T>().Where(predicate).FirstOrDefault();
            }

            if (pm != null)
            {
                // TODO: ideally this could be improved to avoid 2 roundtrips to the server.
                var undispatchedMessages = _context.Set<UndispatchedMessages>().Find(pm.Id);
                try
                {
                    DispatchMessages(undispatchedMessages);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // if another thread already dispatched the messages, ignore
                    //Trace.TraceWarning("Concurrency exception while marking commands as dispatched for process manager with ID {0} in Find method.", pm.Id);

                    _context.Entry(undispatchedMessages).Reload();

                    undispatchedMessages = _context.Set<UndispatchedMessages>().Find(pm.Id);

                    // undispatchedMessages should be null, as we do not have a rowguid to do optimistic locking, other than when the row is deleted.
                    // Nevertheless, we try dispatching just in case the DB schema is changed to provide optimistic locking.
                    DispatchMessages(undispatchedMessages);
                }

                if (!pm.Completed || includeCompleted)
                {
                    return pm;
                }
            }

            return null;
        }

        /// <summary>
        /// Saves the state of the process manager and publishes the commands in a resilient way.
        /// </summary>
        /// <param name="processManager">The instance to save.</param>
        /// <remarks>For explanation of the implementation details, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see>.</remarks>
        public void Save(T processManager)
        {
            var entry = _context.Entry(processManager);

            if (entry.State == EntityState.Detached)
                _context.Set<T>().Add(processManager);

            var commands = processManager.Commands;
            UndispatchedMessages undispatched = null;
            if (commands.Count > 0)
            {
                // if there are pending commands to send, we store them as unDispatched.
                undispatched = new UndispatchedMessages(processManager.Id)
                {
                    Commands = _serializer.Serialize(commands)
                };
                _context.Set<UndispatchedMessages>().Add(undispatched);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new ConcurrencyException(typeof(T), e);
            }

            try
            {
                DispatchMessages(undispatched, commands);
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.ToString());
                // if another thread already dispatched the messages, ignore
                // Trace.TraceWarning("Ignoring concurrency exception while marking commands as dispatched for process manager with ID {0} in Save method.", processManager.Id);
            }
        }

        /// <summary>
        /// TODO: Commands needs to be a dictionary [type, command]
        /// 
        /// </summary>
        /// <param name="unDispatched">Un-dispatched.</param>
        /// <param name="deserializedCommands">Deserialized commands.</param>
        private void DispatchMessages(UndispatchedMessages unDispatched, List<KeyValuePair<Type, Envelope<ICommand>>> deserializedCommands = null)
        {
            if (unDispatched != null)
            {
                if (deserializedCommands == null)
                {
                    deserializedCommands = _serializer.Deserialize<List<KeyValuePair<Type, Envelope<ICommand>>>>(unDispatched.Commands);
                }

                var originalCommandsCount = deserializedCommands.Count;
                try
                {
                    while (deserializedCommands.Count > 0)
                    {
                        var cmd = deserializedCommands.First();

                        var clientType = _busClient.GetType();
                        var mi = clientType.GetMethod("Send");
                        var fooRef = mi.MakeGenericMethod(cmd.Key);
                        fooRef.Invoke(_busClient, new object[]{ cmd.Value.Body });

                        deserializedCommands.Remove(cmd);
                    }
                }
                catch (Exception)
                {
                    // We catch a generic exception as we don't know what implementation of ICommandBus we might be using.
                    if (originalCommandsCount != deserializedCommands.Count)
                    {
                        // if we were able to send some commands, then updates the unDispatched messages.
                        unDispatched.Commands = _serializer.Serialize(deserializedCommands);
                        try
                        {
                            _context.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            // if another thread already dispatched the messages, ignore and surface original exception instead
                        }
                    }

                    throw;
                }

                // we remove all the unDispatched messages for this process manager.
                _context.Set<UndispatchedMessages>().Remove(unDispatched);
                _context.SaveChanges();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqlProcessManagerDataContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}