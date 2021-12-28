using Newtonsoft.Json;

namespace ShopApi.Data.Models.Stripe
{
    public class ConfigResponse
    {
        [JsonProperty("publicKey")]
        public string PublicKey { get; set; }

        [JsonProperty("unitAmount")]
        public long? UnitAmount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}