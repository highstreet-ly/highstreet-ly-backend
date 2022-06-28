using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Highstreetly.Management.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Highstreetly.Management.Handlers
{
    public abstract class OrderEventHandlerBase<T>
    {
        protected ManagementDbContext ManagementDbContext;
        protected readonly ILogger<T> Logger;
        private readonly AsyncRetryPolicy _waitForOrder;

        protected OrderEventHandlerBase(ILogger<T> logger, ManagementDbContext managementDbContext)
        {
            Logger = logger;
            ManagementDbContext = managementDbContext;

            _waitForOrder = Policy
                .Handle<InvalidOperationException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                });
        }

        protected async Task<bool> ProcessOrder(Expression<Func<Order, bool>> lookup, Func<Order, Task> orderAction)
        {
            try
            {
                var order = await _waitForOrder.ExecuteAsync(
                    async () => await ManagementDbContext.Orders.FirstAsync(lookup));


                if (order != null)
                {
                    await orderAction.Invoke(order);
                    await ManagementDbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Couldn't run ProcessOrder", ex);
                throw;
            }

            return false;
        }
    }
}