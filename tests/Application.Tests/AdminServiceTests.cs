using Xunit;
using Moq;
using LampEcommerce.Application.Services;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Tests;

public class AdminServiceTests
{
    [Fact]
    public async Task GetAllUsers_ReturnsAllMappedUsers()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var orderRepoMock = new Mock<IOrderRepository>();
        var campaignRepoMock = new Mock<ICampaignRepository>();

        var usersList = new List<User>
        {
            new User("User 1", "09121111111") { Id = 1, Role = "Customer" },
            new User("User 2", "09122222222") { Id = 2, Role = "Admin" }
        };

        userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(usersList);

        var adminService = new AdminService(userRepoMock.Object, orderRepoMock.Object, campaignRepoMock.Object);

        // Act
        var result = await adminService.GetAllUsers();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.FullName == "User 1" && u.Role == "Customer");
        Assert.Contains(result, u => u.FullName == "User 2" && u.Role == "Admin");
    }

    [Fact]
    public async Task GetCampaignReport_CalculatesStatsCorrectly()
    {
        // Arrange
        var userRepoMock = new Mock<IUserRepository>();
        var orderRepoMock = new Mock<IOrderRepository>();
        var campaignRepoMock = new Mock<ICampaignRepository>();

        var campaign = new Campaign
        {
            Id = 1,
            Name = "Summer Campaign",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            IsActive = true
        };

        campaignRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(campaign);

        var ordersList = new List<Order>
        {
            new Order
            {
                Id = 101,
                UserId = 1,
                CampaignId = 1,
                TotalAmount = 150000,
                DiscountAmount = 10000,
                Status = "PaymentConfirmed"
            },
            new Order
            {
                Id = 102,
                UserId = 2,
                CampaignId = 1,
                TotalAmount = 250000,
                DiscountAmount = 20000,
                Status = "Shipped"
            },
            new Order
            {
                Id = 103,
                UserId = 1,
                CampaignId = 1,
                TotalAmount = 100000,
                DiscountAmount = 0,
                Status = "Open" // Open (Cart) should be excluded from report
            },
            new Order
            {
                Id = 104,
                UserId = 3,
                CampaignId = 2, // Different campaign, should be excluded
                TotalAmount = 500000,
                DiscountAmount = 50000,
                Status = "PaymentConfirmed"
            }
        };

        orderRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(ordersList);

        var adminService = new AdminService(userRepoMock.Object, orderRepoMock.Object, campaignRepoMock.Object);

        // Act
        var result = await adminService.GetCampaignReport(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CampaignId);
        Assert.Equal("Summer Campaign", result.CampaignName);
        Assert.Equal(2, result.TotalOrders); // Order 101 and 102
        Assert.Equal(2, result.TotalUsers); // Distinct UserIds (1 and 2)
        Assert.Equal(400000, result.TotalRevenue); // 150000 + 250000
        Assert.Equal(30000, result.TotalDiscount); // 10000 + 20000
    }
}
