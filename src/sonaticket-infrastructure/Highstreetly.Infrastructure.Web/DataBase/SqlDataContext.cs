using System;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Infrastructure.DataBase
{
    public class SqlDataContext<T> : IDataContext<T> where T : class, IAggregateRoot
    {
        private readonly IBusControl eventBus;
        private readonly DbContext context;


        public SqlDataContext(Func<DbContext> contextFactory, IBusControl eventBus)
        {
            this.eventBus = eventBus;
            context = contextFactory.Invoke();
        }

        public T Find(Guid id)
        {
            return context.Set<T>().Find(id);
        }

        public void Save(T aggregateRoot)
        {
            var entry = context.Entry(aggregateRoot);

            if (entry.State == EntityState.Detached)
                context.Set<T>().Add(aggregateRoot);


            // Can't have transactions across storage and message bus.
            context.SaveChanges();

            var eventPublisher = aggregateRoot as IEventPublisher;
            if (eventPublisher != null)
            {
                foreach (var e in eventPublisher.Events)
                {
                    eventBus.Publish(e.Value, e.Key);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqlDataContext()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
    }
}