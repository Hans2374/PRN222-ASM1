using Microsoft.AspNetCore.Mvc;
using PaymentCVSTS.MVCWebApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentCVSTS.MVCWebApp.Extensions
{
    public static class ControllerExtensions
    {
        public static IActionResult HandleValidationErrors(this Controller controller, List<string> errors, string viewName, object model = null)
        {
            // Add errors to ViewBag
            controller.ViewBag.ValidationErrors = errors;

            // Return the view with the model
            return controller.View(viewName, model);
        }

        public static IActionResult WithValidationErrors(this IActionResult result, Controller controller, List<string> errors)
        {
            controller.ViewBag.ValidationErrors = errors;
            return result;
        }

        public static async Task<IActionResult> ExecuteWithValidation<T>(
            this Controller controller,
            Func<Task<T>> action,
            Func<T, IActionResult> onSuccess,
            Func<Exception, IActionResult> onError = null,
            string defaultErrorMessage = "An error occurred while processing your request.")
        {
            try
            {
                var result = await action();
                return onSuccess(result);
            }
            catch (Exception ex)
            {
                var validationService = controller.HttpContext.RequestServices.GetService(typeof(IValidationService)) as IValidationService;

                var errors = validationService?.GetErrorsFromException(ex) ?? new List<string> { defaultErrorMessage };

                // If custom error handler was provided, use it
                if (onError != null)
                {
                    return onError(ex);
                }

                // Otherwise, add error to ViewBag and return current view
                controller.ViewBag.ValidationErrors = errors;

                // Return to the same view
                return controller.View(controller.ControllerContext.ActionDescriptor.ActionName);
            }
        }
    }
}