using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Item_Order_Management.Controllers;

public class UserController : Controller
{
    private readonly IItemsService _itemService;
    private readonly IJWTService _jwtService;
    private readonly IUserService _userService;

    public UserController(IItemsService itemService, IJWTService jwtService, IUserService userService)
    {
        _itemService = itemService;
        _jwtService = jwtService;
        _userService = userService;
    }

    public IActionResult Dashboard()
    {
        if (Request.Cookies.ContainsKey("JWTToken"))
        {
            string token = Request.Cookies["JWTToken"];
            var claims = _jwtService.GetClaimsFromToken(token);
            if (claims != null)
            {
                return View();
            }
            TempData["ErrorMessage"] = "Token expired. Please login again.";
            return RedirectToAction("Login", "Authentication");
        }
        return RedirectToAction("Login", "Authentication");
    }

    [HttpGet]
    public IActionResult GetItemList(int pageNumber = 1, string search = "", int pageSize = 10, string sortColumn = "ID", string sortDirection = "asc")
    {
        var itemList = _itemService.GetItemList(pageNumber, search, pageSize, sortColumn, sortDirection);
        return PartialView("_ItemList", itemList);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserNavbarData()
    {
        string token = Request.Cookies["JWTToken"];
        var userId = await _userService.GetUserIdFromToken(token);
        return PartialView("_UserNavbar", userId);
    }
}