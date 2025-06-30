using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Item_Order_Management.Controllers;


[Authorize(Roles = "Admin,User")]
public class ItemsController : Controller
{
    private readonly IItemsService _itemService;
    private readonly IJWTService _jwtService;
    private readonly IUserService _userService;
    public readonly IWishListService _wishListService;
    public readonly ICartService _cartService;
    public readonly IOrderService _orderService;
    public readonly INotificationService _notificationService;

    public ItemsController(IItemsService itemService, IJWTService jwtService, IUserService userService, IWishListService wishListService, ICartService cartService, IOrderService orderService, INotificationService notificationService)
    {
        _itemService = itemService;
        _jwtService = jwtService;
        _userService = userService;
        _wishListService = wishListService;
        _cartService = cartService;
        _orderService = orderService;
        _notificationService = notificationService;
    }

    #region Dashboard Content

    public IActionResult Dashboard()
    {
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
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        NotificationCartViewModel CombinedVM = new NotificationCartViewModel();
        CombinedVM.CartListVM = await _cartService.GetCartItems(UserId);
        CombinedVM.NotificationListVM = _notificationService.GetNotificationsById(UserId);
        return PartialView("_UserNavbar", CombinedVM);
    }

    #endregion

    #region Notification

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int userNotificationId)
    {
        await _notificationService.MarkNotificationAsRead(userNotificationId);
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        NotificationCartViewModel CombinedVM = new NotificationCartViewModel();
        CombinedVM.NotificationListVM = _notificationService.GetNotificationsById(UserId);
        return PartialView("_UserNotificationList", CombinedVM);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _notificationService.MarkAllNotificationsAsRead(UserId);
        NotificationCartViewModel CombinedVM = new NotificationCartViewModel();
        CombinedVM.NotificationListVM = _notificationService.GetNotificationsById(UserId);
        return PartialView("_UserNotificationList", CombinedVM);
    }

    #endregion

    #region WishList

    [HttpGet]
    public async Task<IActionResult> Wishlist()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<WishListViewModel>? wishlistItems = await _wishListService.GetUserWishlist(UserId);
        return View(wishlistItems);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleWishlistItem(int itemId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        bool result = await _wishListService.ToggleWishlistItem(UserId, itemId);
        return Json(new { success = true, isFavourite = result });
    }

    [HttpGet]
    public async Task<IActionResult> IsItemInWishlist(int itemId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        bool isFavourite = await _wishListService.IsItemInWishlist(UserId, itemId);
        return Json(new { isFavourite });
    }

    #endregion

    #region Cart

    [HttpGet]
    public async Task<IActionResult> Cart()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(UserId);
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int itemId, int quantity = 1)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        ItemViewModel? item = _itemService.GetItemById(itemId);
        if (item == null) return Json(new { success = false, message = "Item not found" });

        bool isAdded = await _cartService.AddToCart(UserId, itemId, quantity);
        if (isAdded)
        {
            return Json(new { success = true, message = "Item added to cart" });
        }
        return Json(new { success = false, message = "Item is already present in cart" });
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItems()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(UserId);
        return PartialView("_CartItems", cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFromCart(int cartId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _cartService.RemoveFromCart(cartId, UserId);
        List<CartViewModel>? cartItems = await _cartService.GetCartItems(UserId);
        return PartialView("_CartItems", cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCartQuantity(int cartId, int quantity)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        if (quantity < 1)
        {
            return Json(new { success = false, message = "Quantity must be at least 1" });
        }
        bool result = await _cartService.UpdateCartQuantity(cartId, UserId, quantity);
        if (result)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }

    [HttpPost]
    public async Task<IActionResult> AddAllFromWishlistToCart()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        bool result = await _cartService.AddAllFromWishlistToCart(UserId);
        if (result)
        {
            return Json(new { success = true, message = "All wishlist items added to cart" });
        }
        return Json(new { success = false, message = "Failed to add items to cart" });
    }

    [HttpPost]
    public async Task<IActionResult> BuyNow(int itemId, int quantity = 1)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        bool result = await _orderService.CreateOrderFromItemAsync(UserId, itemId, quantity);
        if (result)
        {
            return Json(new { success = true, message = "Order placed successfully", redirectUrl = Url.Action("Orders") });
        }
        return Json(new { success = false, message = "Failed to place order" });
    }

    #endregion

    #region Item Detail

    [HttpGet]
    public IActionResult ItemDetails(int itemId)
    {
         ItemViewModel? item = _itemService.GetItemById(itemId);
        if (item == null)
        {
            TempData["ErrorMessage"] = NotificationMessage.SomethingWentWrong;
            RedirectToAction("Dashboard");
        }
        return View(item);
    }

    #endregion

    #region Orders

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<OrderViewModel>? orders = await _orderService.GetUserOrdersAsync(UserId);
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(string orderData)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        OrderViewModel? orderVM = JsonSerializer.Deserialize<OrderViewModel>(orderData);
        bool result = await _orderService.CreateOrderAsync(UserId, orderVM!);
        if (result)
        {
            return Json(new { success = true, message = "Order placed successfully", redirectUrl = Url.Action("Orders") });
        }

        return Json(new { success = false, message = "Failed to place order" });
    }

    [HttpGet]
    public IActionResult GenerateInvoicePDF(int orderid)
    {
        OrderViewModel? orderDetails = _orderService.GetOrderDetailById(orderid);

        if (orderDetails == null)
        {
            TempData["ErrorMessage"] = "Order not found.";
            return RedirectToAction("Orders");
        }

        // return PartialView("Invoice", orderDetails);

        ViewAsPdf PDF = new ViewAsPdf("Invoice", orderDetails)
        {
            FileName = $"OrderInvoice_{orderid}.pdf"
        };
        return PDF;
    }

    #endregion

}