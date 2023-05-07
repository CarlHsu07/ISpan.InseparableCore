using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ISpan.InseparableCore.Models;
using System.Text.Json;

namespace ISpan.InseparableCore.Controllers
{
	public class SuperController : Controller
	{
		protected TMembers _user = new TMembers();
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			base.OnActionExecuting(context);

			var actionName = context.ActionDescriptor.RouteValues["action"];
			var controllerName = context.ActionDescriptor.RouteValues["controller"];

			if (actionName == "Register" || actionName == "GetAreas" ||actionName== "Paydone") // 不進行權限控管
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
				var serializedTMembers = HttpContext.Session.GetString(CDictionary.SK_LOGINED_USER);
				_user = JsonSerializer.Deserialize<TMembers>(serializedTMembers);
			}

		}
	}
}
