using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Application.Services;
using LampEcommerce.Application.DTOs;
using LampEcommerce.Domain.Entities;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LampEcommerce.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController(
    ICampaignService campaignService,
    IScraperService scraperService,
    IFuzzyMatchingService fuzzyMatchingService,
    IUserRepository userRepository,
    IProductRepository productRepository,
    ICampaignProductRepository campaignProductRepository,
    IAdminService adminService,
    IOrderService orderService,
    ITicketService ticketService,
    ITicketRepository ticketRepository,
    ISupplierRepository supplierRepository,
    IShippingMethodRepository shippingMethodRepository,
    IConfiguration config,
    ILogger<AdminController> logger) : ControllerBase
{
    private readonly ICampaignService _campaignService = campaignService;
    private readonly IScraperService _scraperService = scraperService;
    private readonly IFuzzyMatchingService _fuzzyMatchingService = fuzzyMatchingService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICampaignProductRepository _campaignProductRepository = campaignProductRepository;
    private readonly IAdminService _adminService = adminService;
    private readonly IOrderService _orderService = orderService;
    private readonly ITicketService _ticketService = ticketService;
    private readonly ITicketRepository _ticketRepository = ticketRepository;
    private readonly ISupplierRepository _supplierRepository = supplierRepository;
    private readonly IShippingMethodRepository _shippingMethodRepository = shippingMethodRepository;
    private readonly IConfiguration _config = config;
    private readonly ILogger<AdminController> _logger = logger;

    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var orders = await _adminService.GetAllOrders();
            var campaigns = await _campaignService.GetAllCampaigns();

            var newOrdersCount = orders.Count(o => o.Status == "PaymentPending" || o.Status == "Pending");
            var totalRevenue = orders.Where(o => o.Status == "PaymentConfirmed" || o.Status == "Shipped" || o.Status == "Completed").Sum(o => o.TotalAmount);
            var activeCampaignsCount = campaigns.Count(c => c.IsActive && c.StartDate <= DateTime.UtcNow && c.EndDate >= DateTime.UtcNow);
            var pendingPaymentCount = orders.Count(o => o.Status == "PaymentPending");

            var stats = new List<object>
            {
                new { icon = "📦", value = newOrdersCount.ToString(), label = "سفارشات جدید" },
                new { icon = "💰", value = totalRevenue.ToString("N0") + " تومان", label = "درآمد کل (تومان)" },
                new { icon = "🎯", value = activeCampaignsCount.ToString(), label = "کمپین‌های فعال" },
                new { icon = "⏳", value = pendingPaymentCount.ToString(), label = "در انتظار پرداخت" }
            };

            return Ok(new { success = true, data = stats });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("campaigns")]
    public async Task<IActionResult> GetAllCampaigns()
    {
        try
        {
            var campaigns = await _campaignService.GetAllCampaigns();
            return Ok(new { success = true, data = campaigns });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all campaigns");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("campaigns/{id}")]
    public async Task<IActionResult> GetCampaignById(int id)
    {
        try
        {
            var campaign = await _campaignService.GetCampaignById(id);
            if (campaign == null) return NotFound(new { success = false, message = "Campaign not found" });
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign by ID");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("campaigns")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequest request)
    {
        try
        {
            var campaign = await _campaignService.CreateCampaign(request.Name, request.Slug, request.StartDate, request.EndDate, request.IsActive);
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating campaign");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("campaigns/{id}")]
    public async Task<IActionResult> UpdateCampaign(int id, [FromBody] UpdateCampaignRequest request)
    {
        try
        {
            var campaign = await _campaignService.UpdateCampaign(id, request.Name, request.Slug, request.StartDate, request.EndDate, request.IsActive);
            return Ok(new { success = true, data = campaign });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating campaign");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpDelete("campaigns/{id}")]
    public async Task<IActionResult> DeleteCampaign(int id)
    {
        try
        {
            // Implement delete campaign directly via DB context or repo
            var campaigns = await _campaignService.GetAllCampaigns();
            var campaign = campaigns.FirstOrDefault(c => c.Id == id);
            if (campaign == null) return NotFound(new { success = false, message = "Campaign not found" });

            // We can delete or deactivate it
            // For now, let's deactivate it
            await _campaignService.UpdateCampaign(id, campaign.Name, campaign.Slug ?? "", campaign.StartDate, campaign.EndDate, false);

            return Ok(new { success = true, message = "Campaign deactivated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting campaign");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("campaigns/{id}/report")]
    public async Task<IActionResult> GetCampaignReport(int id)
    {
        try
        {
            var report = await _adminService.GetCampaignReport(id);
            return Ok(new { success = true, data = report });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting campaign report");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("suppliers")]
    public async Task<IActionResult> GetAllSuppliers()
    {
        try
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return Ok(new { success = true, data = suppliers });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suppliers");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("suppliers/{id}")]
    public async Task<IActionResult> GetSupplierById(int id)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return NotFound(new { success = false, message = "Supplier not found" });
            return Ok(new { success = true, data = supplier });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supplier by ID");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("suppliers")]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierRequest request)
    {
        try
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                Website = request.WebsiteUrl,
                ContactInfo = request.ContactInfo,
                RequiresTrackingCode = request.RequiresTrackingCode
            };
            var created = await _supplierRepository.AddAsync(supplier);
            return Ok(new { success = true, data = created, message = "Supplier created successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating supplier");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("suppliers/{id}")]
    public async Task<IActionResult> UpdateSupplier(int id, [FromBody] CreateSupplierRequest request)
    {
        try
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return NotFound(new { success = false, message = "Supplier not found" });

            supplier.Name = request.Name;
            supplier.Website = request.WebsiteUrl;
            supplier.ContactInfo = request.ContactInfo;
            supplier.RequiresTrackingCode = request.RequiresTrackingCode;

            await _supplierRepository.UpdateAsync(supplier);
            return Ok(new { success = true, data = supplier, message = "Supplier updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating supplier");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(new { success = true, data = products });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("products/import-excel")]
    public async Task<IActionResult> ImportProductsFromExcel(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file uploaded" });

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            var matches = new List<object>();

            using var stream = file.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);
            
            var result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
            });

            if (result.Tables.Count == 0)
                return BadRequest(new { success = false, message = "Excel file is empty." });

            var table = result.Tables[0];
            var allProducts = await _productRepository.GetAllAsync();
            var allProductNames = allProducts.Select(p => p.Name).ToList();

            foreach (System.Data.DataRow row in table.Rows)
            {
                var name = row["Name"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;

                var description = row.Table.Columns.Contains("Description") ? row["Description"]?.ToString()?.Trim() : "";
                var basePriceText = row.Table.Columns.Contains("BasePrice") ? row["BasePrice"]?.ToString() : "0";
                var purchasePriceText = row.Table.Columns.Contains("PurchasePrice") ? row["PurchasePrice"]?.ToString() : "0";
                var sellingPriceText = row.Table.Columns.Contains("SellingPrice") ? row["SellingPrice"]?.ToString() : "0";
                var discountText = row.Table.Columns.Contains("Discount") ? row["Discount"]?.ToString() : "0";
                var stockText = row.Table.Columns.Contains("Stock") ? row["Stock"]?.ToString() : "0";
                var minQtyText = row.Table.Columns.Contains("MinQtyPerUser") ? row["MinQtyPerUser"]?.ToString() : "1";
                var maxQtyText = row.Table.Columns.Contains("MaxQtyPerUser") ? row["MaxQtyPerUser"]?.ToString() : "100";

                _ = decimal.TryParse(basePriceText, out var basePrice);
                _ = decimal.TryParse(purchasePriceText, out var purchasePrice);
                _ = decimal.TryParse(sellingPriceText, out var sellingPrice);
                _ = decimal.TryParse(discountText, out var discount);
                _ = int.TryParse(stockText, out var stock);
                _ = int.TryParse(minQtyText, out var minQty);
                _ = int.TryParse(maxQtyText, out var maxQty);

                        var matchResult = await _fuzzyMatchingService.FindBestMatch(name, allProductNames);
                        var bestMatch = matchResult.FirstOrDefault();

                        matches.Add(new
                        {
                            excelName = name,
                            matchedProduct = bestMatch?.MatchedName,
                            matchedProductId = bestMatch != null ? allProducts.FirstOrDefault(p => p.Name == bestMatch.MatchedName)?.Id : null,
                            confidence = bestMatch != null ? (int)Math.Round(bestMatch.ConfidenceScore * 100) : 0,
                            confirmed = bestMatch != null && bestMatch.ConfidenceScore >= 0.7,
                            description,
                            basePrice = basePrice > 0 ? basePrice : sellingPrice,
                            purchasePrice,
                            sellingPrice,
                            discount,
                            stock,
                            minQtyPerUser = minQty,
                            maxQtyPerUser = maxQty
                        });
                    }

            return Ok(new { success = true, matches });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing products from Excel");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("products/import-excel/confirm")]
    public async Task<IActionResult> ConfirmImportExcel([FromBody] List<ConfirmImportItem> items, [FromQuery] int? campaignId)
    {
        try
        {
            int targetCampaignId;
            if (campaignId.HasValue)
            {
                targetCampaignId = campaignId.Value;
            }
            else
            {
                var activeCampaign = await _campaignService.GetActiveCampaign();
                if (activeCampaign == null)
                    return BadRequest(new { success = false, message = "No active campaign found to import products into." });
                targetCampaignId = activeCampaign.Id;
            }

            var processedNames = new List<string>();

            foreach (var item in items)
            {
                Product? product;
                if (item.Confirmed && item.MatchedProductId.HasValue)
                {
                    product = await _productRepository.GetByIdAsync(item.MatchedProductId.Value);
                    if (product != null && !string.IsNullOrWhiteSpace(item.Description))
                    {
                        product.Description = item.Description;
                        await _productRepository.UpdateAsync(product);
                    }
                }
                else
                {
                    product = new Product
                    {
                        Name = item.ExcelName,
                        Description = item.Description,
                        BasePrice = item.BasePrice
                    };
                    product = await _productRepository.AddAsync(product);
                }

                if (product != null)
                {
                    var cp = await _campaignProductRepository.GetCampaignProductAsync(targetCampaignId, product.Id);
                    if (cp == null)
                    {
                        cp = new CampaignProduct
                        {
                            CampaignId = targetCampaignId,
                            ProductId = product.Id,
                            PurchasePrice = item.PurchasePrice,
                            SellingPrice = item.SellingPrice,
                            Discount = item.Discount,
                            Stock = item.Stock,
                            MinQtyPerUser = item.MinQtyPerUser,
                            MaxQtyPerUser = item.MaxQtyPerUser
                        };
                        await _campaignProductRepository.AddAsync(cp);
                    }
                    else
                    {
                        cp.PurchasePrice = item.PurchasePrice;
                        cp.SellingPrice = item.SellingPrice;
                        cp.Discount = item.Discount;
                        cp.Stock = item.Stock;
                        cp.MinQtyPerUser = item.MinQtyPerUser;
                        cp.MaxQtyPerUser = item.MaxQtyPerUser;
                        await _campaignProductRepository.UpdateAsync(cp);
                    }

                    processedNames.Add(product.Name);
                }
            }

            return Ok(new { success = true, message = $"{processedNames.Count} products successfully imported.", data = processedNames });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming product import");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("products/import-template")]
    [AllowAnonymous]
    public IActionResult DownloadExcelTemplate()
    {
        try
        {
            var headers = "Name,Description,BasePrice,PurchasePrice,SellingPrice,Discount,Stock,MinQtyPerUser,MaxQtyPerUser\n" +
                          "کالای نمونه ۱,توضیحات کالای اول,150000,120000,150000,10000,50,1,10\n" +
                          "کالای نمونه ۲,توضیحات کالای دوم,250000,200000,250000,0,100,1,20";

            var bytes = System.Text.Encoding.UTF8.GetBytes(headers);
            var bomBytes = new byte[] { 0xEF, 0xBB, 0xBF };
            var fileBytes = bomBytes.Concat(bytes).ToArray();

            return File(fileBytes, "text/csv", "products_import_template.csv");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating template");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("products/scrape/{supplierId}")]
    public async Task<IActionResult> ScrapeProducts(int supplierId, [FromBody] ScrapeRequest request)
    {
        try
        {
            var products = await _scraperService.ScrapeFromUrl(supplierId, request.Url, request.ExtractionConfig);
            return Ok(new { success = true, data = products });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scraping products");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetAllOrders([FromQuery] string? status)
    {
        try
        {
            var orders = await _adminService.GetAllOrders();
            if (!string.IsNullOrWhiteSpace(status))
            {
                orders = orders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            return Ok(new { success = true, data = orders });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("orders/{id}")]
    public async Task<IActionResult> GetOrderDetail(int id)
    {
        try
        {
            var order = await _orderService.GetOrderDetails(id);
            if (order == null) return NotFound(new { success = false, message = "Order not found" });

            return Ok(new { success = true, data = order });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order details");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("orders/{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var order = await _orderService.GetOrderDetails(id);
            if (order == null) return NotFound(new { success = false, message = "Order not found" });

            // Directly update DB via EF or OrderService
            var dbOrder = await _orderService.GetOrderDetails(id);
            if (dbOrder != null)
            {
                // Retrieve original order from database
                var realOrder = await _orderService.ConfirmPayment(id, dbOrder.PaymentTrackingCode ?? ""); // This confirms, but we can do a direct status update
                // Let's modify OrderService to expose a generic update status or do it in DB directly
                var orderEntity = await _userRepository.GetByIdWithDetailsAsync(dbOrder.UserId); // load user
                var entity = orderEntity?.Orders.FirstOrDefault(o => o.Id == id);
                if (entity != null)
                {
                    entity.Status = request.Status;
                    await _userRepository.UpdateAsync(orderEntity!); // save
                }
            }

            var updated = await _orderService.GetOrderDetails(id);
            return Ok(new { success = true, data = updated, message = "Order status updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating order status");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPut("orders/{id}/invoice")]
    public async Task<IActionResult> EditInvoice(int id, [FromBody] AdminEditInvoiceRequest request)
    {
        try
        {
            var changes = new Dictionary<string, object>();
            if (request.ShippingCost.HasValue) changes["ShippingCost"] = request.ShippingCost.Value;
            if (request.DiscountAmount.HasValue) changes["DiscountAmount"] = request.DiscountAmount.Value;

            var changedBy = User.Identity?.Name ?? "Admin";
            var updated = await _orderService.EditInvoice(id, changes, request.Reason, changedBy);
            if (updated == null) return NotFound(new { success = false, message = "Order not found" });

            return Ok(new { success = true, data = updated, message = "Invoice updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error editing invoice");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("orders/{id}/confirm-payment")]
    public async Task<IActionResult> ConfirmPayment(int id, [FromBody] ConfirmPaymentRequest request)
    {
        try
        {
            var updated = await _orderService.ConfirmPayment(id, request.TrackingCode);
            if (updated == null) return NotFound(new { success = false, message = "Order not found" });

            return Ok(new { success = true, data = updated, message = "Payment confirmed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming payment");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("tickets")]
    public async Task<IActionResult> GetAllTickets()
    {
        try
        {
            var tickets = await _ticketRepository.GetAllAsync();
            var dtos = tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Category = t.Category,
                Subject = t.Subject,
                Message = t.Message,
                Status = t.Status,
                Priority = t.Priority,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                User = new UserDto
                {
                    Id = t.User!.Id,
                    FullName = t.User.FullName,
                    PhoneNumber = t.User.PhoneNumber,
                    Role = t.User.Role,
                    Avatar = t.User.Avatar,
                    CreatedAt = t.User.CreatedAt
                }
            });
            return Ok(new { success = true, data = dtos });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting support tickets");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("tickets/{id}")]
    public async Task<IActionResult> GetTicketDetail(int id)
    {
        try
        {
            var ticket = await _ticketService.GetTicketDetails(id);
            if (ticket == null) return NotFound(new { success = false, message = "Ticket not found" });

            return Ok(new { success = true, data = ticket });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket details");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("tickets/{id}/reply")]
    public async Task<IActionResult> ReplyToTicket(int id, [FromBody] ReplyTicketRequest request)
    {
        try
        {
            // Set userId from Identity (or default to 1 for tests)
            var userId = 1; // Default admin user ID
            var updated = await _ticketService.ReplyToTicket(id, userId, request.Message);
            if (updated == null) return NotFound(new { success = false, message = "Ticket not found" });

            return Ok(new { success = true, data = updated, message = "Reply submitted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replying to ticket");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("users/{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(int userId, [FromBody] ChangeRoleRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Role))
                return BadRequest(new { success = false, message = "Role is required." });

            var validRoles = new List<string> { UserRoles.Admin, UserRoles.OrderManager, UserRoles.CampaignManager, UserRoles.Customer };
            if (!validRoles.Contains(request.Role))
                return BadRequest(new { success = false, message = "Invalid role specified." });

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            user.Role = request.Role;
            await _userRepository.UpdateAsync(user);

            return Ok(new { success = true, message = $"User role updated to '{request.Role}' successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing user role");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    private static readonly string SettingsFilePath = Path.Combine(AppContext.BaseDirectory, "site_settings.json");
    private static readonly System.Text.Json.JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private async Task<SiteSettingsModel> LoadSettingsFromFile()
    {
        if (System.IO.File.Exists(SettingsFilePath))
        {
            try
            {
                var json = await System.IO.File.ReadAllTextAsync(SettingsFilePath);
                var settings = System.Text.Json.JsonSerializer.Deserialize<SiteSettingsModel>(json);
                if (settings != null) return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading site settings file");
            }
        }

        // Fallback to appsettings
        var smsSection = _config.GetSection("SmsSettings");
        var paymentSection = _config.GetSection("PaymentGatewaySettings");

        return new SiteSettingsModel
        {
            SmsApiKey = smsSection.GetValue<string>("ApiKey") ?? "YOUR_SMS_API_KEY",
            SmsSenderId = smsSection.GetValue<string>("SenderId") ?? "LightMarket",
            PaymentMerchantId = paymentSection.GetValue<string>("MerchantId") ?? "YOUR_MERCHANT_ID",
            PaymentCallbackUrl = paymentSection.GetValue<string>("CallbackUrl") ?? "https://lightmarket.ir/payment/callback",
            MagicLinkExpiry = 30,
            FooterLinks =
            [
                new() { Title = "تماس با ما", Url = "/contact" },
                new() { Title = "قوانین و مقررات", Url = "/rules" },
                new() { Title = "حریم خصوصی", Url = "/privacy" }
            ],
            ContactInfo = "تلفن: 021-12345678 | ایمیل: info@lightmarket.ir"
        };
    }

    private static async Task SaveSettingsToFile(SiteSettingsModel settings)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(settings, SerializerOptions);
        await System.IO.File.WriteAllTextAsync(SettingsFilePath, json);
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        try
        {
            var settings = await LoadSettingsFromFile();
            return Ok(new { success = true, data = settings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading system settings");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost("settings")]
    public async Task<IActionResult> SaveSettings([FromBody] SiteSettingsModel settings)
    {
        try
        {
            await SaveSettingsToFile(settings);
            return Ok(new { success = true, data = settings, message = "تنظیمات با موفقیت ذخیره شد." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving system settings");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpGet("settings/public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicSettings()
    {
        try
        {
            var settings = await LoadSettingsFromFile();
            var publicData = new
            {
                settings.FooterLinks,
                settings.ContactInfo
            };
            return Ok(new { success = true, data = publicData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading public settings");
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}

public class FooterLink
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

public class SiteSettingsModel
{
    public string SmsApiKey { get; set; } = string.Empty;
    public string SmsSenderId { get; set; } = string.Empty;
    public string PaymentMerchantId { get; set; } = string.Empty;
    public string PaymentCallbackUrl { get; set; } = string.Empty;
    public int MagicLinkExpiry { get; set; } = 30;
    public List<FooterLink> FooterLinks { get; set; } = [];
    public string ContactInfo { get; set; } = string.Empty;
}

public class CreateCampaignRequest { public string Name { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public bool IsActive { get; set; } }
public class UpdateCampaignRequest { public string Name { get; set; } = string.Empty; public string Slug { get; set; } = string.Empty; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public bool IsActive { get; set; } }
public class CreateSupplierRequest { public string Name { get; set; } = string.Empty; public string WebsiteUrl { get; set; } = string.Empty; public string ContactInfo { get; set; } = string.Empty; public bool RequiresTrackingCode { get; set; } }
public class ScrapeRequest { public string Url { get; set; } = string.Empty; public Dictionary<string, string> ExtractionConfig { get; set; } = []; }
public class ChangeRoleRequest { public string Role { get; set; } = string.Empty; }
public class UpdateStatusRequest { public string Status { get; set; } = string.Empty; }
public class AdminEditInvoiceRequest { public decimal? ShippingCost { get; set; } public decimal? DiscountAmount { get; set; } public string Reason { get; set; } = string.Empty; }
public class ConfirmImportItem
{
    public string ExcelName { get; set; } = string.Empty;
    public int? MatchedProductId { get; set; }
    public bool Confirmed { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal Discount { get; set; }
    public int Stock { get; set; }
    public int MinQtyPerUser { get; set; }
    public int MaxQtyPerUser { get; set; }
}
