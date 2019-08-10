using System;
using System.Linq;
using System.Threading.Tasks;
using FinTech.Domain;
using FinTech.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexter.Domain;

namespace Nexter.FinTech.Infrastructure
{
    public class SessionFilter : IAsyncAuthorizationFilter
    {
        protected IRepository Store { get; }
        protected virtual ILogger<SessionFilter> Logger { get; }
        public SessionFilter(ILogger<SessionFilter> logger, IRepository store)
        {
            Logger = logger;
            Store = store;
        }
        protected virtual Task NotAuthorized(AuthorizationFilterContext context, Result result = null)
        {
            context.Result = new ObjectResult(result ?? Result.Fail(nameof(StatusCode.Unauthorized)));

            return Task.CompletedTask;
        }
        async Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                if (context.ActionDescriptor is ControllerActionDescriptor action)
                {
                    if (action.MethodInfo.DeclaringType.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any()) return;

                    if (action.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any()) return;
                }

                var sessionId = context.HttpContext.Request.Query["Token"];
                if (string.IsNullOrWhiteSpace(sessionId))
                    sessionId = context.HttpContext.Request.Headers["Token"];
                var session = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.AccountCode == sessionId);

                context.HttpContext.Items["Session"] = session;
            }
            catch (BusinessViolation violation)
            {
                await NotAuthorized(context, Result.Fail(nameof(StatusCode.Unauthorized), violation.Message));

                return;
            }
            catch (Exception error)
            {
                Logger?.LogError(0, error, error.Message);
                await NotAuthorized(context, Result.Fail(nameof(StatusCode.InternalServerError)));
                return;
            }
        }
    }
}
