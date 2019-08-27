using FinTech.ApplicationServices.Dto;
using Refit;
using System.Threading.Tasks;

namespace FinTech.ApplicationServices
{
    public interface IWeChatApi
    {

        [Get("/sns/jscode2session")]
        Task<GetOpenIdResponse> GetOpenId([Query]GetOpenIdRequest request);

        [Get("/cgi-bin/token")]
        Task<Authenticate> GetAccessToken([Query]GetAccessTokenRequest request);

        [Headers("content-type: application/json")]
        [Post("/cgi-bin/message/wxopen/template/send?access_token={token}")]
        Task<SendMessageResponse> SendMessage(string token, [Body]SendMessageRequest message);
    }

}