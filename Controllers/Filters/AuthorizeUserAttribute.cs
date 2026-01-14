using Microsoft.AspNetCore.Mvc.Filters;

namespace GymTime.Controllers.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        //Designed to solve repetitive code inside GymController for creating authentication checks.
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.RedirectToActionResult("Login", "Account", null);
            }
            base.OnActionExecuting(context);
        }


    }
}
