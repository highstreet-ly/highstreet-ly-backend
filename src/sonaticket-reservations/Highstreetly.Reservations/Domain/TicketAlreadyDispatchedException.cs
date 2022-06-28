using System;

namespace Highstreetly.Reservations.Domain
{
    public class TicketAlreadyDispatchedException : Exception
    {
        private Guid _id;
        private int _position;

        public TicketAlreadyDispatchedException(Guid id, int position)
        {
            _id = id;
            _position = position;
        }
    }
}