using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;

namespace App.Web.UI.Controllers
{
    [Route("error")]
    [ApiController]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult Index([FromQuery] HttpStatusCode statusCode, string? message = null)
        {
            ViewBag.StatusCode = (int)statusCode;
            ViewBag.Message = HtmlEncoder.Default.Encode(message ?? GetDefaultMessageForStatusCode(statusCode));
            return View();
        }

        private string? GetDefaultMessageForStatusCode(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => "The page you are looking for could not be found.",
                HttpStatusCode.Forbidden => "You do not have permission to access this resource.",
                HttpStatusCode.InternalServerError => "An unexpected error occurred on the server.",
                _ => "An unexpected error occurred."
            };
        }
    }
}
