using WebApiClientCore;

namespace FinTech.ApplicationServices.Dto
{
    public class GetAccessTokenRequest
    {
        //[AliasAs("appid")]
        public string Appid { get; set; }

        //[AliasAs("secret")]
        public string Secret { get; set; }

        public string grant_type => "client_credential";
    }
}