using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ISpan.InseparableCore.Models;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminSuperController : Controller
    {
        protected TAdministrators _admin = new TAdministrators();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var actionName = context.ActionDescriptor.RouteValues["action"];

            if (actionName == "GetAreas" || actionName == "Paydone") // 不進行權限控管
            {
                return;
            }

            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_ADMINISTRATOR))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    Controller = "AdminHome",
                    Action = "Login"
                }));
            }
            else
            {
                var serializedTAdministrators = HttpContext.Session.GetString(CDictionary.SK_LOGINED_ADMINISTRATOR);
                _admin = JsonSerializer.Deserialize<TAdministrators>(serializedTAdministrators);
            }

        }

    }
}
