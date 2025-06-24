using System.Diagnostics;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Item_Order_Management.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IItemsService _itemService;
    private readonly IJWTService _JWTService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly IOrderService _orderService;
    public AdminController(IItemsService itemService, IJWTService JWTService, IUserService userService, INotificationService notificationService, IOrderService orderService)
    {
        _itemService = itemService;
        _JWTService = JWTService;
        _userService = userService;
        _notificationService = notificationService;
        _orderService = orderService;
    }

    #region Dashboard

    public IActionResult Dashboard()
    {
        // check if Token exist or not
        if (Request.Cookies.ContainsKey("JWTToken"))
        {
            string? token = Request.Cookies["JWTToken"];
            System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

            if (claims != null)
            {
                // User is authenticated, proceed to the dashboard
                return View();
            }
            else
            {
                // Invalid token, redirect to login
                TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
                return RedirectToAction("Login", "Authentication");
            }
        }
        return RedirectToAction("Login", "Authentication");
    }

    public async Task<IActionResult> GetAdminNavbarData()
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);

        List<NotificationViewModel> NotifyVM = _notificationService.GetNotificationsById(UserId);
        return PartialView("_AdminNavbar", NotifyVM);
    }

    #endregion

    #region Notification

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int userNotificationId)
    {
        await _notificationService.MarkNotificationAsRead(userNotificationId);
        string? token = Request.Cookies["JWTToken"];
        var userId = _userService.GetUserIdFromToken(token);
        List<NotificationViewModel> notifications = _notificationService.GetNotificationsById(userId);
        return PartialView("_NotificationList", notifications);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        string? token = Request.Cookies["JWTToken"];
        var userId = _userService.GetUserIdFromToken(token);
        await _notificationService.MarkAllNotificationsAsRead(userId);
        List<NotificationViewModel> notifications = _notificationService.GetNotificationsById(userId);
        return PartialView("_NotificationList", notifications);
    }

    #endregion

    #region Items CRUD

    [HttpGet]
    public IActionResult GetItemList(int pageNumber = 1, string search = "", int pageSize = 5, string sortColumn = "", string sortDirection = "")
    {
        PaginationViewModel<ItemViewModel>? ItemList = _itemService.GetItemList(pageNumber, search, pageSize, sortColumn, sortDirection);
        return PartialView("_ItemList", ItemList);
    }

    [HttpGet]
    public IActionResult GetItemById(int ItemId)
    {
        ItemViewModel ItemVM = (ItemId == 0) ? new ItemViewModel() : _itemService.GetItemById(ItemId);
        return PartialView("_SaveItem", ItemVM);
    }

    [HttpPost]
    public async Task<IActionResult> SaveItem([FromForm] ItemViewModel ItemVM)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);


        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        if (_itemService.CheckItemExists(ItemVM))
        {
            return Json(new { success = false, text = NotificationMessage.AlreadyExists.Replace("{0}", "Item") });
        }

        // Handle thumbnail image
        if (ItemVM.ThumbnailImageFile != null)
        {
            string[] extension = ItemVM.ThumbnailImageFile.FileName.Split(".");
            string ext = extension[extension.Length - 1].ToLower();

            if (new[] { "jpg", "jpeg", "png", "gif", "webp", "jfif" }.Contains(ext))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                string fileName = ImageTemplate.GetFileName(ItemVM.ThumbnailImageFile, path);
                ItemVM.ThumbnailImageUrl = $"/Uploads/{fileName}";
            }
            else
            {
                return Json(new { success = false, text = NotificationMessage.ImageFormat });
            }
        }

        bool saveStatus = _itemService.SaveItem(ItemVM, UserId);
        return Json(saveStatus
            ? new { success = true, text = ItemVM.ItemId == 0 ? NotificationMessage.CreateSuccess.Replace("{0}", "Item") : NotificationMessage.UpdateSuccess.Replace("{0}", "Item") }
            : new { success = false, text = ItemVM.ItemId == 0 ? NotificationMessage.CreateFailure.Replace("{0}", "Item") : NotificationMessage.UpdateFailure.Replace("{0}", "Item") });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem(int ItemId)
    {
        string? token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

        var UserId = _userService.GetUserIdFromToken(token);


        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        bool deleteStatus = _itemService.DeleteItem(ItemId, UserId);
        return Json(deleteStatus
            ? new { success = true, text = NotificationMessage.DeleteSuccess.Replace("{0}", "Item") }
            : new { success = false, text = NotificationMessage.DeleteFailure.Replace("{0}", "Item") });
    }

    #endregion

    #region Orders

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        string? token = Request.Cookies["JWTToken"];
        var claims = _JWTService.GetClaimsFromToken(token);
        int userId = _userService.GetUserIdFromToken(token);

        if (claims == null || userId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        return View();
    }

    public async Task<IActionResult> GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "")
    {
        string? token = Request.Cookies["JWTToken"];
        var claims = _JWTService.GetClaimsFromToken(token);
        int userId = _userService.GetUserIdFromToken(token);

        if (claims == null || userId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        PaginationViewModel<OrderViewModel>? OrderList = _orderService.GetOrderList(search,sortColumn,sortDirection,pageNumber,pageSize,Status);
        return PartialView("_OrderList", OrderList);

    }

    [HttpGet]
    public async Task<IActionResult> OrderDetails(int orderid)
    {
        string? token = Request.Cookies["JWTToken"];
        var claims = _JWTService.GetClaimsFromToken(token);
        int userId = _userService.GetUserIdFromToken(token);

        if (claims == null || userId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        OrderViewModel? orders = await _orderService.GetOrderById(orderid);
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> MarkOrderStatus(int orderId)
    {
        string? token = Request.Cookies["JWTToken"];
        var claims = _JWTService.GetClaimsFromToken(token);
        int userId = _userService.GetUserIdFromToken(token);

        if (claims == null || userId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        bool orders = await _orderService.MarkOrderStatus(orderId);

        if (orders) return Json(new { success = true, text = "Order Marked as Completed" });
        return Json(new { success = false, text = "Failed to mark order" });
    }

    #endregion

}