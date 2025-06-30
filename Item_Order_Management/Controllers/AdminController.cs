using System.Diagnostics;
using System.Security.Claims;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

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
        return View();
    }

    public IActionResult GetAdminNavbarData()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<NotificationViewModel> NotifyVM = _notificationService.GetNotificationsById(UserId);
        return PartialView("_AdminNavbar", NotifyVM);
    }

    #endregion

    #region Notification

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int userNotificationId)
    {
        await _notificationService.MarkNotificationAsRead(userNotificationId);
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        List<NotificationViewModel> NotificationList = _notificationService.GetNotificationsById(UserId);
        return PartialView("_NotificationList", NotificationList);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAllAsRead()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        await _notificationService.MarkAllNotificationsAsRead(UserId);
        List<NotificationViewModel> NotificationList = _notificationService.GetNotificationsById(UserId);
        return PartialView("_NotificationList", NotificationList);
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
    public async Task<IActionResult> SaveItemAsync([FromForm] ItemViewModel ItemVM)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (await _itemService.CheckItemExists(ItemVM))
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

        string NewAdditionalImagesURL;
        List<string> NewAdditionalImageList = new List<string>();
        foreach (IFormFile? AdditionalImagesFiles in ItemVM.AdditionalImagesFile!)
        {
            if (AdditionalImagesFiles != null)
            {
                string[] extension = AdditionalImagesFiles.FileName.Split(".");
                string ext = extension[extension.Length - 1].ToLower();

                if (new[] { "jpg", "jpeg", "png", "gif", "webp", "jfif" }.Contains(ext))
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    string fileName = ImageTemplate.GetFileName(AdditionalImagesFiles, path);
                    NewAdditionalImagesURL = $"/Uploads/{fileName}";
                    NewAdditionalImageList.Add(NewAdditionalImagesURL);
                }
                else
                {
                    return Json(new { success = false, text = NotificationMessage.ImageFormat });
                }
            }
        }

        bool saveStatus = await _itemService.SaveItem(ItemVM, UserId, NewAdditionalImageList);
        return Json(saveStatus
            ? new { success = true, text = ItemVM.ItemId == 0 ? NotificationMessage.CreateSuccess.Replace("{0}", "Item") : NotificationMessage.UpdateSuccess.Replace("{0}", "Item") }
            : new { success = false, text = ItemVM.ItemId == 0 ? NotificationMessage.CreateFailure.Replace("{0}", "Item") : NotificationMessage.UpdateFailure.Replace("{0}", "Item") });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem(int ItemId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        bool deleteStatus = await _itemService.DeleteItem(ItemId, UserId);
        return Json(deleteStatus
            ? new { success = true, text = NotificationMessage.DeleteSuccess.Replace("{0}", "Item") }
            : new { success = false, text = NotificationMessage.DeleteFailure.Replace("{0}", "Item") });
    }

    #endregion

    #region Orders

    [HttpGet]
    public IActionResult Orders()
    {
        return View();
    }

    public IActionResult GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "", int UserId = 0, string fromDate = "", string toDate = "")
    {
        PaginationViewModel<OrderViewModel>? OrderList = _orderService.GetOrderList(search, sortColumn, sortDirection, pageNumber, pageSize, Status, UserId, fromDate, toDate);
        return PartialView("_OrderList", OrderList);
    }

    [HttpGet]
    public IActionResult OrderDetails(int orderid)
    {
        OrderViewModel? orders = _orderService.GetOrderDetailById(orderid);
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int orderId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        bool OrderStatus = await _orderService.UpdateOrderStatus(orderId, UserId);
        if (OrderStatus) return Json(new { success = true, text = NotificationMessage.OrderCompleteMark });
        return Json(new { success = false, text = NotificationMessage.OrderCompleteMarkFailure });
    }

    public async Task<IActionResult> ExportOrderDataToExcel(string search = "", string Status = "", int UserId = 0, string fromDate = "", string toDate = "")
    {
        byte[]? FileData = await _orderService.ExportData(search, Status, UserId, fromDate, toDate);
        return File(FileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders.xlsx");
    }

    #endregion

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var UserList = _userService.GetAllUsers()
            .Select(u => new { id = u.Id, name = u.Username })
            .OrderBy(u => u.name)
            .ToList();
        return Json(UserList);
    }

}