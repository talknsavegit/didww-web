using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

public class TokenValidationFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Session.GetString("Token");

        if (token == null)
        {
            // Set error information in HttpContext.Items
            context.HttpContext.Items["Error"] = "User must log in first!";
            context.Result = new UnauthorizedObjectResult("User must login First!");
        }

        base.OnActionExecuting(context);
    }
}
