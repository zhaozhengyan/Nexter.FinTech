using Newtonsoft.Json;
namespace FinTech.ApplicationServices.Dto
{
    public class SendMessageRequest
    {
        [JsonProperty(PropertyName = "touser")]
        public string ToUser { get; set; }

        [JsonProperty(PropertyName = "mp_template_msg")]
        public Template Template { get; set; }
    }
    public class Template
    {
        [JsonProperty(PropertyName = "appid")]
        public string Appid { get; set; }
        [JsonProperty(PropertyName = "template_id")]
        public string TemplateId { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "miniprogram")]
        public MiniProgram MiniProgram { get; set; }

        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "keyword1")]
        public Keyword Keyword1 { get; set; }
        [JsonProperty(PropertyName = "keyword2")]
        public Keyword Keyword2 { get; set; }
        [JsonProperty(PropertyName = "keyword3")]
        public Keyword Keyword3 { get; set; }
    }

    public class Keyword
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color => "#173177";

    }

    public class MiniProgram
    {
        [JsonProperty(PropertyName = "appid")]
        public string Appid { get; set; }

        [JsonProperty(PropertyName = "pagepath")]
        public string PagePath { get; set; }
    }
}