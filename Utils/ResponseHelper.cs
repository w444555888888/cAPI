using Microsoft.AspNetCore.Mvc;

namespace BookingApi.Utils
{
    public static class ResponseHelper
    {
        public static IActionResult SendResponse<T>(ControllerBase controller, int status, T data = default, string message = "")
        {
            var response = new
            {
                success = true,
                data = data,
                message = message
            };

            return controller.StatusCode(status, response);
        }
    }
}
