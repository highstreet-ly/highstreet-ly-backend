using System.Diagnostics;
using Highstreetly.Reservations.Resources;

namespace Highstreetly.Reservations.ReadModel
{
    public abstract class DraftOrderReadModelHandlerBase<T>
    {
        protected static bool WasNotAlreadyHandled(DraftOrder draftOrder, int eventVersion)
        {
            // This assumes that events will be handled in order, but we might get the same message more than once.
            if (eventVersion > draftOrder.OrderVersion)
            {
                return true;
            }
            else if (eventVersion == draftOrder.OrderVersion)
            {
                Trace.TraceWarning(
                    "Ignoring duplicate draft order update message with version {1} for order id {0}",
                    draftOrder.OrderId,
                    eventVersion);
                return false;
            }
            else
            {
                Trace.TraceWarning(
                    @"An older order update message was received with with version {1} for order id {0}, last known version {2}.
This read model generator has an expectation that the EventBus will deliver messages for the same source in order.",
                    draftOrder.OrderId,
                    eventVersion,
                    draftOrder.OrderVersion);
                return false;
            }
        }
    }
}