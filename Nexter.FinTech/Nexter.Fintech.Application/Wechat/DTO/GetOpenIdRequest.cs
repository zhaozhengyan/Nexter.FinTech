using WebApiClientCore;

namespace FinTech.ApplicationServices.Dto
{
    public class GetOpenIdRequest
    {
        public GetOpenIdRequest() { }
        public GetOpenIdRequest(string appid, string secret, string jsCode)
        {
            this.Appid = appid;
            this.Secret = secret;
            Js_Code = jsCode;
        }

        public string Appid { get; set; }

        public string Secret { get; set; }

        public string Js_Code { get; set; }

        public string Grant_Type => "authorization_code";
    }
}