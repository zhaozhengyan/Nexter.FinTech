using FinTech.API.Wechat.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FinTech.API.Wechat.Controllers
{
    public static class Extensions
    {
        public static Session GetSession(this ControllerBase me)
        {
            return me.HttpContext.Items["Session"] as Session;
        }
    }
}