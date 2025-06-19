// // using BusinessLogicLayer.Services.Interfaces;
// // using DataAccessLayer.ViewModels;
// // using Microsoft.AspNetCore.Mvc;
// // using System.Threading.Tasks;

// // namespace Item_Order_Management.Controllers;

// // public class UserController : Controller
// // {
// //     private readonly IItemsService _itemService;
// //     private readonly IJWTService _jwtService;
// //     private readonly IUserService _userService;

// //     public UserController(IItemsService itemService, IJWTService jwtService, IUserService userService)
// //     {
// //         _itemService = itemService;
// //         _jwtService = jwtService;
// //         _userService = userService;
// //     }

// //     public IActionResult Dashboard()
// //     {
// //         if (Request.Cookies.ContainsKey("JWTToken"))
// //         {
// //             string token = Request.Cookies["JWTToken"];
// //             var claims = _jwtService.GetClaimsFromToken(token);
// //             if (claims != null)
// //             {
// //                 return View();
// //             }
// //             TempData["ErrorMessage"] = "Token expired. Please login again.";
// //             return RedirectToAction("Login", "Authentication");
// //         }
// //         return RedirectToAction("Login", "Authentication");
// //     }

// //     [HttpGet]
// //     public IActionResult GetItemList(int pageNumber = 1, string search = "", int pageSize = 10, string sortColumn = "ID", string sortDirection = "asc")
// //     {
// //         var itemList = _itemService.GetItemList(pageNumber, search, pageSize, sortColumn, sortDirection);
// //         return PartialView("_ItemList", itemList);
// //     }

// //     [HttpGet]
// //     public async Task<IActionResult> GetUserNavbarData()
// //     {
// //         string token = Request.Cookies["JWTToken"];
// //         var userId = await _userService.GetUserIdFromToken(token);
// //         return PartialView("_UserNavbar", userId);
// //     }
// // }

// using BusinessLogicLayer.Services.Interfaces;
// using DataAccessLayer.ViewModels;
// using Microsoft.AspNetCore.Mvc;
// using System.Threading.Tasks;

// namespace Item_Order_Management.Controllers;

// public class UserController : Controller
// {
//     private readonly IItemsService _itemService;
//     private readonly IOrderService _orderService;
//     private readonly IJWTService _jwtService;
//     private readonly IUserService _userService;

//     public UserController(IItemsService itemService, IOrderService orderService, IJWTService jwtService, IUserService userService)
//     {
//         _itemService = itemService;
//         _orderService = orderService;
//         _jwtService = jwtService;
//         _userService = userService;
//     }

//     public IActionResult Dashboard()
//     {
//         if (Request.Cookies.ContainsKey("JWTToken"))
//         {
//             string token = Request.Cookies["JWTToken"];
//             var claims = _jwtService.GetClaimsFromToken(token);
//             if (claims != null)
//             {
//                 return View();
//             }
//             TempData["ErrorMessage"] = "Token expired. Please login again.";
//             return RedirectToAction("Login", "Authentication");
//         }
//         return RedirectToAction("Login", "Authentication");
//     }

//     [HttpGet]
//     public IActionResult GetItemList(int pageNumber = 1, string search = "", int pageSize = 10, string sortColumn = "ID", string sortDirection = "asc")
//     {
//         var itemList = _itemService.GetItemList(pageNumber, search, pageSize, sortColumn, sortDirection);
//         return PartialView("_ItemList", itemList);
//     }

//     [HttpGet]
//     public IActionResult GetOrderList(int pageNumber = 1, string search = "", int pageSize = 15, string sortColumn = "ID", string sortDirection = "asc")
//     {
//         var orderList = _orderService.GetOrderList(pageNumber, search, pageSize, sortColumn, sortDirection);
//         return PartialView("_OrderList", orderList);
//     }

//     [HttpGet]
//     public IActionResult GetOrderById(int orderId)
//     {
//         var orderVM = orderId == 0 ? new OrderViewModel { OrderDate = DateTime.Today, DeliveryDate = DateTime.Today.AddDays(15) } : _orderService.GetOrderById(orderId);
//         return PartialView("_SaveOrder", orderVM);
//     }

//     [HttpPost]
//     public async Task<IActionResult> SaveOrder([FromForm] OrderViewModel orderVM)
//     {
//         string token = Request.Cookies["JWTToken"];
//         var userId = await _userService.GetUserIdFromToken(token);

//         if (userId == 0 || string.IsNullOrEmpty(token))
//         {
//             return Json(new { success = false, text = "Authentication failed. Please login again." });
//         }

//         if (orderVM.DeliveryDate < orderVM.OrderDate.AddDays(15))
//         {
//             return Json(new { success = false, text = "Delivery date must be at least 15 days after order date." });
//         }

//         if (orderVM.OrderId > 0 && (DateTime.UtcNow - orderVM.OrderDate).TotalDays > 2)
//         {
//             return Json(new { success = false, text = "Order cannot be edited after 2 days from order date." });
//         }

