using System.Collections.Generic;

namespace Highstreetly.Payments.Models.Stripe.Transfer
{
    public class Reversals
    {
        public string Object { get; set; }

        public List<object> Data { get; set; }

        public bool HasMore { get; set; }

        public int TotalCount { get; set; }

        public string Url { get; set; }
    }
}