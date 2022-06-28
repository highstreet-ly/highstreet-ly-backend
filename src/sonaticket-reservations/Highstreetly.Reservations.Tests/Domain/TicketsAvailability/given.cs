using System;
using System.Linq;
using FluentAssertions;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.MessageDtos;
using NUnit.Framework;

namespace Highstreetly.Reservations.Tests.Domain.TicketsAvailability
{
    public class Given
    {
        [Test]
        public void when_adding_seat_type_then_changes_availability()
        {
            var id = Guid.NewGuid();
            var seatType = Guid.NewGuid();

            var sut = new Reservations.Domain.TicketsAvailability(id, Guid.NewGuid());
            sut.AddTickets(seatType, 50, new OrderTicketDetails());

            sut.SingleEvent<AvailableTicketsChanged>().Tickets.Single().TicketType.Should().Be(seatType);
            sut.SingleEvent<AvailableTicketsChanged>().Tickets.Single().Quantity.Should().Be(50);
        }
    }

    public class GivenNonStockCheckedItems
    {
        private static readonly Guid ConferenceId = Guid.NewGuid();
        private static readonly Guid TicketTypeId = Guid.NewGuid();
        private Reservations.Domain.TicketsAvailability _sut;
        
        [SetUp]
        public void SetUp()
        {
            _sut = new Reservations.Domain.TicketsAvailability(
                ConferenceId, 
                Guid.NewGuid(),
                new[] { new AvailableTicketsChanged { Tickets = new[] { new TicketQuantity(TicketTypeId, 0, new OrderTicketDetails()) } } });
        }
        
        [Test]
        public void when_reserving_items_then_reserves_items()
        {
            _sut.MakeReservation(Guid.NewGuid(), new[] { new TicketQuantity(TicketTypeId, 10, new OrderTicketDetails()) }, false);

            _sut.Events.Select(x => x.Value).OfType<TicketsReserved>().Single().ReservationDetails.ElementAt(0).Quantity.Should().Be(10);
        }
    }
    

    public class GivenAvailableItems
    {
        private static readonly Guid ConferenceId = Guid.NewGuid();
        private static readonly Guid TicketTypeId = Guid.NewGuid();

        private Reservations.Domain.TicketsAvailability _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new Reservations.Domain.TicketsAvailability(
                ConferenceId, 
                Guid.NewGuid(), 
                new[] { new AvailableTicketsChanged { Tickets = new[] { new TicketQuantity(TicketTypeId, 10, new OrderTicketDetails()) } } });
        }

        [Test]
        public void when_adding_non_existing_seat_type_then_adds_availability()
        {
            var seatType = Guid.NewGuid();
            _sut.AddTickets(seatType, 50, new OrderTicketDetails());

            _sut.SingleEvent<AvailableTicketsChanged>().Tickets.Single().TicketType.Should().Be(seatType);
            _sut.SingleEvent<AvailableTicketsChanged>().Tickets.Single().Quantity.Should().Be(50);
        }

        [Test]
        public void when_adding_items_to_existing_seat_type_then_adds_remaining_items()
        {
            _sut.AddTickets(TicketTypeId, 10, new OrderTicketDetails());

            _sut.SingleEvent<AvailableTicketsChanged>().Tickets.Single().TicketType.Should().Be(TicketTypeId);
            _sut.SingleEvent<AvailableTicketsChanged>().Tickets.Single().Quantity.Should().Be(10);
        }

        [Test]
        public void when_removing_items_to_existing_seat_type_then_removes_remaining_items()
        {
            _sut.RemoveTickets(TicketTypeId, 5);

            _sut.MakeReservation(Guid.NewGuid(), new[] { new TicketQuantity(TicketTypeId, 10, new OrderTicketDetails()) }, true);

            _sut.Events.Select(x=>x.Value).OfType<AvailableTicketsChanged>().Last().Tickets.Single().TicketType.Should().Be(TicketTypeId);
            _sut.Events.Select(x => x.Value).OfType<AvailableTicketsChanged>().Last().Tickets.Single().Quantity.Should().Be(-5);
            _sut.Events.Select(x => x.Value).OfType<TicketsReserved>().Single().ReservationDetails.ElementAt(0).Quantity.Should().Be(5);
        }


