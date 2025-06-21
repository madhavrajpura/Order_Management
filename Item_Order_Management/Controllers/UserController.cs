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
    public readonly IWishListService _wishListService;

    public UserController(IItemsService itemService, IJWTService jwtService, IUserService userService,IWishListService wishListService)
    {
        _itemService = itemService;
        _jwtService = jwtService;
        _userService = userService;
        _wishListService = wishListService;
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
        var userId = _userService.GetUserIdFromToken(token);
        return PartialView("_UserNavbar", userId);
    }

    [HttpPost]

    public async Task<IActionResult> ToggleWishlistItem(int itemId)
    {
        var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
        var result = await _wishListService.ToggleWishlistItem(userId, itemId);
        return Json(new { success = true, isFavourite = result });
    }
    
    [HttpGet]
    public async Task<IActionResult> Wishlist()
    {
        var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
        var wishlistItems = await _wishListService.GetUserWishlist(userId);
        return View(wishlistItems);
    }

    [HttpGet]
    public async Task<IActionResult> IsItemInWishlist(int itemId)
    {
        var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
        var isFavourite = await _wishListService.IsItemInWishlist(userId, itemId);
                
        return Json(new { isFavourite });

    }

}