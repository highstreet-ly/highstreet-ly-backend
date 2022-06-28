using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Highstreetly.Infrastructure;
using Highstreetly.Infrastructure.DataBase;
using Highstreetly.Infrastructure.Events;
using Highstreetly.Infrastructure.Extensions;
using Stripe;

namespace Highstreetly.Payments.Domain
{
    /// <summary>
    /// Represents a payment through a 3rd party system.
    /// </summary>
    /// <remarks>
    /// <para>For more information on the Payments BC, see <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258555">Journey chapter 5</see>.</para>
    /// </remarks>
    public class ThirdPartyProcessorPayment : IAggregateRoot, IEventPublisher
    {
        private readonly Dictionary<Type, ISonaticketEvent> _events = new();

        public ThirdPartyProcessorPayment(Guid id,
            Guid paymentSourceId,
            string description,
            long totalAmount,
            string paymentIntentId,
            IEnumerable<ThirdPartyProcessorPaymentItem> items)
            : this()
        {
            Id = id;
            PaymentSourceId = paymentSourceId;
            Description = description;
            TotalAmount = totalAmount;
            PaymentIntentId = paymentIntentId;
            Items.AddRange(items);

            AddEvent<IPaymentInitiated>(new PaymentInitiated{ SourceId = id, PaymentSourceId = paymentSourceId });
        }

        public string Email { get; set; }

        protected ThirdPartyProcessorPayment()
        {
            Items = new ObservableCollection<ThirdPartyProcessorPaymentItem>();
        }

        public int StateValue { get; private set; }

        [NotMapped]
        public PaymentStates State
        {
            get => (PaymentStates)StateValue;
            internal set => StateValue = (int)value;
        }

        public Dictionary<Type, ISonaticketEvent> Events => _events;

        public Guid Id { get; private set; }

        /// <summary>
        /// This will be the order id for most cases
        /// In the future it could be subscription id or whatever
        /// </summary>
        public Guid PaymentSourceId { get; private set; }

        public string Description { get; private set; }

        /// <summary>
        /// This is Stripe specific so it's a hack for now
        /// </summary>
        public string PaymentIntentId { get; set; }

        public long TotalAmount { get; private set; }

        public ICollection<ThirdPartyProcessorPaymentItem> Items { get; private set; }

        public void Complete(string email, Guid userId)
        {
            if (State != PaymentStates.Initiated)
            {
                throw new InvalidOperationException();
            }

            Email = email;
            State = PaymentStates.Completed;
            AddEvent<IPaymentCompleted>(new PaymentCompleted
            {
                SourceId = Id,
                PaymentSourceId = PaymentSourceId,
                Email = Email,
                UserId = userId
            });
        }

        public void Cancel(string reason)
        {
            //if (State != PaymentStates.Initiated)
            //{
            //    throw new InvalidOperationException($"current state = {State}");
            //}

			Description = reason;

            State = PaymentStates.Rejected;
            AddEvent<IPaymentRejected>(new PaymentRejected { SourceId = Id, PaymentSourceId = PaymentSourceId });
        }

        public void IssueRefund(int refundAmount, string chargeId, Guid refundId)
        {
            if (State != PaymentStates.Completed && State != PaymentStates.Refunded)
            {
                throw new InvalidOperationException($"current state = {State} - the payment hasn't been processed - no refund can be issued");
            }
            
            var service = new RefundService();
            
            var options = new RefundCreateOptions
                          {
                              ReverseTransfer = false,
                              Reason = "requested_by_customer",
                              Amount = refundAmount,
                              Charge = chargeId, 
                              RefundApplicationFee = false
                          };

            var refundResponse =   service.Create(options);

            if (refundResponse.Status.ToLower() == "succeeded")
            {
                State = PaymentStates.Refunded;

                AddEvent<IRefundIssued>(new RefundIssued
                                        {
                                            SourceId = refundId,
                                            Amount = refundAmount,
                                        });
            }
            else
            {
                throw new InvalidOperationException($"Failed to issue refund {refundResponse.Reason} - the refund hasn't been processed - no refund can be issued");
            }
        }

        protected void AddEvent<T>(ISonaticketEvent @event)
        {
            _events.Add(@event.GetType().GetInterfaces()[0], @event);
        }
    }
}