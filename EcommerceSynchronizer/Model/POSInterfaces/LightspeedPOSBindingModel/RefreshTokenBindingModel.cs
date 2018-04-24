using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EcommerceSynchronizer.Model.POSInterfaces.LightspeedPOSBindingModel
{
    public class RefreshTokenBindingModel
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("client_id")]
        public string ClientID { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
    }
}