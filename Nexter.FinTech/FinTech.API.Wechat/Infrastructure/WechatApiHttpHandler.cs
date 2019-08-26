using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FinTech.API.Wechat.Infrastructure
{
    public class WechatApiHttpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var requestStr = await request.Content.ReadAsStringAsync();
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}