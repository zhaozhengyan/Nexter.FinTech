using Newtonsoft.Json;
namespace FinTech.ApplicationServices.Dto
{
    public class SendMessageRequest
    {
        [JsonProperty(PropertyName = "touser")]
        public string ToUser { get; set; }

        [JsonProperty(PropertyName = "template_id")]
        public string TemplateId { get; set; }
        [JsonProperty(PropertyName = "page")]
        public string Page { get; set; }
        [JsonProperty(PropertyName = "form_id")]
        public string FormId { get; set; }
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }
    public class Data
    {
        [JsonProperty(PropertyName = "first")]
        public Keyword First { get; set; }
        [JsonProperty(PropertyName = "keyword1")]
        public Keyword Keyword1 { get; set; }
        [JsonProperty(PropertyName = "keyword2")]
        public Keyword Keyword2 { get; set; }
        [JsonProperty(PropertyName = "keyword3")]
        public Keyword Keyword3 { get; set; }
        [JsonProperty(PropertyName = "remark")]
        public Keyword Remark { get; set; }
    }

    public class Keyword
    {
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        [JsonProperty(PropertyName = "color")]
        public string Color => "#173177";

    }
   
}