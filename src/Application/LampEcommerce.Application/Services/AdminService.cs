using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LampEcommerce.Application.Services;

public interface IAdminService
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<IEnumerable<OrderDto>> GetAllOrders();
    Task<CampaignReportDto?> GetCampaignReport(int campaignId);
}

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICampaignRepository _campaignRepository;

    public AdminService(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        ICampaignRepository campaignRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _campaignRepository = campaignRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Role = u.Role,
            Avatar = u.Avatar,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<IEnumerable<OrderDto>> GetAllOrders()
    {
        var orders = await _orderRepository.GetAllWithDetailsAsync();
        return orders.Select(o => new OrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            CampaignId = o.CampaignId,
            TotalAmount = o.TotalAmount,
            DiscountAmount = o.DiscountAmount,
            TaxAmount = o.TaxAmount,
            ShippingCost = o.ShippingCost,
            ShippingMethod = o.ShippingMethod,
            PaymentMethod = o.PaymentMethod,
            PaymentTrackingCode = o.PaymentTrackingCode,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            User = new UserDto
            {
                Id = o.User.Id,
                FullName = o.User.FullName,
                PhoneNumber = o.User.PhoneNumber,
                Role = o.User.Role,
                Avatar = o.User.Avatar,
                CreatedAt = o.User.CreatedAt
            },
            Campaign = new CampaignDto
            {
                Id = o.Campaign.Id,
                Name = o.Campaign.Name,
                Slug = o.Campaign.Slug,
                SupplierId = o.Campaign.SupplierId,
                StartDate = o.Campaign.StartDate,
                EndDate = o.Campaign.EndDate,
                IsActive = o.Campaign.IsActive,
                IsDefault = o.Campaign.IsDefault
            },
            OrderItems = o.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                OrderId = oi.OrderId,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                Product = oi.Product == null ? null : new ProductDto
                {
                    Id = oi.Product.Id,
                    Name = oi.Product.Name,
                    ImageUrl = oi.Product.ImageUrl,
                    Description = oi.Product.Description,
                    BasePrice = oi.Product.BasePrice
                }
            }).ToList()
        });
    }

    public async Task<CampaignReportDto?> GetCampaignReport(int campaignId)
    {
        var campaign = await _campaignRepository.GetByIdAsync(campaignId);
        if (campaign == null) return null;

        var allOrders = await _orderRepository.GetAllAsync();
        var campaignOrders = allOrders.Where(o => o.CampaignId == campaignId && o.Status != "Open" && o.Status != "Cancelled").ToList();

        var totalRevenue = campaignOrders.Sum(o => o.TotalAmount);
        var totalDiscount = campaignOrders.Sum(o => o.DiscountAmount);
        var totalOrdersCount = campaignOrders.Count;
        var totalUsersCount = campaignOrders.Select(o => o.UserId).Distinct().Count();

        return new CampaignReportDto
        {
            CampaignId = campaign.Id,
            CampaignName = campaign.Name,
            TotalOrders = totalOrdersCount,
            TotalUsers = totalUsersCount,
            TotalRevenue = totalRevenue,
            TotalDiscount = totalDiscount,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate
        };
    }
}
