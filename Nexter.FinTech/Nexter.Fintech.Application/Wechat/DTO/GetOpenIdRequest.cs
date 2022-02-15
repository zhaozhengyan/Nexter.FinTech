namespace FinTech.ApplicationServices.Dto
{
    public class GetOpenIdRequest
    {
        public GetOpenIdRequest() { }
        public GetOpenIdRequest(string appid, string secret, string jsCode)
        {
            Appid = appid;
            Secret = secret;
            JsCode = jsCode;
        }

        //[AliasAs("appid")]
        public string Appid { get; set; }

        //[AliasAs("secret")]
        public string Secret { get; set; }

        //[AliasAs("js_code")]
        public string JsCode { get; set; }

        //[AliasAs("grant_type")]
        public string GrantType => "authorization_code";
    }
}