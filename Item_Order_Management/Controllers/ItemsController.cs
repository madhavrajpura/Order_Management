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

    public readonly ICouponService _couponService;

    public ItemsController(IItemsService itemService, IJWTService jwtService, IUserService userService, IWishListService wishListService, ICartService cartService, IOrderService orderService, INotificationService notificationService, ICouponService couponService)
    {
        _itemService = itemService;
        _jwtService = jwtService;
        _userService = userService;
        _wishListService = wishListService;
        _cartService = cartService;
        _orderService = orderService;
        _notificationService = notificationService;
        _couponService = couponService;
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

    [HttpPost]
    public async Task<IActionResult> RequestStockNotification(int itemId, string itemName)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ItemViewModel? item = _itemService.GetItemById(itemId);
        if (item == null)
        {
            return Json(new { success = false, message = "Item not found" });
        }

        if (item.Stock > 0)
        {
            return Json(new { success = false, message = $"{itemName} is already in stock" });
        }

        bool alreadyRequested = await _notificationService.HasUserRequestedNotification(userId, itemId);
        if (alreadyRequested)
        {
            return Json(new { success = false, message = $"You have already requested a notification for {itemName}" });
        }

        bool result = await _notificationService.RequestStockNotificationAsync(userId, itemId, itemName);
        if (result)
        {
            return Json(new { success = true, message = $"You will be notified when {itemName} is back in stock" });
        }
        return Json(new { success = false, message = "Failed to request notification" });
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

        if (!await _itemService.IsItemInStock(itemId))
        {
            return Json(new { success = false, message = $"{item.ItemName} is out of stock" });
        }

        if (await _cartService.IsItemExistsInCart(UserId, itemId))
        {
            return Json(new { success = false, message = $"{item.ItemName} already exists in cart" });
        }

        if (quantity > item.Stock)
        {
            return Json(new { success = false, message = $"Only {item.Stock} units of {item.ItemName} available in stock" });
        }

        bool IsItemAddedToCart = await _cartService.AddToCart(UserId, itemId, quantity);

        if (IsItemAddedToCart)
        {
            return Json(new { success = true, message = "Item added to cart" });
        }
        return Json(new { success = false, message = "Failed to add item into cart" });
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

        CartViewModel? cartItem = await _cartService.GetCartItemById(cartId, UserId);
        if (cartItem == null)
        {
            return Json(new { success = false, message = "Cart item not found" });
        }

        ItemViewModel? item = _itemService.GetItemById(cartItem.ItemId);
        if (item == null)
        {
            return Json(new { success = false, message = "Item not found" });
        }

        if (quantity > item.Stock && quantity > cartItem.Quantity)
        {
            return Json(new { success = false, message = $"Only {item.Stock} units of {cartItem.ItemName} available in stock" });
        }

        bool result = await _cartService.UpdateCartQuantity(cartId, UserId, quantity);

        if (result)
        {
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Failed to update cart quantity" });
    }

    [HttpPost]
    public async Task<IActionResult> AddAllFromWishlistToCart()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<WishListViewModel>? wishlistItems = await _wishListService.GetUserWishlist(userId);

        if (wishlistItems == null || !wishlistItems.Any())
        {
            return Json(new { success = false, message = "Wishlist is empty" });
        }

        bool anyAdded = false;
        bool allOutOfStock = true;

        foreach (var item in wishlistItems)
        {
            if (await _itemService.IsItemInStock(item.ItemId))
            {
                if (!await _cartService.IsItemExistsInCart(userId, item.ItemId))
                {
                    bool result = await _cartService.AddToCart(userId, item.ItemId, 1);
                    if (result)
                    {
                        anyAdded = true;
                        allOutOfStock = false;
                    }
                }
            }
        }

        if (anyAdded)
        {
            return Json(new { success = true, message = allOutOfStock ? "Some items are out of stock" : "Available wishlist items added to cart" });
        }

        return Json(new { success = false, message = "Some items are out of stock" });
    }

    [HttpPost]
    public async Task<IActionResult> BuyNow(int itemId, int quantity = 1)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        ItemViewModel? item = _itemService.GetItemById(itemId);
        if (item == null)
        {
            return Json(new { success = false, message = "Item not found" });
        }

        if (!await _itemService.IsItemInStock(itemId))
        {
            return Json(new { success = false, message = $"{item.ItemName} is out of stock" });
        }

        if (quantity > item.Stock)
        {
            return Json(new { success = false, message = $"Only {item.Stock} units of {item.ItemName} available in stock" });
        }

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

    // [HttpPost]
    // public async Task<IActionResult> CreateOrder(string orderData)
    // {
    //     int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    //     OrderViewModel orderVM = new OrderViewModel();
    //     orderVM = JsonSerializer.Deserialize<OrderViewModel>(orderData);

    //     if (orderVM == null || orderVM.OrderItems == null || !orderVM.OrderItems.Any())
    //     {
    //         return Json(new { success = false, message = "No items in the order" });
    //     }

    //     // Check stock validation
    //     foreach (var item in orderVM.OrderItems)
    //     {
    //         ItemViewModel? itemDetails = _itemService.GetItemById(item.ItemId);
    //         if (itemDetails == null)
    //         {
    //             return Json(new { success = false, message = $"{item.ItemName} not found" });
    //         }
    //         if (!await _itemService.IsItemInStock(item.ItemId))
    //         {
    //             return Json(new { success = false, message = $"{item.ItemName} is out of stock" });
    //         }
    //         if (item.Quantity > itemDetails.Stock)
    //         {
    //             return Json(new { success = false, message = $"Only {itemDetails.Stock} units of {item.ItemName} available in stock" });
    //         }
    //     }

    //     bool result = await _orderService.CreateOrderAsync(UserId, orderVM);
    //     if (result)
    //     {
    //         return Json(new { success = true, message = "Order placed successfully", redirectUrl = Url.Action("Orders") });
    //     }
    //     return Json(new { success = false, message = "Failed to place order" });
    // }

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

    [HttpGet]
    public async Task<IActionResult> GetAvailableCoupons()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var cartItems = await _cartService.GetCartItems(userId);
        decimal cartSubtotal = cartItems.Sum(i => i.Quantity * i.Price);
        var coupons = await _couponService.GetAvailableCouponsAsync(userId, cartSubtotal);
        return Json(coupons);
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(string code, decimal cartSubtotal)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var (isValid, errorMessage, coupon, discount) = await _couponService.ValidateCouponAsync(code, userId, cartSubtotal);
        if (!isValid)
        {
            return Json(new { success = false, message = errorMessage });
        }

        return Json(new
        {
            success = true,
            message = "Coupon applied successfully",
            discount = discount,
            couponCode = coupon.Code
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(string orderData)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        OrderViewModel orderVM = new OrderViewModel();
        orderVM = JsonSerializer.Deserialize<OrderViewModel>(orderData);

        if (orderVM == null || orderVM.OrderItems == null || !orderVM.OrderItems.Any())
        {
            return Json(new { success = false, message = "No items in the order" });
        }

        var cartItems = await _cartService.GetCartItems(userId);
        decimal cartSubtotal = cartItems.Sum(i => i.Quantity * i.Price);

        foreach (var item in orderVM.OrderItems)
        {
            ItemViewModel itemDetails = _itemService.GetItemById(item.ItemId);
            if (itemDetails == null)
            {
                return Json(new { success = false, message = $"{item.ItemName} not found" });
            }
            if (!await _itemService.IsItemInStock(item.ItemId))
            {
                return Json(new { success = false, message = $"{item.ItemName} is out of stock" });
            }
            if (item.Quantity > itemDetails.Stock)
            {
                return Json(new { success = false, message = $"Only {itemDetails.Stock} units of {item.ItemName} available in stock" });
            }
        }

        decimal totalDiscount = 0;
        orderVM.CouponCodes = orderVM.CouponCodes ?? new List<string>();
        foreach (var code in orderVM.CouponCodes)
        {
            var (isValid, errorMessage, coupon, discount) = await _couponService.ValidateCouponAsync(code, userId, cartSubtotal);
            if (!isValid)
            {
                return Json(new { success = false, message = $"Coupon {code} invalid: {errorMessage}" });
            }
            totalDiscount += discount;
        }

        orderVM.DiscountAmount = totalDiscount;
        orderVM.TotalAmount = cartSubtotal - totalDiscount;
        orderVM.OrderDate = DateTime.Now;
        orderVM.CreatedByUser = userId;

        bool result = await _orderService.CreateOrderAsync(userId, orderVM);
        if (result)
        {
            foreach (var code in orderVM.CouponCodes)
            {
                var (isValid, _, coupon, _) = await _couponService.ValidateCouponAsync(code, userId, cartSubtotal);
                if (isValid)
                {
                    await _couponService.RecordCouponUsageAsync(coupon.CouponId, userId, orderVM.OrderId);
                }
            }
            return Json(new { success = true, message = "Order placed successfully", redirectUrl = Url.Action("Orders") });
        }
        return Json(new { success = false, message = "Failed to place order" });
    }

}