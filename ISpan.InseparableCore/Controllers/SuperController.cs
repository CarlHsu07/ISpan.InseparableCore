using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using prjMvcCoreDemo.Models;

namespace ISpan.InseparableCore.Controllers
{
    public class SuperController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var actionName = context.ActionDescriptor.RouteValues["action"];

            // todo 指定的action不進行權限控管
            if (actionName == "Register" || actionName == "GetAreas" || actionName == "Paydone") // 指定的action不進行權限控管
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
        }
    }
}
