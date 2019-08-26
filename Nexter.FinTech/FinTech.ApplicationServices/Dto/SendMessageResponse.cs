using Newtonsoft.Json;
namespace FinTech.ApplicationServices.Dto
{
    public class SendMessageResponse : BaseResponse
    {
        [JsonProperty(PropertyName = "msgid")]
        public string Msgid { get; set; }

        public bool IsSuccess => Errcode == 0 && Errmsg == "ok";
    }

    public class BaseResponse
    {
        [JsonProperty(PropertyName = "errcode")]
        public int Errcode { get; set; }
        [JsonProperty(PropertyName = "errmsg")]
        public string Errmsg { get; set; }
    }
}