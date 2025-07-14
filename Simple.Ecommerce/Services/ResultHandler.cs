using Microsoft.AspNetCore.Mvc;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.Api.Services
{
    public static class ResultHandler
    {
        public static ActionResult HandleResult<TResult>(ControllerBase controller, Result<TResult> result)
        {
            if (result.IsFailure)
            {
                var message = "";

                foreach (var error in result.Errors!)
                {
                    message += error.Type + ": " + error.Message + "\n";
                }

                return controller.Problem(message);
            }

            return controller.Ok(result.GetValue());
        }
    }
}
