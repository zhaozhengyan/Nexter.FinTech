using WebApiClientCore;

namespace FinTech.ApplicationServices.Dto
{
    public class GetOpenIdRequest
    {
        public GetOpenIdRequest() { }
        public GetOpenIdRequest(string appid, string secret, string jsCode)
        {
            this.appid = appid;
            this.secret = secret;
            js_code = jsCode;
        }

        public string appid { get; set; }

        public string secret { get; set; }

        public string js_code { get; set; }

        public string grant_type => "authorization_code";
    }
}