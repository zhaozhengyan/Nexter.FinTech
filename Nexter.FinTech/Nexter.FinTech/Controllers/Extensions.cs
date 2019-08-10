using Microsoft.AspNetCore.Mvc;

namespace Nexter.FinTech.Controllers
{
    public static class Extensions
    {
        public static Session GetSession(this ControllerBase me)
        {
            return me.HttpContext.Items["Session"] as Session;
        }
    }
}