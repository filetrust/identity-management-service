using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glasswall.IdentityManagementService.Api.ActionFilters
{
    public class ModelStateValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.ErrorCount > 0)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ModelStateValidationActionFilterAttribute>>();
                logger.LogInformation("Request contained bad data");
            }
        }
    }
}
