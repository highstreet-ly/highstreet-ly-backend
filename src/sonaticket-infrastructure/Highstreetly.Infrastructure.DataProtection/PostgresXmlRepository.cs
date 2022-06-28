using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Highstreetly.Infrastructure.DataProtection
{
    /// <summary>
    /// An XML repository backed by a Postgres database.
    /// </summary>
    internal class PostgresXmlRepository : IXmlRepository
    {
        private readonly Func<DataContext> _dbContextFactory;
        private readonly Guid _appIdentifier;
        private readonly Guid _instanceIdentifier;

        public PostgresXmlRepository(Func<DataContext> dbContextFactory, Guid appIdentifier, Guid instanceIdentifier)
        {
            if (appIdentifier == Guid.Empty) throw new ArgumentException(nameof(dbContextFactory));
            if (appIdentifier == Guid.Empty) throw new ArgumentException(nameof(dbContextFactory));

            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _appIdentifier = appIdentifier;
            _instanceIdentifier = instanceIdentifier;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return GetAllElementsCore().ToList().AsReadOnly();
        }

        private IEnumerable<XElement> GetAllElementsCore()
        {
            var dbContext = _dbContextFactory();
            var keys = dbContext.KeyCollections.AsNoTracking().Where(x => x.AppId == _appIdentifier).Select(x => x.Value);

            foreach (var value in keys)
            {
                yield return XElement.Parse(value);
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            var keyCollection = new KeyValuesCollection
            {
                AppId = _appIdentifier,
                Value = element.ToString(SaveOptions.DisableFormatting),
                Timestamp = DateTime.Now,
                InstanceId = _instanceIdentifier
            };

            var dbContext = _dbContextFactory();

            dbContext.KeyCollections.Add(keyCollection);
            dbContext.SaveChanges();
        }
    }
}
