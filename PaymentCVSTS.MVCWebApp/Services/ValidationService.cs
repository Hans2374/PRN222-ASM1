using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace PaymentCVSTS.MVCWebApp.Services
{
    public interface IValidationService
    {
        List<string> GetModelStateErrors(ModelStateDictionary modelState);
        void AddError(ModelStateDictionary modelState, string key, string errorMessage);
        List<string> GetErrorsFromException(Exception ex);
    }

    public class ValidationService : IValidationService
    {
        public List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            return modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        public void AddError(ModelStateDictionary modelState, string key, string errorMessage)
        {
            modelState.AddModelError(key, errorMessage);
        }

        public List<string> GetErrorsFromException(Exception ex)
        {
            var errors = new List<string>();

            // Add the main exception message
            errors.Add(ex.Message);

            // If it's a DbUpdateException, try to get more specific information
            if (ex is Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null)
                {
                    errors.Add(dbEx.InnerException.Message);
                }
            }

            // Add inner exception messages if they exist
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                errors.Add(innerEx.Message);
                innerEx = innerEx.InnerException;
            }

            return errors;
        }
    }
}