//         if (_orderService.CheckOrderExists(orderVM))
//         {
//             return Json(new { success = false, text = "An order with this customer name and item already exists." });
//         }

//         bool saveStatus = _orderService.SaveOrder(orderVM, userId);
//         return Json(saveStatus
//             ? new { success = true, text = orderVM.OrderId == 0 ? "Order created successfully." : "Order updated successfully." }
//             : new { success = false, text = orderVM.OrderId == 0 ? "Failed to create order." : "Failed to update order." });
//     }

//     [HttpPost]
//     public async Task<IActionResult> DeleteOrder(int orderId)
//     {
//         string token = Request.Cookies["JWTToken"];
//         var userId = await _userService.GetUserIdFromToken(token);

//         if (userId == 0 || string.IsNullOrEmpty(token))
//         {
//             return Json(new { success = false, text = "Authentication failed. Please login again." });
//         }

//         bool deleteStatus = _orderService.DeleteOrder(orderId, userId);
//         return Json(deleteStatus
//             ? new { success = true, text = "Order deleted successfully." }
//             : new { success = false, text = "Failed to delete order." });
//     }

//     [HttpGet]
//     public IActionResult GetItemsForAutocomplete(string term)
//     {
//         var items = _orderService.GetItemsForAutocomplete(term);
//         return Json(items.Select(i => new { label = i.ItemName, value = i.ItemName, id = i.ItemId, price = i.Price }));
//     }

//     [HttpGet]
//     public async Task<IActionResult> GetUserNavbarData()
//     {
//         string token = Request.Cookies["JWTToken"];
//         var userId = await _userService.GetUserIdFromToken(token);
//         return PartialView("_UserNavbar", userId);
//     }
// }

using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Item_Order_Management.Controllers;

public class UserController : Controller
{
    private readonly IItemsService _itemService;
    private readonly IOrderService _orderService;
    private readonly IJWTService _jwtService;
    private readonly IUserService _userService;

    public UserController(IItemsService itemService, IOrderService orderService, IJWTService jwtService, IUserService userService)
    {
        _itemService = itemService;
        _orderService = orderService;
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

    public IActionResult Orders()
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
    public IActionResult GetOrderList(int pageNumber = 1, string search = "", int pageSize = 15, string sortColumn = "ID", string sortDirection = "asc")
    {
        var orderList = _orderService.GetOrderList(pageNumber, search, pageSize, sortColumn, sortDirection);
        return PartialView("_OrderList", orderList);
    }

    [HttpGet]
public IActionResult GetOrderById(int orderId)
{
    var orderVM = orderId == 0 ? new OrderViewModel { OrderDate = DateTime.Today, DeliveryDate = DateTime.Today.AddDays(15) } : _orderService.GetOrderById(orderId);
    return PartialView("_SaveOrder", orderVM);
}

    [HttpPost]
    public async Task<IActionResult> SaveOrder([FromForm] OrderViewModel orderVM)
    {
        string token = Request.Cookies["JWTToken"];
        var userId = await _userService.GetUserIdFromToken(token);

        if (userId == 0 || string.IsNullOrEmpty(token))
        {
            return Json(new { success = false, text = "Authentication failed. Please login again." });
        }

        if (orderVM.DeliveryDate < orderVM.OrderDate.AddDays(15))
        {
            return Json(new { success = false, text = "Delivery date must be at least 15 days after order date." });
        }

        if (orderVM.OrderId > 0 && (DateTime.UtcNow - orderVM.OrderDate).TotalDays > 2)
        {
            return Json(new { success = false, text = "Order cannot be edited after 2 days from order date." });
        }

        if (_orderService.CheckOrderExists(orderVM))
        {
            return Json(new { success = false, text = "An order with this customer name and item already exists." });
        }

        bool saveStatus = _orderService.SaveOrder(orderVM, userId);
        return Json(saveStatus
            ? new { success = true, text = orderVM.OrderId == 0 ? "Order created successfully." : "Order updated successfully." }
            : new { success = false, text = orderVM.OrderId == 0 ? "Failed to create order." : "Failed to update order." });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteOrder(int orderId)
    {
        string token = Request.Cookies["JWTToken"];
        var userId = await _userService.GetUserIdFromToken(token);

        if (userId == 0 || string.IsNullOrEmpty(token))
        {
            return Json(new { success = false, text = "Authentication failed. Please login again." });
        }

        bool deleteStatus = _orderService.DeleteOrder(orderId, userId);
        return Json(deleteStatus
            ? new { success = true, text = "Order deleted successfully." }
            : new { success = false, text = "Failed to delete order." });
    }

    [HttpGet]
    public IActionResult GetItemsForAutocomplete(string term)
    {
        var items = _orderService.GetItemsForAutocomplete(term);
        return Json(items.Select(i => new { label = i.ItemName, value = i.ItemName, id = i.ItemId, price = i.Price }));
    }

    [HttpGet]
    public async Task<IActionResult> GetUserNavbarData()
    {
        string token = Request.Cookies["JWTToken"];
        var userId = await _userService.GetUserIdFromToken(token);
        return PartialView("_UserNavbar", userId);
    }
}