using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Messaging;
using Highstreetly.Infrastructure.Processors;
using MassTransit;

namespace Highstreetly.Reservations.Sagas
{
    public class RegistrationProcessManager : IProcessManager
    {
        public enum ProcessState
        {
            NotStarted = 0,
            AwaitingReservationConfirmation = 1,
            ReservationConfirmationReceived = 2,
            PaymentConfirmationReceived = 3
        }

        private static readonly TimeSpan BufferTimeBeforeReleasingSeatsAfterExpiration = TimeSpan.FromMinutes(0);

        public RegistrationProcessManager()
        {
            Id = NewId.NextGuid();
        }

        public Guid EventInstanceId { get; set; }

        public Guid OrderId { get; set; }

        public Guid ReservationId { get; set; }

        public bool IsStockManaged { get; set; }

        public Guid TicketReservationCommandId { get; private set; }

        // feels awkward and possibly disrupting to store these properties here.
        // Would it be better if instead of using 
        // current state values, we use event sourcing?
        public DateTime? ReservationAutoExpiration { get;  set; }

        public Guid ExpirationCommandId { get; set; }

        public int StateValue { get; private set; }

        [NotMapped]
        public ProcessState State
        {
            get => (ProcessState)StateValue;
            set => StateValue = (int)value;
        }

        [ConcurrencyCheck] [Timestamp] public byte[] TimeStamp { get; private set; }

        [Key]
        public Guid Id { get; }
        public bool Completed { get; private set; }


        [NotMapped]
        public List<KeyValuePair<Type, Envelope<ICommand>>> Commands { get; } = new();

        public void Handle(IOrderPlaced message)
        {
            if (State == ProcessState.NotStarted)
            {
                EventInstanceId = message.EventInstanceId;
                OrderId = message.SourceId;
                ReservationId = message.SourceId;
                ReservationAutoExpiration = message.ReservationAutoExpiration;
                var expirationWindow = ReservationAutoExpiration.GetValueOrDefault().Subtract(DateTime.UtcNow);

                if (expirationWindow > TimeSpan.Zero)
                {
                    State = ProcessState.AwaitingReservationConfirmation;

                    var seatReservationCommand = new MakeTicketReservation
                    {
                        EventInstanceId = EventInstanceId,
                        ReservationId = ReservationId,
                        Tickets = message.Tickets.ToList(),
                        CorrelationId = message.CorrelationId,
                        IsStockManaged = IsStockManaged
                    };

                    TicketReservationCommandId = seatReservationCommand.Id;

                    AddCommand(typeof(IMakeTicketReservation), new Envelope<ICommand>(seatReservationCommand)
                    {
                        TimeToLive = expirationWindow.Add(TimeSpan.FromMinutes(1)),
                        CorrelationId = message.CorrelationId,
                    });

                    var expirationCommand = new ExpireRegistrationProcess
                    {
                        Id = NewId.NextGuid(),
                        ProcessId = Id,
                        Delay = expirationWindow.Add(BufferTimeBeforeReleasingSeatsAfterExpiration),
                        CorrelationId = message.CorrelationId
                    };

                    ExpirationCommandId = expirationCommand.Id;
                    AddCommand(typeof(IExpireRegistrationProcess), new Envelope<ICommand>(expirationCommand));
                }
                else
                {
                    AddCommand(typeof(IRejectOrder), new RejectOrder { OrderId = OrderId, CorrelationId = message.CorrelationId });
                    Completed = true;
                }
            }
            else
            {
                if (message.EventInstanceId != EventInstanceId)
                {
                    // throw only if not reprocessing
                    throw new InvalidOperationException();
                }
            }
        }

        public void Handle(ICommitOrder message)
        {
            // todo: delete this
        }
        
