using System.Collections.Generic;

namespace Highstreetly.Payments.Models.Stripe.PaymentIntent
{
    public class PaymentIntent
    {
        public string Id { get; set; }

        public string Object { get; set; }

        public int Amount { get; set; }

        public int AmountCapturable { get; set; }

        public int AmountReceived { get; set; }

        public object Application { get; set; }

        public int ApplicationFeeAmount { get; set; }

        public object CanceledAt { get; set; }

        public object CancellationReason { get; set; }

        public string CaptureMethod { get; set; }

        public Charges Charges { get; set; }

        public string ClientSecret { get; set; }

        public string ConfirmationMethod { get; set; }

        public int Created { get; set; }

        public string Currency { get; set; }

        public object Customer { get; set; }

        public object Description { get; set; }

        public object Invoice { get; set; }

        public object LastPaymentError { get; set; }

        public bool Livemode { get; set; }

        public Metadata Metadata { get; set; }

        public object NextAction { get; set; }

        public object OnBehalfOf { get; set; }

        public object PaymentMethod { get; set; }

        public PaymentMethodOptions PaymentMethodOptions { get; set; }

        public List<string> PaymentMethodTypes { get; set; }

        public object ReceiptEmail { get; set; }

        public object Review { get; set; }

        public object SetupFutureUsage { get; set; }

        public object Shipping { get; set; }

        public object Source { get; set; }

        public object StatementDescriptor { get; set; }

        public object StatementDescriptorSuffix { get; set; }

        public string Status { get; set; }

        public TransferData TransferData { get; set; }

        public object TransferGroup { get; set; }
    }
}