        [Test]
        public void when_reserving_less_items_than_total_then_reserves_all_requested_items()
        {
            _sut.MakeReservation(Guid.NewGuid(), new[] { new TicketQuantity(TicketTypeId, 4, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).TicketType.Should()
                .Be(TicketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).Quantity.Should().Be(4);
        }

        [Test]
        public void when_reserving_less_items_than_total_then_reduces_remaining_items()
        {
            _sut.MakeReservation(Guid.NewGuid(), new[] { new TicketQuantity(TicketTypeId, 4, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).TicketType.Should()
                .Be(TicketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).Quantity.Should().Be(-4);
        }
        
        [Test]
        public void when_reserving_more_items_than_total_then_reserves_total()
        {
            var id = Guid.NewGuid();
            _sut.MakeReservation(id, new[] { new TicketQuantity(TicketTypeId, 11, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).TicketType.Should()
                .Be(TicketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).Quantity.Should().Be(10);
        }
        
        [Test]
        public void when_reserving_more_items_than_total_then_reduces_remaining_items()
        {
            var id = Guid.NewGuid();
            _sut.MakeReservation(id, new[] { new TicketQuantity(TicketTypeId, 11, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).TicketType.Should()
                .Be(TicketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).Quantity.Should().Be(-10);
        }
        
        [Test]
        public void when_reserving_non_existing_seat_type_then_throws()
        {
            var id = Guid.NewGuid();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _sut.MakeReservation(id, new[]
                                         {
                                             new TicketQuantity(TicketTypeId, 11,new OrderTicketDetails()),
                                             new TicketQuantity(Guid.NewGuid(), 3,new OrderTicketDetails()),
                                         }, true));
        }
    }

    public class GivenSomeAvialableItemsAndSomeTaken
    {
        private Reservations.Domain.TicketsAvailability _sut;
        private readonly Guid _eventInstanceId = Guid.NewGuid();
        private readonly Guid _ticketTypeId = Guid.NewGuid();
        private readonly Guid _otherTicketTypeId = Guid.NewGuid();
        private readonly Guid _reservationId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _sut = new Reservations.Domain.TicketsAvailability(_eventInstanceId, Guid.NewGuid(),
                new ISonaticketEvent[]
                    {
                        new AvailableTicketsChanged()
                            {
                                Tickets = new[] { new TicketQuantity(_ticketTypeId, 10, new OrderTicketDetails()) , new TicketQuantity(_otherTicketTypeId, 12, new OrderTicketDetails()) }
                            },
                        new TicketsReserved()
                        {
                            ReservationId = _reservationId,
                            ReservationDetails = new[] { new TicketQuantity(_ticketTypeId, 6, new OrderTicketDetails() ) },
                            AvailableTicketsChanged = new[] { new TicketQuantity(_ticketTypeId, -6, new OrderTicketDetails()) }
                        }
                    });
        }

        [Test]
        public void when_reserving_less_items_than_remaining_then_items_are_reserved()
        {
            _sut.MakeReservation(Guid.NewGuid(), new[] { new TicketQuantity(_ticketTypeId, 4, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).Quantity.Should().Be(4);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).TicketType.Should()
                .Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).Quantity.Should().Be(-4);
        }

        [Test]
        public void when_reserving_more_items_than_remaining_then_reserves_all_remaining()
        {
            var id = Guid.NewGuid();
            _sut.MakeReservation(Guid.NewGuid(), new[] { new TicketQuantity(_ticketTypeId, 5, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.ElementAt(0).Quantity.Should().Be(4);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).TicketType.Should()
                .Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.ElementAt(0).Quantity.Should().Be(-4);
        }

        [Test]
        public void when_cancelling_an_inexistent_reservation_then_no_op()
        {
            _sut.CancelReservation(Guid.NewGuid());

            _sut.Events.Count().Should().Be(1);
        }

        [Test]
        public void when_committing_an_inexistant_reservation_then_no_op()
        {
            _sut.CommitReservation(Guid.NewGuid());

            _sut.Events.Count().Should().Be(1);
        }
    }

    public class GivenAnExistingReservation
    {
        private Reservations.Domain.TicketsAvailability _sut;
        private readonly Guid _eventInstanceId = Guid.NewGuid();
        private readonly Guid _ticketTypeId = Guid.NewGuid();
        private readonly Guid _otherTicketTypeId = Guid.NewGuid();
        private readonly Guid _reservationId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _sut = new Reservations.Domain.TicketsAvailability(
                _eventInstanceId, Guid.NewGuid(),
                new ISonaticketEvent[]
                    {
                        new AvailableTicketsChanged
                            {
                                Tickets = new[]
                                {
                                    new TicketQuantity(_ticketTypeId, 10, new OrderTicketDetails()) , 
                                    new TicketQuantity(_otherTicketTypeId, 12, new OrderTicketDetails())
                                },
                                Version = 1,
                            },
                        new TicketsReserved
                        {
                            ReservationId = _reservationId,
                            ReservationDetails = new[] { new TicketQuantity(_ticketTypeId, 6, new OrderTicketDetails()) },
                            AvailableTicketsChanged = new[] { new TicketQuantity(_ticketTypeId, -6, new OrderTicketDetails()) },
                            Version = 2,
                        }
                    });
        }

        [Test]
        public void when_committing_then_commits_reservation_id()
        {
            _sut.CommitReservation(_reservationId);
            _sut.SingleEvent<TicketReservationCommitted>().ReservationId.Should().Be(_reservationId);
        }

        [Test]
        public void when_cancelling_then_cancels_reservation_id()
        {
            _sut.CancelReservation(_reservationId);

            _sut.SingleEvent<TicketsReservationCancelled>().ReservationId.Should().Be(_reservationId);
        }

        [Test]
        public void when_cancelled_then_items_become_available()
        {
            _sut.CancelReservation(_reservationId);
            _sut.SingleEvent<TicketsReservationCancelled>().AvailableTicketsChanged.Single().TicketType.Should()
                .Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReservationCancelled>().AvailableTicketsChanged.Single().Quantity.Should().Be(6);
        }

        [Test]
        public void when_updating_reservation_with_more_items_then_reserves_all_requested()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_ticketTypeId, 8, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationId.Should().Be(_reservationId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().Quantity.Should().Be(8);
        }

        [Test]
        public void when_updating_reservation_with_more_items_then_changes_available_items()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_ticketTypeId, 8, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single().TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single().Quantity.Should().Be(-2);
        }

        [Test]
        public void when_updating_reservation_with_less_items_then_reserves_all_requested()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_ticketTypeId, 2, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationId.Should().Be(_reservationId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().Quantity.Should().Be(2);
        }

        [Test]
        public void when_updating_reservation_with_less_items_then_changes_available_items()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_ticketTypeId, 2, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single().TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single().Quantity.Should().Be(4);
        }

        [Test]
        public void when_updating_reservation_with_more_items_than_available_then_reserves_as_much_as_possible()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_ticketTypeId, 12, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationId.Should().Be(_reservationId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().Quantity.Should().Be(10);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single().TicketType.Should().Be(_ticketTypeId);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single().Quantity.Should().Be(-4);
        }

        [Test]
        public void when_updating_reservation_with_different_items_then_reserves_them()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_otherTicketTypeId, 3, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().ReservationId.Should().Be(_reservationId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().TicketType.Should().Be(_otherTicketTypeId);
            _sut.SingleEvent<TicketsReserved>().ReservationDetails.Single().Quantity.Should().Be(3);
        }

        [Test]
        public void when_updating_reservation_with_different_items_then_unreserves_the_previous_ones_and_reserves_new_ones()
        {
            _sut.MakeReservation(_reservationId, new[] { new TicketQuantity(_otherTicketTypeId, 3, new OrderTicketDetails()) }, true);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Count().Should().Be(2);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single(x => x.TicketType == _otherTicketTypeId)
                .Quantity.Should().Be(-3);
            _sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single(x => x.TicketType == _ticketTypeId)
                .Quantity.Should().Be(6);
        }

        // [Test]
        // public void when_regenerating_from_memento_then_can_continue()
        // {
        //     var memento = sut.SaveToMemento();
        //     sut = new Reservations.Domain.TicketsAvailability(sut.Id, memento, Enumerable.Empty<ISonaticketEvent>());
        //
        //     Assert.Equal(2, sut.Version);
        //
        //     sut.MakeReservation(ReservationId, new[] { new TicketQuantity(OtherTicketTypeId, 3, new OrderTicketDetails()) });
        //
        //     Assert.Equal(2, sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Count());
        //     Assert.Equal(-3, sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single(x => x.TicketType == OtherTicketTypeId).Quantity);
        //     Assert.Equal(6, sut.SingleEvent<TicketsReserved>().AvailableTicketsChanged.Single(x => x.TicketType == TicketTypeId).Quantity);
        //     Assert.Equal(3, sut.SingleEvent<TicketsReserved>().Version);
        // }
    }
}