        public void Handle(IOrderUpdated message)
        {
            if (State == ProcessState.AwaitingReservationConfirmation
                || State == ProcessState.ReservationConfirmationReceived)
            {
                State = ProcessState.AwaitingReservationConfirmation;

                var seatReservationCommand =
                    new MakeTicketReservation
                    {
                        EventInstanceId = EventInstanceId,
                        ReservationId = ReservationId,
                        Tickets = message.Tickets.ToList(),
                        CorrelationId = message.CorrelationId
                    };
                TicketReservationCommandId = seatReservationCommand.Id;
                AddCommand(typeof(IMakeTicketReservation), seatReservationCommand);
            }
            else
            {
                throw new InvalidOperationException("The order cannot be updated at this stage.");
            }
        }

        public void Handle(ITicketsReserved @event)
        {
            if (State == ProcessState.AwaitingReservationConfirmation)
            {
                if (@event.CorrelationId != Guid.Empty)
                {
                    if (string.CompareOrdinal(TicketReservationCommandId.ToString(), @event.CorrelationId.ToString()) != 0)
                    {
                        // skip this event
                        Trace.TraceWarning("Seat reservation response for reservation id {0} does not match the expected correlation id.", @event.ReservationId);
                        return;
                    }
                }

                State = ProcessState.ReservationConfirmationReceived;

                AddCommand(typeof(IMarkTicketsAsReserved), new MarkTicketsAsReserved
                {
                    OrderId = OrderId,
                    Tickets = @event.ReservationDetails.ToList(),
                    Expiration = ReservationAutoExpiration ?? default,
                    CorrelationId = @event.CorrelationId
                });
            }
            else if (string.CompareOrdinal(TicketReservationCommandId.ToString(), @event.CorrelationId.ToString()) == 0)
            {
                Trace.TraceInformation("Seat reservation response for request {1} for reservation id {0} was already handled. Skipping event.", @event.ReservationId, @event.CorrelationId);
            }
            else
            {
                throw new InvalidOperationException("Cannot handle seat reservation at this stage.");
            }
        }

        public void Handle(IPaymentCompleted @event)
        {
            if (State == ProcessState.ReservationConfirmationReceived)
            {
                State = ProcessState.PaymentConfirmationReceived;
                AddCommand(typeof(IConfirmOrder), new ConfirmOrder { OrderId = OrderId, Email = @event.Email, CorrelationId = @event.CorrelationId });
            }
            else
            {
                throw new InvalidOperationException("Cannot handle payment confirmation at this stage.");
            }
        }

        public void Handle(IOrderConfirmed @event)
        {
            if (State == ProcessState.ReservationConfirmationReceived ||
                State == ProcessState.PaymentConfirmationReceived)
            {
                ExpirationCommandId = Guid.Empty;
                Completed = true;

                AddCommand(typeof(ICommitTicketReservation), new CommitTicketReservation
                {
                    ReservationId = ReservationId,
                    EventInstanceId = EventInstanceId,
                    CorrelationId = @event.CorrelationId
                });
            }
            else
            {
                throw new InvalidOperationException("Cannot handle order confirmation at this stage.");
            }
        }

        public void Handle(IExpireRegistrationProcess command)
        {
            if (ExpirationCommandId == command.Id)
            {
                Completed = true;

                AddCommand(typeof(IRejectOrder), new RejectOrder { OrderId = OrderId, CorrelationId = command.CorrelationId });
                AddCommand(typeof(ICancelTicketReservation), new CancelTicketReservation
                {
                    EventInstanceId = EventInstanceId,
                    ReservationId = ReservationId,
                    CorrelationId = command.CorrelationId,
                });

                // TODO cancel payment if any
            }

            // else ignore the message as it is no longer relevant (but not invalid)
        }

        // TODO: Commands needs to be a dictionary [type, command]
        private void AddCommand<T>(Type type, T command)
            where T : ICommand
        {
            Commands.Add(new KeyValuePair<Type, Envelope<ICommand>>(type, Envelope.Create<ICommand>(command)));
        }

        // TODO: Commands needs to be a dictionary [type, command]
        private void AddCommand(Type type, Envelope<ICommand> envelope)
        {
            Commands.Add(new KeyValuePair<Type, Envelope<ICommand>>(type, envelope));
        }
    }
}