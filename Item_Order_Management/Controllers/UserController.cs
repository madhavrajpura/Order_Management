using BusinessLogicLayer.Helper;
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
    public readonly ICartService _cartService;

    public UserController(IItemsService itemService, IJWTService jwtService, IUserService userService, IWishListService wishListService, ICartService cartService)
    {
        _itemService = itemService;
        _jwtService = jwtService;
        _userService = userService;
        _wishListService = wishListService;
        _cartService = cartService;
    }

    // public IActionResult Dashboard()
    // {
    //     if (Request.Cookies.ContainsKey("JWTToken"))
    //     {
    //         string token = Request.Cookies["JWTToken"];
    //         var claims = _jwtService.GetClaimsFromToken(token);
    //         if (claims != null)
    //         {
    //             return View();
    //         }
    //         TempData["ErrorMessage"] = "Token expired. Please login again.";
    //         return RedirectToAction("Login", "Authentication");
    //     }
    //     return RedirectToAction("Login", "Authentication");
    // }

    // [HttpGet]
    // public IActionResult GetItemList(int pageNumber = 1, string search = "", int pageSize = 10, string sortColumn = "ID", string sortDirection = "asc")
    // {
    //     var itemList = _itemService.GetItemList(pageNumber, search, pageSize, sortColumn, sortDirection);
    //     return PartialView("_ItemList", itemList);
    // }

    // [HttpGet]
    // public async Task<IActionResult> GetUserNavbarData()
    // {
    //     string token = Request.Cookies["JWTToken"];
    //     var userId = _userService.GetUserIdFromToken(token);
    //     return PartialView("_UserNavbar", userId);
    // }

    // [HttpPost]

    // public async Task<IActionResult> ToggleWishlistItem(int itemId)
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     var result = await _wishListService.ToggleWishlistItem(userId, itemId);
    //     return Json(new { success = true, isFavourite = result });
    // }

    // [HttpGet]
    // public async Task<IActionResult> Wishlist()
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     var wishlistItems = await _wishListService.GetUserWishlist(userId);
    //     return View(wishlistItems);
    // }

    // [HttpGet]
    // public async Task<IActionResult> IsItemInWishlist(int itemId)
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     var isFavourite = await _wishListService.IsItemInWishlist(userId, itemId);

    //     return Json(new { isFavourite });

    // }

    // [HttpPost]
    // public async Task<IActionResult> AddToCart(int itemId)
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     var item = _itemService.GetItemById(itemId);
    //     if (item == null) return Json(new { success = false, message = "Item not found" });

    //     var isAdded = await _cartService.AddToCart(userId, itemId);
    //     if (!isAdded)
    //     {
    //         return Json(new { success = false, message = "Item already in cart" });
    //     }
    //     return Json(new { success = true });
    // }

    // [HttpGet]
    // public async Task<IActionResult> Cart()
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     var cartItems = await _cartService.GetCartItems(userId);
    //     return View(cartItems);
    // }

    // [HttpGet]
    // public async Task<IActionResult> GetCartItems()
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     var cartItems = await _cartService.GetCartItems(userId);
    //     return PartialView("_CartItems", cartItems);
    // }

    // [HttpPost]
    // public async Task<IActionResult> RemoveFromCart(int cartId)
    // {
    //     var userId = _userService.GetUserIdFromToken(Request.Cookies["JWTToken"]);
    //     await _cartService.RemoveFromCart(cartId, userId);
    //     var cartItems = await _cartService.GetCartItems(userId);
    //     return PartialView("_CartItems", cartItems);
    // }

    private int? GetAuthenticatedUserId()
    {
        if (!Request.Cookies.ContainsKey("JWTToken")) return null;
        string token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _jwtService.GetClaimsFromToken(token);
        return claims != null ? _userService.GetUserIdFromToken(token) : null;
    }

    public IActionResult Dashboard()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        return View();
    }

    [HttpGet]
    public IActionResult GetItemList(int pageNumber = 1, string search = "", int pageSize = 10, string sortColumn = "ID", string sortDirection = "asc")
    {
        PaginationViewModel<ItemViewModel>? itemList = _itemService.GetItemList(pageNumber, search, pageSize, sortColumn, sortDirection);
        return PartialView("_ItemList", itemList);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserNavbarData()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        return PartialView("_UserNavbar", userId);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleWishlistItem(int itemId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        bool result = await _wishListService.ToggleWishlistItem(userId.Value, itemId);
        return Json(new { success = true, isFavourite = result });
    }

    [HttpGet]
    public async Task<IActionResult> Wishlist()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        List<WishListViewModel>? wishlistItems = await _wishListService.GetUserWishlist(userId.Value);
        return View(wishlistItems);
    }

    [HttpGet]
    public async Task<IActionResult> IsItemInWishlist(int itemId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        bool isFavourite = await _wishListService.IsItemInWishlist(userId.Value, itemId);
        return Json(new { isFavourite });
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int itemId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        ItemViewModel? item = _itemService.GetItemById(itemId);

        if (item == null) return Json(new { success = false, message = "Item not found" });

        bool isAdded = await _cartService.AddToCart(userId.Value, itemId);
        if (isAdded)
        {
            return Json(new { success = true, message = "Item added into cart" });
        }
        return Json(new { success = false, message = "Item is already present into cart" });
    }

    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(userId.Value);
        return View(cartItems);
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItems()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        List<CartViewModel>? cartItems = await _cartService.GetCartItems(userId.Value);
        return PartialView("_CartItems", cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int cartId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        await _cartService.RemoveFromCart(cartId, userId.Value);
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(userId.Value);
        return PartialView("_CartItems", cartItems);
    }

    [HttpGet]
    public async Task<IActionResult> ItemDetails(int itemId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        var item = _itemService.GetItemById(itemId);

        if (item == null) return RedirectToAction("Dashboard");

        return View(item);
    }

    /*
[HttpPost]
    public async Task<IActionResult> PlaceOrderFromCart()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Json(new { success = false, message = "Authentication required" });
        }

        var cartItems = await _cartService.GetCartItems(userId.Value);
        if (!cartItems.Any())
        {
            return Json(new { success = false, message = "Cart is empty" });
        }

        int orderId = await _orderService.PlaceOrder(userId.Value, cartItems);
        await _cartService.ClearCart(userId.Value); // Assume ClearCart method exists
        return Json(new { success = true, message = "Order placed successfully", orderId });
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrderFromItem(int itemId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Json(new { success = false, message = "Authentication required" });
        }

        var item = _itemService.GetItemById(itemId);
        if (item == null)
        {
            return Json(new { success = false, message = "Item not found" });
        }

        var cartItems = new List<CartViewModel>
        {
            new CartViewModel { ItemId = item.ItemId, ItemName = item.ItemName, Price = item.Price, ThumbnailImageUrl = item.ThumbnailImageUrl, Quantity = 1 }
        };

        int orderId = await _orderService.PlaceOrder(userId.Value, cartItems);
        return Json(new { success = true, message = "Order placed successfully", orderId });
    }

    [HttpGet]
    public async Task<IActionResult> MyOrders()
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }
        var orders = await _orderService.GetUserOrders(userId.Value);
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> EditOrder(int orderId, decimal newTotalAmount)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Json(new { success = false, message = "Authentication required" });
        }

        var order = (await _orderService.GetUserOrders(userId.Value)).FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return Json(new { success = false, message = "Order not found" });
        }

        if (order.IsDelivered)
        {
            return Json(new { success = false, message = "Cannot edit a delivered order" });
        }

        var orderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
        if ((DateTime.UtcNow - orderDate).TotalDays > 2)
        {
            return Json(new { success = false, message = "Cannot edit order after 2 days" });
        }

        var success = await _orderService.UpdateOrder(orderId, newTotalAmount);
        return Json(new { success, message = success ? "Order updated" : "Failed to update order" });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        int? userId = GetAuthenticatedUserId();
        if (userId == null)
        {
            return Json(new { success = false, message = "Authentication required" });
        }

        var order = (await _orderService.GetUserOrders(userId.Value)).FirstOrDefault(o => o.Id == orderId);
        if (order == null)
        {
            return Json(new { success = false, message = "Order not found" });
        }

        if (order.IsDelivered)
        {
            return Json(new { success = false, message = "Cannot delete a delivered order" });
        }

        var orderDate = DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc);
        if ((DateTime.UtcNow - orderDate).TotalDays > 2)
        {
            return Json(new { success = false, message = "Cannot delete order after 2 days" });
        }

        var success = await _orderService.DeleteOrder(orderId);
        return Json(new { success, message = success ? "Order deleted" : "Failed to delete order" });
    }
    */

}