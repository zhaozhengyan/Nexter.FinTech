using FinTech.ApplicationServices.Dto;
using Refit;
using System.Threading.Tasks;

namespace FinTech.ApplicationServices
{
    public interface IWeChatApi
    {
        [Get("/cgi-bin/token")]
        Task<Authenticate> GetAccessToken([Query]GetAccessTokenRequest user);

        [Headers("content-type: application/json")]
        [Post("/cgi-bin/message/wxopen/template/uniform_send?access_token={token}")]
        Task<SendMessageResponse> SendMessage(string token, [Body]SendMessageRequest message);
    }


}