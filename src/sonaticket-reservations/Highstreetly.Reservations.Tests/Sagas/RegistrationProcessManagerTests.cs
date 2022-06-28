using System;
using System.Linq;
using FluentAssertions;
using Highstreetly.Infrastructure.Commands;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.MessageDtos;
using Highstreetly.Reservations.Sagas;
using NUnit.Framework;

namespace Highstreetly.Reservations.Tests.Sagas
{
    public class Context
    {
        protected RegistrationProcessManager Sut;

        [SetUp]
        public void Setup()
        {
            Sut = new RegistrationProcessManager();
        }
    }

    public class WhenOrderIsPlaced : Context
    {
        private OrderPlaced _orderPlaced;

        [SetUp]
        public void Setup()
        {
            _orderPlaced = new OrderPlaced
            {
                SourceId = Guid.NewGuid(),
                EventInstanceId = Guid.NewGuid(),
                Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 2, new OrderTicketDetails()) },
                ReservationAutoExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(22))
            };
            Sut.Handle(_orderPlaced);
        }

        [Test]
        public void then_sends_no_commands()
        {
            Sut.Commands.Count().Should().Be(2);
        }

        [Test]
        public void then_reservation_state_is_AwaitingReservationConfirmation()
        {
            Sut.State.Should().Be(RegistrationProcessManager.ProcessState.AwaitingReservationConfirmation);
        }

        [Test]
        public void then_expiration_command_is_fired_with_a_delay()
        {
            var reservation = Sut.Commands.Select(x => x.Value.Body).OfType<IExpireRegistrationProcess>().Single();
            reservation.Delay.Should().BeGreaterThan(TimeSpan.FromMinutes(20));
            reservation.ProcessId.Should().Be(Sut.Id);
            reservation.Id.Should().Be(Sut.ExpirationCommandId);
        }

        [Test]
        public void then_reservation_is_requested_for_specific_operator_service()
        {
            var reservation = Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().Single();
            _orderPlaced.EventInstanceId.Should().Be(reservation.EventInstanceId);
            reservation.Tickets.First().Quantity.Should().Be(2);
        }

        [Test]
        public void then_saves_reservation_command_id_for_later_use()
        {
            var reservation = Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().Single();
            reservation.Id.Should().Be(Sut.TicketReservationCommandId);
        }


        [Test]
        public void then_reservation_expiration_time_is_stored_for_later_use()
        {
            Sut.ReservationAutoExpiration.HasValue.Should().BeTrue();
            _orderPlaced.ReservationAutoExpiration.Should().Be(Sut.ReservationAutoExpiration.Value);
        }

        [Test]
        public void then_transitions_to_awaiting_reservation_confirmation_state()
        {
            RegistrationProcessManager.ProcessState.AwaitingReservationConfirmation.Should().Be(Sut.State);
        }
    }

    public class WhenOrderIsPlacedButAlreadyExpired : Context
    {
        private OrderPlaced _orderPlaced;

        [SetUp]
        public void Setup()
        {
            _orderPlaced = new OrderPlaced
            {
                SourceId = Guid.NewGuid(),
                EventInstanceId = Guid.NewGuid(),
                Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 2, new OrderTicketDetails()) },
                ReservationAutoExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(-1))
            };
            Sut.Handle(_orderPlaced);
        }

        [Test]
        public void then_order_is_rejected()
        {
            var command = Sut.Commands.Select(x => x.Value.Body).Cast<RejectOrder>().Single();
            _orderPlaced.SourceId.Should().Be(command.OrderId);
        }

        [Test]
        public void then_process_manager_is_completed()
        {
            Sut.Completed.Should().BeTrue();
        }
    }

    namespace Highstreetly.Reservations.Tests.Sagas.RegistrationProcessFixture.given_process_awaiting_for_reservation_confirmation
    {
        public class Context
        {
            protected RegistrationProcessManager Sut;
            protected Guid OrderId;
            protected Guid EventInstanceId;

            [SetUp]
            public void Setup()
            {
                Sut = new RegistrationProcessManager();
                OrderId = Guid.NewGuid();
                EventInstanceId = Guid.NewGuid();

                Sut.Handle(
                    new OrderPlaced
                    {
                        SourceId = OrderId,
                        EventInstanceId = EventInstanceId,
                        Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 2, new OrderTicketDetails()) },
                        ReservationAutoExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(22))
                    });
            }
        }

        public class WhenReservationConfirmationIsReceived : Context
        {
            private Guid _reservationId;

            [SetUp]
            public void Setup()
            {
                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();
                _reservationId = makeReservationCommand.ReservationId;

                var itemsReserved = new TicketsReserved
                {
                    SourceId = EventInstanceId,
                    ReservationId = makeReservationCommand.ReservationId,
                    ReservationDetails = Array.Empty<TicketQuantity>(),
                    CorrelationId = makeReservationCommand.Id
                };
                Sut.Handle(itemsReserved);
            }

            [Test]
            public void then_updates_order_status()
            {
                var command = Sut.Commands.Select(x => x.Value.Body).OfType<MarkTicketsAsReserved>().Single();

                OrderId.Should().Be(command.OrderId);
            }

            [Test]
            public void then_transitions_state()
            {
                RegistrationProcessManager.ProcessState.ReservationConfirmationReceived.Should().Be(Sut.State);
            }
        }

        public class WhenReservationConfirmationIsReceivedForNonCurrentCorrelationId : Context
        {
            private int _initialCommandCount;

            [SetUp]
            public void Setup()
            {
                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();

                var itemsReserved = new TicketsReserved
                {
                    SourceId = EventInstanceId,
                    ReservationId = makeReservationCommand.ReservationId,
                    ReservationDetails = Array.Empty<TicketQuantity>(),
                    CorrelationId = Guid.NewGuid()
                };
                _initialCommandCount = Sut.Commands.Count();
                Sut.Handle(itemsReserved);
            }

            [Test]
            public void then_does_not_update_order_status()
            {
                _initialCommandCount.Should().Be(Sut.Commands.Count());
            }

            [Test]
            public void then_does_not_transition_state()
            {
                RegistrationProcessManager.ProcessState.AwaitingReservationConfirmation.Should().Be(Sut.State);
            }
        }

        public class WhenOrderUpdateIsReceived : Context
        {
            private Guid _reservationId;
            private OrderUpdated _orderUpdated;

            [SetUp]
            public void Setup()
            {
                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();
                _reservationId = makeReservationCommand.ReservationId;

                _orderUpdated = new OrderUpdated
                {
                    SourceId = Guid.NewGuid(),
                    Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 3, new OrderTicketDetails()) },
                };
                Sut.Handle(_orderUpdated);
            }

            [Test]
            public void then_sends_new_reservation_command()
            {
                Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().Count().Should().Be(2);
            }

            [Test]
            public void then_reservation_is_requested_for_specific_operator_service()
            {
                var newReservation = Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().ElementAt(1);
                EventInstanceId.Should().Be(newReservation.EventInstanceId);

                newReservation.Tickets[0].Quantity.Should().Be(3);
            }

            [Test]
            public void then_saves_reservation_command_id_for_later_use()
            {
                var reservation = Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().ElementAt(1);
                reservation.Id.Should().Be(Sut.TicketReservationCommandId);
            }

            [Test]
            public void then_transitions_to_awaiting_reservation_confirmation_state()
            {
                RegistrationProcessManager.ProcessState.AwaitingReservationConfirmation.Should().Be(Sut.State);
            }
        }
    }

    namespace Highstreetly.Reservations.Tests.Sagas.RegistrationProcessFixture.given_process_with_reservation_confirmation_received
    {
        public class Context
        {
            protected RegistrationProcessManager Sut;
            protected Guid OrderId;
            protected Guid EventInstanceId;
            protected Guid ReservationId;

            [SetUp]
            public void Setup()
            {
                Sut = new RegistrationProcessManager();
                OrderId = Guid.NewGuid();
                EventInstanceId = Guid.NewGuid();

                var seatType = Guid.NewGuid();

                Sut.Handle(
                    new OrderPlaced
                    {
                        SourceId = OrderId,
                        EventInstanceId = EventInstanceId,
                        Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 2, new OrderTicketDetails()) },
                        ReservationAutoExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(22))
                    });

                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();
                ReservationId = makeReservationCommand.ReservationId;

                Sut.Handle(
                    new TicketsReserved
                    {
                        SourceId = EventInstanceId,
                        ReservationId = makeReservationCommand.ReservationId,
                        ReservationDetails = new[] { new TicketQuantity(seatType, 2, new OrderTicketDetails()) },
                        CorrelationId = makeReservationCommand.Id
                    });
            }
        }

        public class WhenReservationIsExpired : Context
        {
            [SetUp]
            public void Setup()
            {
                var expirationCommand = Sut.Commands.Select(x => x.Value.Body).OfType<ExpireRegistrationProcess>().Single();
                Sut.Handle(expirationCommand);
            }

            [Test]
            public void then_cancels_seat_reservation()
            {
                var command = Sut.Commands.Select(x => x.Value.Body).OfType<CancelTicketReservation>().Single();
                ReservationId.Should().Be(command.ReservationId);
                EventInstanceId.Should().Be(command.EventInstanceId);
            }

            [Test]
            public void then_updates_order_status()
            {
                var command = Sut.Commands.Select(x => x.Value.Body).OfType<RejectOrder>().Single();
                command.OrderId.Should().Be(OrderId);
            }

            [Test]
            public void then_transitions_state()
            {
                Sut.Completed.Should().BeTrue();
            }
        }

        public class WhenOrderUpdateIsReceived : Context
        {
            private OrderUpdated _orderUpdated;

            [SetUp]
            public void Setup()
            {
                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();
                ReservationId = makeReservationCommand.ReservationId;

                _orderUpdated = new OrderUpdated
                {
                    SourceId = OrderId,
                    Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 3, new OrderTicketDetails()) }
                };
                Sut.Handle(_orderUpdated);
            }

            [Test]
            public void then_sends_new_reservation_command()
            {
                Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().Count().Should().Be(2);
            }

            [Test]
            public void then_reservation_is_requested_for_specific_conference()
            {
                var newReservation = Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().ElementAt(1);
                EventInstanceId.Should().Be(newReservation.EventInstanceId);
                newReservation.Tickets[0].Quantity.Should().Be(3);
            }

            [Test]
            public void then_saves_reservation_command_id_for_later_use()
            {
                var reservation = Sut.Commands.Select(x => x.Value.Body).OfType<MakeTicketReservation>().ElementAt(1);
                reservation.Id.Should().Be(Sut.TicketReservationCommandId);
            }

            [Test]
            public void then_transitions_to_awaiting_reservation_confirmation_state()
            {
                RegistrationProcessManager.ProcessState.AwaitingReservationConfirmation.Should().Be(Sut.State);
            }
        }

        public class WhenPaymentConfirmationIsReceived : Context
        {
            [SetUp]
            public void Setup()
            {
                Sut.Handle(new PaymentCompleted
                {
                    PaymentSourceId = OrderId,
                });
            }

            [Test]
            public void then_confirms_order()
            {
                var command = Sut.Commands.Select(x => x.Value.Body).OfType<ConfirmOrder>().Single();
                OrderId.Should().Be(command.OrderId);
            }

            [Test]
            public void then_transitions_state()
            {
                RegistrationProcessManager.ProcessState.PaymentConfirmationReceived.Should().Be(Sut.State);
            }
        }

        public class WhenOrderIsConfirmed : Context
        {
            [SetUp]
            public void Setup()
            {
                Sut.Handle(new OrderConfirmed
                {
                    SourceId = OrderId,
                });
            }

            [Test]
            public void then_commits_seat_reservations()
            {
                var command = Sut.Commands.Select(x => x.Value.Body).OfType<CommitTicketReservation>().Single();
                ReservationId.Should().Be(command.ReservationId);
                EventInstanceId.Should().Be(command.EventInstanceId);
            }

            [Test]
            public void then_transitions_state()
            {
                Assert.True(Sut.Completed);
            }
        }

        public class WhenReservationConfirmationIsReceivedForCurrentCorrelationId : Context
        {
            private int _initialCommandCount;

            [SetUp]
            public void Setup()
            {
                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();

                var itemsReserved = new TicketsReserved
                {
                    SourceId = EventInstanceId,
                    ReservationId = makeReservationCommand.ReservationId,
                    ReservationDetails = new TicketQuantity[0],
                    CorrelationId = makeReservationCommand.Id
                };
                _initialCommandCount = Sut.Commands.Count();
                Sut.Handle(itemsReserved);
            }

            [Test]
            public void then_does_not_send_new_update_to_order()
            {
                _initialCommandCount.Should().Be(Sut.Commands.Count());
            }

            [Test]
            public void then_does_not_transition_state()
            {
                RegistrationProcessManager.ProcessState.ReservationConfirmationReceived.Should().Be(Sut.State);
            }
        }

        public class WhenReservationConfirmationIsReceivedForNonCurrentCorrelationId : Context
        {
            private Exception _exception;

            [SetUp]
            public void Setup()
            {
                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();

                var itemsReserved = new TicketsReserved
                {
                    SourceId = EventInstanceId,
                    ReservationId = makeReservationCommand.ReservationId,
                    ReservationDetails = new TicketQuantity[0],
                    CorrelationId = Guid.NewGuid()
                };

                try
                {
                    Sut.Handle(itemsReserved);
                }
                catch (InvalidOperationException e)
                {
                    _exception = e;
                }
            }

            [Test]
            public void then_throws()
            {
                Assert.NotNull(_exception);
            }
        }
    }


    namespace Highstreetly.Reservations.Tests.Sagas.RegistrationProcessFixture.given_process_with_payment_confirmation_received
    {
        public class Context
        {
            protected RegistrationProcessManager Sut;
            protected Guid OrderId;
            protected Guid ConferenceId;
            protected Guid ReservationId;

            public Context()
            {
                this.Sut = new RegistrationProcessManager();
                this.OrderId = Guid.NewGuid();
                this.ConferenceId = Guid.NewGuid();

                var seatType = Guid.NewGuid();

                this.Sut.Handle(
                    new OrderPlaced
                    {
                        SourceId = this.OrderId,
                        EventInstanceId = this.ConferenceId,
                        Tickets = new[] { new TicketQuantity(Guid.NewGuid(), 2, new OrderTicketDetails()) },
                        ReservationAutoExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(22))
                    });

                var makeReservationCommand = Sut.Commands.Select(e => e.Value.Body).OfType<MakeTicketReservation>().Single();
                this.ReservationId = makeReservationCommand.ReservationId;

                this.Sut.Handle(
                    new TicketsReserved()
                        {
                            SourceId = this.ConferenceId,
                            ReservationId = makeReservationCommand.ReservationId,
                            ReservationDetails = new[] { new TicketQuantity(seatType, 2, new OrderTicketDetails()) },
                            CorrelationId = makeReservationCommand.Id
                        });

                this.Sut.Handle(
                    new PaymentCompleted
                    {
                        PaymentSourceId = this.OrderId,
                    });
            }
        }
        
        public class WhenOrderIsConfirmed : Context
        {
            public WhenOrderIsConfirmed()
            {
                Sut.Handle(new OrderConfirmed
                {
                    SourceId = this.OrderId,
                });
            }
        
            [Test]
            public void then_commits_seat_reservations()
            {
                var command = Sut.Commands.Select(x => x.Value.Body).OfType<CommitTicketReservation>().Single();
                ReservationId.Should().Be(command.ReservationId);
                ConferenceId.Should().Be(command.EventInstanceId);
            }
        
            [Test]
            public void then_transitions_state()
            {
                Sut.Completed.Should().BeTrue();
            }
        }
    }
}
