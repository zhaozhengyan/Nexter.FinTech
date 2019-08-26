using Newtonsoft.Json;
namespace FinTech.ApplicationServices.Dto
{
    public class Authenticate : BaseResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}