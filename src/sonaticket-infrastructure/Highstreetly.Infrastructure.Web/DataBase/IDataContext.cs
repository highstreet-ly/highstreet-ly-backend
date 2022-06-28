using System;

namespace Highstreetly.Infrastructure.DataBase
{
    /// <summary>
    /// Represents a temporary data context to load and save an entity that implements <see cref="IAggregateRoot"/>.
    /// </summary>
    /// <typeparam name="T">The entity type that will be retrieved or persisted.</typeparam>
    public interface IDataContext<T> : IDisposable
        where T : IAggregateRoot
    {
        T Find(Guid id);

        void Save(T aggregate);
    }
}
