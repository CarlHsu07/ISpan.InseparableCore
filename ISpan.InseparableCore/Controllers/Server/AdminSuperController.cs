using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjMvcCoreDemo.Models;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers.Server
{
    public class AdminSuperController : Controller
    {
        protected TAdministrators _user = new TAdministrators();
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var actionName = context.ActionDescriptor.RouteValues["action"];

            if (actionName == "GetAreas" || actionName == "Paydone") // 不進行權限控管
            {
                return;
            }

            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_LOGINED_USER))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    Controller = "Home",
                    Action = "Login"
                }));
            }
            else
            {
                var serializedTAdministrators = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
                _user = JsonSerializer.Deserialize<TAdministrators>(serializedTAdministrators);
            }

        }

    }
}
