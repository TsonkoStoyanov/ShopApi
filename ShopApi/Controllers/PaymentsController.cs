namespace ShopApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using ShopApi.Data.Dtos;
    using ShopApi.Data.Models.Stripe;
    using Stripe;
    using Stripe.Checkout;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    [Route("api")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        public readonly StripeOptions _options;
        private readonly IStripeClient client;

        public PaymentsController(IOptions<StripeOptions> options)
        {
            _options = options.Value;
            client = new StripeClient(_options.SecretKey);
        }

        [HttpPost(nameof(CreateCheckoutSession))]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutDto chekoutDto)
        {

            var options = CreateSessionOptions(chekoutDto.Email, chekoutDto.LineItems);

            var service = new SessionService(client);

            try
            {
                var session = await service.CreateAsync(options);
                return Ok(new SessionResponse {SessionId = session.Id});
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private SessionCreateOptions CreateSessionOptions(string userEmail, List<SessionLineItemOptions> lineItems)
            => new()
            {
                CustomerEmail = userEmail,
                LineItems = lineItems,
                PaymentMethodTypes = _options.PaymentMethodTypes,
                Mode = "payment",
                Locale = "auto",
                SuccessUrl = $"{_options.Domain}/{_options.SucessUrl}",
                CancelUrl = $"{_options.Domain}/{_options.CancelUrl}",
            };
    }
}