using Newtonsoft.Json;
namespace FinTech.ApplicationServices.Dto
{
    public class GetOpenIdResponse : BaseResponse
    {
        public string OpenId { get; set; }
        public string Session_Key { get; set; }
        public string UnionId { get; set; }
    }
}