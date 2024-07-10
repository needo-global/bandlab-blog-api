using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Posts.Api.Models;

namespace Posts.Api.Middleware.Validation;

public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = EmitValidationResult(context);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    private BadRequestObjectResult EmitValidationResult(ActionExecutingContext context)
    {
        var errors = (from modelState in context.ModelState.Values from error in modelState.Errors select new ErrorDto("E001", error.ErrorMessage)).ToList();
        return new BadRequestObjectResult(new ErrorsDto(errors));
    }
}