using Newtonsoft.Json;
namespace FinTech.ApplicationServices.Dto
{
    public class GetOpenIdResponse : BaseResponse
    {
        [JsonProperty(PropertyName = "openid")]
        public string OpenId { get; set; }
        [JsonProperty(PropertyName = "session_key")]
        public string SessionKey { get; set; }
        [JsonProperty(PropertyName = "unionid")]
        public string UnionId { get; set; }

    }

    
}