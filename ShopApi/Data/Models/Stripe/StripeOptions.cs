namespace ShopApi.Data.Models.Stripe
{
    public class StripeOptions
    {
        public string PublishableKey { get; set; }

        public string SecretKey { get; set; }

        public string WebhookSecret { get; set; }

        public string Price { get; set; }

        public List<string> PaymentMethodTypes { get; set; }

        public string Domain { get; set; }

        public string SucessUrl { get; set; }

        public string CancelUrl { get; set; }

        public Dictionary<string, string> PreviewImages { get; set; } = new Dictionary<string, string>();
    }
}