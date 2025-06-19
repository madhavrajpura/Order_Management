using System.Diagnostics;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Item_Order_Management.Controllers;

public class AdminController : Controller
{
    private readonly IItemsService _itemService;
    private readonly IJWTService _JWTService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    public AdminController(IItemsService itemService, IJWTService JWTService, IUserService userService, INotificationService notificationService)
    {
        _itemService = itemService;
        _JWTService = JWTService;
        _userService = userService;
        _notificationService = notificationService;
    }

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
        string token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

        var UserId = await _userService.GetUserIdFromToken(token);

        List<NotificationViewModel> NotifyVM = _notificationService.GetNotificationsById(UserId);
        return PartialView("_AdminNavbar", NotifyVM);
    }

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
        string token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

        var UserId = await _userService.GetUserIdFromToken(token);


        if (claims == null || UserId == 0 || string.IsNullOrEmpty(token))
        {
            TempData["ErrorMessage"] = NotificationMessage.TokenExpired;
            return RedirectToAction("Login", "Authentication");
        }

        if (_itemService.CheckItemExists(ItemVM))
        {
            return Json(new { success = false, text = NotificationMessage.AlreadyExists.Replace("{0}", "Item") });
        }

        bool saveStatus = _itemService.SaveItem(ItemVM, UserId);
        return Json(saveStatus
            ? new { success = true, text = ItemVM.ItemId == 0 ? NotificationMessage.CreateSuccess.Replace("{0}", "Item") : NotificationMessage.UpdateSuccess.Replace("{0}", "Item") }
            : new { success = false, text = ItemVM.ItemId == 0 ? NotificationMessage.CreateFailure.Replace("{0}", "Item") : NotificationMessage.UpdateFailure.Replace("{0}", "Item") });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem(int ItemId)
    {
        string token = Request.Cookies["JWTToken"];
        System.Security.Claims.ClaimsPrincipal? claims = _JWTService.GetClaimsFromToken(token!);

        var UserId = await _userService.GetUserIdFromToken(token);


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

}
