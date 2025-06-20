using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Microsoft.AspNetCore.Mvc;

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
                    message += error.Type + ": " + error.Message;
                }

                return controller.Problem(message);
            }

            return controller.Ok(result.GetValue());
        }
    }
}
