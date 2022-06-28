using System;

namespace Highstreetly.Payments.Models.Stripe.Charge
{
    public class Charge 
    {
        public string Id { get; set; }
      
        public Guid PaymentId { get; set; }

        public string Object { get; set; }

        public int Amount { get; set; }

        public int AmountCaptured { get; set; }

        public int AmountRefunded { get; set; }

        public object Application { get; set; }

        public string ApplicationFee { get; set; }

        public int ApplicationFeeAmount { get; set; }

        public string BalanceTransaction { get; set; }

        public BillingDetails BillingDetails { get; set; }

        public string CalculatedStatementDescriptor { get; set; }

        public bool Captured { get; set; }

        public int Created { get; set; }

        public string Currency { get; set; }

        public object Customer { get; set; }

        public object Description { get; set; }

        public string Destination { get; set; }

        public object Dispute { get; set; }

        public bool Disputed { get; set; }

        public object FailureCode { get; set; }

        public object FailureMessage { get; set; }

        public FraudDetails FraudDetails { get; set; }

        public object Invoice { get; set; }

        public bool Livemode { get; set; }

        public Metadata Metadata { get; set; }

        public object OnBehalfOf { get; set; }

        public object Order { get; set; }

        public Outcome Outcome { get; set; }

        public bool Paid { get; set; }

        public string PaymentIntentId { get; set; }
        public string PaymentIntent { get; set; }

        public string PaymentMethod { get; set; }

        public PaymentMethodDetails PaymentMethodDetails { get; set; }

        public object ReceiptEmail { get; set; }

        public object ReceiptNumber { get; set; }

        public string ReceiptUrl { get; set; }

        public bool Refunded { get; set; }

        public Refunds Refunds { get; set; }

        public object Review { get; set; }

        public object Shipping { get; set; }

        public object Source { get; set; }

        public object SourceTransfer { get; set; }

        public object StatementDescriptor { get; set; }

        public object StatementDescriptorSuffix { get; set; }

        public string Status { get; set; }

        public string Transfer { get; set; }

        public TransferData TransferData { get; set; }

        public string TransferGroup { get; set; }
    }
}