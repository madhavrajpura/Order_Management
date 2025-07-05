using System.Drawing;
using System.Linq;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Services.Interfaces;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.Interfaces;
using DataAccessLayer.ViewModels;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BusinessLogicLayer.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly IItemsService _itemsService;

    public OrderService(IOrderRepository orderRepo, ICartRepository cartRepo, IItemsService itemsService)
    {
        _orderRepo = orderRepo;
        _cartRepo = cartRepo;
        _itemsService = itemsService;
    }

    // Modified: Added stock validation and decrementing
    public async Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");
        if (orderViewModel == null || orderViewModel.OrderItems == null || !orderViewModel.OrderItems.Any()) 
            throw new CustomException("Invalid order data");

        // Added: Validate stock for each item
        foreach (var orderItem in orderViewModel.OrderItems)
        {
            ItemViewModel? item = _itemsService.GetItemById(orderItem.ItemId);
            if (item == null)
                throw new CustomException($"{orderItem.ItemName} not found");
            if (!await _itemsService.IsItemInStock(orderItem.ItemId))
                throw new CustomException($"{orderItem.ItemName} is out of stock");
            if (orderItem.Quantity > item.Stock)
                throw new CustomException($"Only {item.Stock} units of {orderItem.ItemName} available in stock");
        }

        Order order = new Order
        {
            TotalAmount = orderViewModel.TotalAmount,
            OrderDate = DateTime.Now,
            CreatedBy = userId
        };

        List<OrderItem> orderItemsList = orderViewModel.OrderItems.Select(orderItemViewModel => new OrderItem
        {
            ItemId = orderItemViewModel.ItemId,
            ItemName = orderItemViewModel.ItemName,
            Price = orderItemViewModel.Price,
            Quantity = orderItemViewModel.Quantity,
            CreatedBy = userId,
            CreatedAt = DateTime.Now
        }).ToList();

        // Added: Update stock transactionally
        bool orderCreationStatus = await _orderRepo.CreateOrderAsync(order, orderItemsList, orderViewModel.OrderItems);

        if (orderCreationStatus)
        {
            // Modified: Clear cart only after successful order and stock update
            List<CartViewModel> cartItems = await _cartRepo.GetCartItems(userId);
            foreach (CartViewModel cartItem in cartItems)
            {
                await _cartRepo.RemoveFromCart(cartItem.Id, userId);
            }
        }

        return orderCreationStatus;
    }

    // Modified: Added stock validation and decrementing
    public async Task<bool> CreateOrderFromItemAsync(int userId, int itemId, int quantity)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");
        if (itemId <= 0) throw new CustomException("Invalid Item ID.");
        if (quantity <= 0) throw new CustomException("Invalid Quantity.");

        ItemViewModel? item = _itemsService.GetItemById(itemId);
        if (item == null) throw new CustomException("Item not found");
        if (!await _itemsService.IsItemInStock(itemId))
            throw new CustomException($"{item.ItemName} is out of stock");
        if (quantity > item.Stock)
            throw new CustomException($"Only {item.Stock} units of {item.ItemName} available in stock");

        Order order = new Order
        {
            CreatedBy = userId,
            OrderDate = DateTime.Now,
            TotalAmount = item.Price * quantity,
        };

        OrderItem orderItem = new OrderItem
        {
            ItemId = itemId,
            Price = item.Price,
            ItemName = item.ItemName,
            CreatedBy = userId,
            CreatedAt = DateTime.Now,
            Quantity = quantity
        };

        // Added: Pass single item for stock update
        return await _orderRepo.CreateOrderAsync(order, new List<OrderItem> { orderItem }, 
            new List<OrderItemViewModel> { new OrderItemViewModel { ItemId = itemId, Quantity = quantity, ItemName = item.ItemName } });
    }

    // Get Order By User ID
    public async Task<List<OrderViewModel>> GetUserOrdersAsync(int userId)
    {
        if (userId <= 0) throw new CustomException("Invalid User ID.");

        List<Order>? orders = await _orderRepo.GetUserOrdersAsync(userId);

        return orders.Select(o => new OrderViewModel
        {
            OrderId = o.Id,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            DeliveryDate = o.DeliveryDate,
            IsDelivered = o.IsDelivered,
            IsDelete = o.IsDelete,
            OrderItems = o.OrderItems.Where(oi => !oi.IsDelete).Select(oi => new OrderItemViewModel
            {
                OrderItemId = oi.Id,
                OrderId = (int)oi.OrderId,
                ItemId = oi.ItemId,
                ItemName = oi.ItemName,
                Price = oi.Price,
                Stock = oi.Item.Stock,
                Quantity = oi.Quantity
            }).ToList()
        }).ToList();
    }

    // Get Order List
    public PaginationViewModel<OrderViewModel> GetOrderList(string search = "", string sortColumn = "", string sortDirection = "", int pageNumber = 1, int pageSize = 5, string Status = "", int userId = 0, string fromDate = "", string toDate = "")
    {
        IQueryable<OrderViewModel>? query = _orderRepo.GetOrderList();

        // Apply search 
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();

            query = query.Where(u =>
                u.OrderId.ToString().ToLower().Contains(lowerSearchTerm) ||
                u.TotalAmount.ToString().Contains(lowerSearchTerm) ||
                u.CustomerName.Contains(lowerSearchTerm)
            );
        }

        // Apply filter
        if (Status == "Pending")
        {
            query = query.Where(o => !o.IsDelivered);
        }
        else if (Status == "Delivered")
        {
            query = query.Where(o => o.IsDelivered);
        }

        if (userId > 0)
        {
            query = query.Where(o => o.CreatedByUser == userId);
        }

        // Apply date filter
        if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
        {
            query = query.Where(u => u.OrderDate >= DateTime.Parse(fromDate) && u.OrderDate <= DateTime.Parse(toDate).AddDays(1));
        }
        else if (!string.IsNullOrEmpty(toDate))
        {
            query = query.Where(u => u.OrderDate <= DateTime.Parse(toDate).AddDays(1));
        }
        else if (!string.IsNullOrEmpty(fromDate))
        {
            query = query.Where(u => u.OrderDate >= DateTime.Parse(fromDate));
        }

        // Get total records count (before pagination)
        int totalCount = query.Count();

        //sorting
        switch (sortColumn)
        {
            case "ID":
                query = sortDirection == "asc" ? query.OrderBy(u => u.OrderId) : query.OrderByDescending(u => u.OrderId);
                break;
            case "Date":
                query = sortDirection == "asc" ? query.OrderBy(u => u.OrderDate) : query.OrderByDescending(u => u.OrderDate);
                break;
            case "Amount":
                query = sortDirection == "asc" ? query.OrderBy(u => u.TotalAmount) : query.OrderByDescending(u => u.TotalAmount);
                break;
            case "Name":
                query = sortDirection == "asc" ? query.OrderBy(u => u.CustomerName) : query.OrderByDescending(u => u.CustomerName);
                break;
        }

        // Apply pagination
        List<OrderViewModel>? items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginationViewModel<OrderViewModel>(items, totalCount, pageNumber, pageSize);
    }


    // Get Order Detail by Order ID.
    public OrderViewModel GetOrderDetailById(int OrderId)
    {
        if (OrderId <= 0) throw new CustomException("Invalid Order ID.");

        IQueryable<Order>? Orders = _orderRepo.GetOrderListByModel();

        if (Orders == null) return new OrderViewModel();

        OrderViewModel? orderData = Orders
        .Where(order => order.Id == OrderId)
        .Select(order => new OrderViewModel
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            CustomerName = order.CreatedByNavigation.Username,
            Email = order.CreatedByNavigation.Email,
            PhoneNumber = order.CreatedByNavigation.PhoneNumber,
            Address = order.CreatedByNavigation.Address,
            TotalAmount = order.TotalAmount,
            DeliveryDate = order.DeliveryDate,
            IsDelivered = order.IsDelivered,
            IsDelete = order.IsDelete,
            OrderItems = order.OrderItems.Where(oi => !oi.IsDelete).Select(oi => new OrderItemViewModel
            {
                OrderItemId = oi.Id,
                OrderId = (int)oi.OrderId,
                ItemId = oi.ItemId,
                ItemName = oi.ItemName,
                ImageURL = oi.Item.ItemImages.Where(item => item.ItemId == oi.ItemId).Select(item => item.ImageUrl).FirstOrDefault(),
                Price = oi.Price,
                Stock = oi.Item.Stock,
                Quantity = oi.Quantity
            }).ToList()
        }).FirstOrDefault();

        return orderData;
    }

    // Update Order Status.
    public async Task<bool> UpdateOrderStatus(int orderId, int UserId)
    {
        if (UserId <= 0) throw new CustomException("Invalid User ID.");
        if (orderId <= 0) throw new CustomException("Invalid order ID.");
        return await _orderRepo.UpdateOrderStatus(orderId, UserId);
    }

    // Excel Export Functionality
    public Task<byte[]> ExportData(string search = "", string Status = "", int UserId = 0, string fromDate = "", string toDate = "")
    {
        IQueryable<OrderViewModel>? query = _orderRepo.GetOrderList();
        IQueryable<OrderViewModel>? allDataQuery = _orderRepo.GetOrderList(); // Separate query for all data

        // Apply filters for filtered data sheet
        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearchTerm = search.ToLower();
            query = query.Where(u =>
                u.OrderId.ToString().ToLower().Contains(lowerSearchTerm) ||
                u.TotalAmount.ToString().Contains(lowerSearchTerm) ||
                u.CustomerName.Contains(lowerSearchTerm)
            );
        }

        if (Status == "Pending")
        {
            query = query.Where(o => !o.IsDelivered);
        }
        else if (Status == "Delivered")
        {
            query = query.Where(o => o.IsDelivered);
        }

        if (UserId > 0)
        {
            query = query.Where(o => o.CreatedByUser == UserId);
        }

        if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
        {
            query = query.Where(u => u.OrderDate >= DateTime.Parse(fromDate) && u.OrderDate <= DateTime.Parse(toDate).AddDays(1));
        }
        else if (!string.IsNullOrEmpty(toDate))
        {
            query = query.Where(u => u.OrderDate <= DateTime.Parse(toDate).AddDays(1));
        }
        else if (!string.IsNullOrEmpty(fromDate))
        {
            query = query.Where(u => u.OrderDate >= DateTime.Parse(fromDate));
        }

        List<OrderViewModel>? filteredOrders = query.ToList();
        List<OrderViewModel>? allOrders = allDataQuery.ToList();

        // Create Excel package
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            // Sheet 1: All Data
            ExcelWorksheet? allDataSheet = package.Workbook.Worksheets.Add("All Orders");
            int currentRow = 3;
            int currentCol = 2;

            // Header for All Data Sheet
            allDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            allDataSheet.Cells[currentRow, currentCol].Value = "No. of Records: ";
            using (var headingCells = allDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            allDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            allDataSheet.Cells[currentRow, currentCol].Value = allOrders.Count;
            using (var headingCells = allDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Table headers for All Data Sheet
            int headingRow = currentRow + 4;
            int headingCol = 2;
            allDataSheet.Cells[headingRow, headingCol].Value = "Order No";
            headingCol++;
            allDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            allDataSheet.Cells[headingRow, headingCol].Value = "Order Date";
            headingCol += 3;
            allDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            allDataSheet.Cells[headingRow, headingCol].Value = "Customer Name";
            headingCol += 3;
            allDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            allDataSheet.Cells[headingRow, headingCol].Value = "Status";
            headingCol += 3;
            allDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            allDataSheet.Cells[headingRow, headingCol].Value = "Total Amount";
            using (var headingCells = allDataSheet.Cells[headingRow, 2, headingRow, headingCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Populate All Data Sheet
            int row = headingRow + 1;
            foreach (var order in allOrders)
            {
                int startCol = 2;
                allDataSheet.Cells[row, startCol].Value = order.OrderId;
                startCol += 1;
                allDataSheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                allDataSheet.Cells[row, startCol].Value = order.OrderDate.ToString();
                startCol += 3;
                allDataSheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                allDataSheet.Cells[row, startCol].Value = order.CustomerName;
                startCol += 3;
                allDataSheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                allDataSheet.Cells[row, startCol].Value = order.IsDelivered ? "Delivered" : "Pending";
                startCol += 3;
                allDataSheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                allDataSheet.Cells[row, startCol].Value = order.TotalAmount;
                using (var rowCells = allDataSheet.Cells[row, 2, row, startCol + 1])
                {
                    if (row % 2 == 0)
                    {
                        rowCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rowCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }
                    rowCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    rowCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                row++;
            }

            // Sheet 2: Filtered Data
            ExcelWorksheet? filteredDataSheet = package.Workbook.Worksheets.Add("Filtered Orders");
            currentRow = 3;
            currentCol = 2;

            // Headers for Filtered Data Sheet
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = "Customer Name: ";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = UserId > 0 ? filteredOrders.FirstOrDefault(i => i.CreatedByUser == UserId)?.CustomerName ?? "-" : "-";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = "Search Text: ";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = !string.IsNullOrEmpty(search) ? search : "-";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentRow += 3;
            currentCol = 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = "From Date: ";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = !string.IsNullOrEmpty(fromDate) ? fromDate : "-";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 5;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = "To Date: ";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = !string.IsNullOrEmpty(toDate) ? toDate : "-";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentRow += 3;
            currentCol = 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = "No. of Records: ";
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            currentCol += 2;
            filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            filteredDataSheet.Cells[currentRow, currentCol].Value = filteredOrders.Count;
            using (var headingCells = filteredDataSheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Table headers for Filtered Data Sheet
            headingRow = currentRow + 4;
            headingCol = 2;
            filteredDataSheet.Cells[headingRow, headingCol].Value = "Order No";
            headingCol++;
            filteredDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            filteredDataSheet.Cells[headingRow, headingCol].Value = "Order Date";
            headingCol += 3;
            filteredDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            filteredDataSheet.Cells[headingRow, headingCol].Value = "Customer Name";
            headingCol += 3;
            filteredDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            filteredDataSheet.Cells[headingRow, headingCol].Value = "Status";
            headingCol += 3;
            filteredDataSheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            filteredDataSheet.Cells[headingRow, headingCol].Value = "Total Amount";
            using (var headingCells = filteredDataSheet.Cells[headingRow, 2, headingRow, headingCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // Populate Filtered Data Sheet
            row = headingRow + 1;
            foreach (var order in filteredOrders)
            {
                int startCol = 2;
                filteredDataSheet.Cells[row, startCol].Value = order.OrderId;
                startCol += 1;
                filteredDataSheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                filteredDataSheet.Cells[row, startCol].Value = order.OrderDate.ToString();
                startCol += 3;
                filteredDataSheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                filteredDataSheet.Cells[row, startCol].Value = order.CustomerName;
                startCol += 3;
                filteredDataSheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                filteredDataSheet.Cells[row, startCol].Value = order.IsDelivered ? "Delivered" : "Pending";
                startCol += 3;
                filteredDataSheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                filteredDataSheet.Cells[row, startCol].Value = order.TotalAmount;
                using (var rowCells = filteredDataSheet.Cells[row, 2, row, startCol + 1])
                {
                    if (row % 2 == 0)
                    {
                        rowCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rowCells.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }
                    rowCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                    rowCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    rowCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                row++;
            }

            return Task.FromResult(package.GetAsByteArray());
        }
    }

}