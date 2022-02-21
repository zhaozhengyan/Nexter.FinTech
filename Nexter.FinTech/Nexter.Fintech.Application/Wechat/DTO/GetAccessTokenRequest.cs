using WebApiClientCore;

namespace FinTech.ApplicationServices.Dto
{
    public class GetAccessTokenRequest
    {
        //[AliasAs("appid")]
        public string Appid { get; set; }

        //[AliasAs("secret")]
        public string Secret { get; set; }

        //[AliasAs("grant_type")]
        public string GrantType => "client_credential";
    }
}