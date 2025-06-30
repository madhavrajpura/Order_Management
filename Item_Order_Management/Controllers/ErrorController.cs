using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Item_Order_Management.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
// to prevent error page caching.
public class ErrorController : Controller
{
    [Route("Error/Unauthorized")]
    public IActionResult Unauthorized()
    {
        return View("Unauthorized");
    }

    [Route("Error/PageNotFound")]
    public IActionResult PageNotFound()
    {
        return View("PageNotFound");
    }

    [Route("Error/Forbidden")]
    public IActionResult Forbidden()
    {
        return View("Forbidden");
    }

    [Route("Error/InternalServerError")]
    public IActionResult InternalServerError()
    {
        return View("InternalServerError");
    }

    [Route("Error/HandleError/{statusCode}")]
    public IActionResult HandleError(int statusCode)
    {
        switch (statusCode)
        {
            case 401:
                return View("Unauthorized");
            case 403:
                return View("Forbidden");
            case 404:
                return View("PageNotFound");
            case 500:
                return View("InternalServerError");
            default:
                return View("GenericError");
        }
    }
}
