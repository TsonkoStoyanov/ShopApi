using Stripe.Checkout;

namespace ShopApi.Data.Dtos
{
    public class CheckoutDto
    {
        public string Email { get; set; }

        public List<SessionLineItemOptions> LineItems { get; set; }
    }
}
