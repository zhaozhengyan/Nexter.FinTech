using FinTech.Domain;
using Furion.DatabaseAccessor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nexter.Fintech.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinTech.API.Wechat.Infrastructure
{
    public class SessionFilter : IAsyncAuthorizationFilter
    {
        private readonly IRepository<Member> _memberRep;
        protected virtual ILogger<SessionFilter> Logger { get; }
        public SessionFilter(ILogger<SessionFilter> logger, IRepository<Member> memberRep)
        {
            Logger = logger;
            _memberRep = memberRep;
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
                if (string.IsNullOrWhiteSpace(sessionId))
                    throw new BusinessViolation(BusinessViolationStatusCodes.RuleViolated);
                var session = await _memberRep.AsQueryable()
                     .FirstOrDefaultAsync(e => e.AccountCode == sessionId.FirstOrDefault());
                context.HttpContext.Items["Session"] = new Session(session);
            }
            catch (BusinessViolation violation)
            {
                await NotAuthorized(context, Result.Fail(nameof(StatusCode.Unauthorized), violation.Message));
                return;
            }
            catch (Exception error)
            {
                Logger?.LogError(0, error, error.Message);
                await NotAuthorized(context, Result.Fail(nameof(StatusCode.Unauthorized)));
                return;
            }
        }
    }
}
