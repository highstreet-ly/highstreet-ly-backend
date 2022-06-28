namespace Highstreetly.Infrastructure.Stripe
{
    public class StripeResponse
    {
        public string access_token { get; set; }
        public bool livemode { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string stripe_publishable_key { get; set; }
        public string stripe_user_id { get; set; }
        public string scope { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }

    }
}
