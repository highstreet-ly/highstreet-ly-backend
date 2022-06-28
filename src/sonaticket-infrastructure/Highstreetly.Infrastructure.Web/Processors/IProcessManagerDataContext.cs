using System;
using System.Linq.Expressions;

namespace Highstreetly.Infrastructure.Processors
{
    public interface IProcessManagerDataContext<T> : IDisposable
        where T : class, IProcessManager
    {
        T Find(Guid id);

        void Save(T processManager);

        // TODO: queryability to reload processes from correlation ids, etc. 
        // Is this appropriate? How do others reload processes? (MassTransit 
        // uses this kind of queryable approach, apparently).
        T Find(Expression<Func<T, bool>> predicate, bool includeCompleted = false);
    }
}