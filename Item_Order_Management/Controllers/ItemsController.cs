using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Item_Order_Management.Controllers;

public class ItemsController : Controller
{
    private readonly IItemsService _itemService;
    private readonly IJWTService _jwtService;
    private readonly IUserService _userService;
    public readonly IWishListService _wishListService;
    public readonly ICartService _cartService;
    public readonly IOrderService _orderService;

    public ItemsController(IItemsService itemService, IJWTService jwtService, IUserService userService, IWishListService wishListService, ICartService cartService, IOrderService orderService)
    {
        _itemService = itemService;
        _jwtService = jwtService;
        _userService = userService;
        _wishListService = wishListService;
        _cartService = cartService;
        _orderService = orderService;
    }

    #region Dashboard Content

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
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
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
        var UserId = _userService.GetUserIdFromToken(token);
        return PartialView("_UserNavbar", UserId);
    }

    #endregion

    #region WishList

    [HttpGet]
    public async Task<IActionResult> Wishlist()
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        List<WishListViewModel>? wishlistItems = await _wishListService.GetUserWishlist(UserId);
        return View(wishlistItems);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleWishlistItem(int itemId)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        bool result = await _wishListService.ToggleWishlistItem(UserId, itemId);
        return Json(new { success = true, isFavourite = result });
    }

    [HttpGet]
    public async Task<IActionResult> IsItemInWishlist(int itemId)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        bool isFavourite = await _wishListService.IsItemInWishlist(UserId, itemId);
        return Json(new { isFavourite });
    }

    #endregion

    #region Cart

    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(UserId);
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int itemId)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        ItemViewModel? item = _itemService.GetItemById(itemId);

        if (item == null) return Json(new { success = false, message = "Item not found" });

        bool isAdded = await _cartService.AddToCart(UserId, itemId);
        if (isAdded)
        {
            return Json(new { success = true, message = "Item added into cart" });
        }
        return Json(new { success = false, message = "Item is already present into cart" });
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItems()
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        List<CartViewModel>? cartItems = await _cartService.GetCartItems(UserId);
        return PartialView("_CartItems", cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int cartId)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        await _cartService.RemoveFromCart(cartId, UserId);
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(UserId);
        return PartialView("_CartItems", cartItems);
    }

    #endregion

    #region Item Detail

    [HttpGet]
    public async Task<IActionResult> ItemDetails(int itemId)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        var item = _itemService.GetItemById(itemId);

        if (item == null) return RedirectToAction("Dashboard");

        return View(item);
    }

    #endregion

    #region Orders

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        string? token = Request.Cookies["JWTToken"];
        var claims = _jwtService.GetClaimsFromToken(token);
        var userId = _userService.GetUserIdFromToken(token);

        if (claims == null || userId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        var orders = await _orderService.GetUserOrdersAsync(userId);
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(string orderData)
    {
        string? token = Request.Cookies["JWTToken"];
        var claims = _jwtService.GetClaimsFromToken(token);
        var userId = _userService.GetUserIdFromToken(token);


        OrderViewModel orderVM = JsonSerializer.Deserialize<OrderViewModel>(orderData);

        if (claims == null || userId == 0 || string.IsNullOrEmpty(token))
        {
            return Json(new { success = false, message = NotificationMessage.TokenExpired });
        }

        var result = await _orderService.CreateOrderAsync(userId, orderVM);
        if (result)
        {
            var cartItems = await _cartService.GetCartItems(userId);

            foreach (var cartItem in cartItems)
            {
                await _cartService.RemoveFromCart(cartItem.Id, userId);
            }

            return Json(new { success = true, message = "Order placed successfully", redirectUrl = Url.Action("Orders") });
        }

        return Json(new { success = false, message = "Failed to place order" });
    }

    #endregion

    public IActionResult UserProfile()
    {
        string? token = Request.Cookies["JWTToken"];
        string? Email = _jwtService.GetClaimValue(token, "email");

        List<UserViewModel>? data = _userService.GetUserProfileDetails(Email);
        return View(data[0]);
    }

    [HttpPost]
    public IActionResult UserProfile(UserViewModel user)
    {
        string? token = Request.Cookies["JWTToken"];
        string? userEmail = _jwtService.GetClaimValue(token, "email");

        if (user.ImageFile != null)
        {
            string[]? extension = user.ImageFile.FileName.Split(".");
            if (extension[extension.Length - 1] == "jpg" || extension[extension.Length - 1] == "jpeg" || extension[extension.Length - 1] == "png")
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string fileName = ImageTemplate.GetFileName(user.ImageFile, path);
                user.ImageURL = $"/uploads/{fileName}";
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.ImageFormat;
                return RedirectToAction("AddUser", "User", new { Email = user.Email });
            }
        }


        _userService.UpdateUserProfile(user, userEmail);

        CookieOptions options = new CookieOptions();
        options.Expires = DateTime.Now.AddMinutes(60);
        if (user.ImageURL != null)
        {
            Response.Cookies.Append("profileImage", user.ImageURL, options);
        }
        Response.Cookies.Append("username", user.UserName, options);

        TempData["SuccessMessage"] = NotificationMessage.UpdateSuccess.Replace("{0}", "Profile");
        return RedirectToAction("UserProfile");
    }

    #region ChangePassword
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel changepassword)

    {
        string? token = Request.Cookies["JWTToken"];
        string? userEmail = _jwtService.GetClaimValue(token, "email");


        if (changepassword.CurrentPassword == changepassword.NewPassword)
        {
            TempData["ErrorMessage"] = "Current Password and New Password cannot be same";
            return View();
        }
        else
        {
            changepassword.CurrentPassword = Encryption.EncryptPassword(changepassword.CurrentPassword);
            changepassword.NewPassword = Encryption.EncryptPassword(changepassword.NewPassword);
            bool password_verify = _userService.ChangePassword(changepassword, userEmail);

            if (password_verify)
            {
                TempData["SuccessMessage"] = NotificationMessage.UpdateSuccess.Replace("{0}", "Password");
                return RedirectToAction("Dashboard", "Items");
            }
            else
            {
                TempData["ErrorMessage"] = NotificationMessage.UpdateFailure.Replace("{0}", "Password");
                return View();
            }
        }
    }
    #endregion

}