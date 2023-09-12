using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvolvedTax.Helpers
{
    public class UserSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Session == null ||
                             !context.HttpContext.Session.TryGetValue("RecipientEmail", out byte[] val))
            {
                context.Result =
                    new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Account",
                        action = "AccessDenied",
                    }));
            }
            base.OnActionExecuting(context);
        }
    }
}
