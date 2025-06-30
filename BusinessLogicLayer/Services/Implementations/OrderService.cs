using System.Drawing;
using System.Linq;
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

    public async Task<bool> CreateOrderAsync(int userId, OrderViewModel orderViewModel)
    {
        try
        {
            Order? order = new Order
            {
                TotalAmount = orderViewModel.TotalAmount,
                OrderDate = DateTime.Now,
                CreatedBy = userId
            };

            List<OrderItem>? orderItemsList = new List<OrderItem>();

            foreach (OrderItemViewModel? orderItemViewModel in orderViewModel.OrderItems)
            {
                OrderItem newOrderItem = new OrderItem
                {
                    ItemId = orderItemViewModel.ItemId,
                    ItemName = orderItemViewModel.ItemName,
                    Price = orderItemViewModel.Price,
                    Quantity = orderItemViewModel.Quantity,
                    CreatedBy = userId,
                    CreatedAt = DateTime.Now
                };

                orderItemsList.Add(newOrderItem);
            }

            bool result = await _orderRepo.CreateOrderAsync(order, orderItemsList);

            if (result)
            {
                // Clear cart after successfully Placed order
                List<CartViewModel>? cartItems = await _cartRepo.GetCartItems(userId);
                foreach (CartViewModel? cartItem in cartItems)
                {
                    await _cartRepo.RemoveFromCart(cartItem.Id, userId);
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateOrderAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<List<OrderViewModel>> GetUserOrdersAsync(int userId)
    {
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
                Quantity = oi.Quantity
            }).ToList()
        }).ToList();
    }

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

    public OrderViewModel GetOrderDetailById(int OrderId)
    {
        IQueryable<Order>? Orders = _orderRepo.GetOrderListByModel();

        if (Orders == null) return new OrderViewModel();

        OrderViewModel? orderData = Orders
        .Where(order => order.Id == OrderId)
        .Select(order => new OrderViewModel
        {
            OrderId = order.Id,
            OrderDate = order.OrderDate,
            CustomerName = order.CreatedByUser.Username,
            Email = order.CreatedByUser.Email,
            PhoneNumber = order.CreatedByUser.PhoneNumber,
            Address = order.CreatedByUser.Address,
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
                ImageURL = oi.Items.ItemImages.Where(item => item.ItemId == oi.ItemId).Select(item => item.ImageURL).FirstOrDefault(),
                Price = oi.Price,
                Quantity = oi.Quantity
            }).ToList()
        }).FirstOrDefault();

        return orderData;
    }

    public async Task<bool> UpdateOrderStatus(int orderId, int UserId)
    {
        return await _orderRepo.UpdateOrderStatus(orderId, UserId);
    }

    public async Task<bool> CreateOrderFromItemAsync(int userId, int itemId, int quantity)
    {
        ItemViewModel? item = _itemsService.GetItemById(itemId);
        if (item == null) return false;

        Order? order = new Order
        {
            CreatedBy = userId,
            OrderDate = DateTime.Now,
            TotalAmount = item.Price * quantity,
        };

        OrderItem? orderItem = new OrderItem
        {
            ItemId = itemId,
            Price = item.Price,
            ItemName = item.ItemName,
            CreatedBy = userId,
            CreatedAt = DateTime.Now,
            Quantity = quantity
        };

        return await _orderRepo.CreateOrderAsync(order, new List<OrderItem> { orderItem });
    }

    public Task<byte[]> ExportData(string search = "", string Status = "", int UserId = 0, string fromDate = "", string toDate = "")
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

        if (UserId > 0)
        {
            query = query.Where(o => o.CreatedByUser == UserId);
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

        List<OrderViewModel>? orders = query.ToList();

        // Create Excel package
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage())
        {
            ExcelWorksheet? worksheet = package.Workbook.Worksheets.Add("Orders");
            int currentRow = 3;
            int currentCol = 2;

            // this is first row....................................
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Customer Name: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
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
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;

            if (UserId > 0)
            {
                worksheet.Cells[currentRow, currentCol].Value = query.Where(i => i.CreatedByUser == UserId).Select(i => i.CustomerName).FirstOrDefault();
            }
            else
            {
                worksheet.Cells[currentRow, currentCol].Value = "-";
            }

            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "Search Text: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
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
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;

            if (!string.IsNullOrEmpty(search))
            {
                worksheet.Cells[currentRow, currentCol].Value = search;
            }
            else
            {
                worksheet.Cells[currentRow, currentCol].Value = "-";
            }

            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            // this is second row....................................
            currentRow += 3;
            currentCol = 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "From Date: ";

            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
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
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            if (!string.IsNullOrEmpty(fromDate))
            {
                worksheet.Cells[currentRow, currentCol].Value = fromDate;
            }
            else
            {
                worksheet.Cells[currentRow, currentCol].Value = "-";
            }
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentCol += 5;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "To Date : ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
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
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            if (!string.IsNullOrEmpty(toDate))
            {
                worksheet.Cells[currentRow, currentCol].Value = toDate;
            }
            else
            {
                worksheet.Cells[currentRow, currentCol].Value = "-";
            }
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }

            currentRow += 3;
            currentCol = 2;
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = "No. of Records: ";
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 1])
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
            worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3].Merge = true;
            worksheet.Cells[currentRow, currentCol].Value = orders.Count;
            using (var headingCells = worksheet.Cells[currentRow, currentCol, currentRow + 1, currentCol + 3])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(Color.White);
                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            // this is table ....................................
            int headingRow = currentRow + 4;
            int headingCol = 2;

            worksheet.Cells[headingRow, headingCol].Value = "Order No";
            headingCol++;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Order Date";
            headingCol += 3;  // Move to next unmerged column

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Customer Name";
            headingCol += 3;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 2].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Status";
            headingCol += 3;

            worksheet.Cells[headingRow, headingCol, headingRow, headingCol + 1].Merge = true;
            worksheet.Cells[headingRow, headingCol].Value = "Total Amount";


            using (var headingCells = worksheet.Cells[headingRow, 2, headingRow, headingCol + 1])
            {
                headingCells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headingCells.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#0066A7"));
                headingCells.Style.Font.Bold = true;
                headingCells.Style.Font.Color.SetColor(Color.White);

                headingCells.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);


                headingCells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headingCells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }


            // Populate data
            int row = headingRow + 1;

            foreach (var order in orders)
            {
                int startCol = 2;

                worksheet.Cells[row, startCol].Value = order.OrderId;
                startCol += 1;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = order.OrderDate.ToString();
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                worksheet.Cells[row, startCol].Value = order.CustomerName;
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 2].Merge = true;
                if (order.IsDelivered)
                {
                    worksheet.Cells[row, startCol].Value = "Delivered";
                }
                else
                {
                    worksheet.Cells[row, startCol].Value = "Pending";
                }
                startCol += 3;

                worksheet.Cells[row, startCol, row, startCol + 1].Merge = true;
                worksheet.Cells[row, startCol].Value = order.TotalAmount;

                using (var rowCells = worksheet.Cells[row, 2, row, startCol + 1])
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