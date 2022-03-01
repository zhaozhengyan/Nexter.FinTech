using FinTech.ApplicationServices.Dto;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace FinTech.ApplicationServices.WeChat
{
    /// <summary>
    /// Wechat接口
    /// </summary>
    [LoggingFilter]
    public interface IWechatApi : IHttpApi
    {

        [JsonReturn]
        [HttpGet("/sns/jscode2session")]
        Task<GetOpenIdResponse> GetOpenId(GetOpenIdRequest request);

        [JsonReturn]
        [HttpGet("/cgi-bin/token")]
        Task<Authenticate> GetAccessToken(GetAccessTokenRequest request);

        [HttpPost("/cgi-bin/message/wxopen/template/send?access_token={token}")]
        Task<SendMessageResponse> SendMessage(string token, SendMessageRequest message);
    }

}