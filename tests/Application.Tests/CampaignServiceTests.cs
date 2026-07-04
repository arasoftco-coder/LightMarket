using Xunit;
using Moq;
using LampEcommerce.Application.Services;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Tests;

public class CampaignServiceTests
{
    [Fact]
    public async Task GetActiveCampaign_WhenExpired_ThrowsInvalidOperationException()
    {
        // Arrange
        var campaignRepoMock = new Mock<ICampaignRepository>();
        var campaignProductRepoMock = new Mock<ICampaignProductRepository>();
        var userRepoMock = new Mock<IUserRepository>();
        var supplierRepoMock = new Mock<ISupplierRepository>();

        var expiredCampaign = new Campaign
        {
            Id = 1,
            Name = "Expired Campaign",
            IsActive = true,
            IsDefault = true,
            EndDate = DateTime.UtcNow.AddDays(-1) // Expired yesterday
        };

        campaignRepoMock
            .Setup(c => c.GetDefaultActiveCampaignAsync())
            .ReturnsAsync(expiredCampaign);

        var service = new CampaignService(
            campaignRepoMock.Object,
            campaignProductRepoMock.Object,
            userRepoMock.Object,
            supplierRepoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetActiveCampaign());
    }

    [Fact]
    public async Task GetCampaignBySlug_WhenInactive_ThrowsInvalidOperationException()
    {
        // Arrange
        var campaignRepoMock = new Mock<ICampaignRepository>();
        var campaignProductRepoMock = new Mock<ICampaignProductRepository>();
        var userRepoMock = new Mock<IUserRepository>();
        var supplierRepoMock = new Mock<ISupplierRepository>();

        var inactiveCampaign = new Campaign
        {
            Id = 2,
            Name = "Inactive Campaign",
            IsActive = false,
            EndDate = DateTime.UtcNow.AddDays(1)
        };

        campaignRepoMock
            .Setup(c => c.GetBySlugAsync("inactive-slug"))
            .ReturnsAsync(inactiveCampaign);

        var service = new CampaignService(
            campaignRepoMock.Object,
            campaignProductRepoMock.Object,
            userRepoMock.Object,
            supplierRepoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetCampaignBySlug("inactive-slug"));
    }

    [Fact]
    public async Task ValidateCampaignAccess_WhenGeographicRestrictionNotMatched_ReturnsFalse()
    {
        // Arrange
        var campaignRepoMock = new Mock<ICampaignRepository>();
        var campaignProductRepoMock = new Mock<ICampaignProductRepository>();
        var userRepoMock = new Mock<IUserRepository>();
        var supplierRepoMock = new Mock<ISupplierRepository>();

        var campaign = new Campaign
        {
            Id = 1,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            AllowedProvinces = "Tehran, Isfahan"
        };

        var user = new User("John Doe", "09123456789");
        user.Addresses.Add(new Address
        {
            Province = "Shiraz", // Non-matching province
            City = "Shiraz"
        });

        campaignRepoMock
            .Setup(c => c.GetByIdAsync(1))
            .ReturnsAsync(campaign);

        userRepoMock
            .Setup(u => u.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(user);

        var service = new CampaignService(
            campaignRepoMock.Object,
            campaignProductRepoMock.Object,
            userRepoMock.Object,
            supplierRepoMock.Object);

        // Act
        var result = await service.ValidateCampaignAccess(1, 1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCampaignAccess_WhenQuantityLimitExceeded_ReturnsFalse()
    {
        // Arrange
        var campaignRepoMock = new Mock<ICampaignRepository>();
        var campaignProductRepoMock = new Mock<ICampaignProductRepository>();
        var userRepoMock = new Mock<IUserRepository>();
        var supplierRepoMock = new Mock<ISupplierRepository>();

        var campaign = new Campaign
        {
            Id = 1,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(1),
            MaxOrderQty = 5 // User can buy at most 5 items in this campaign
        };

        var user = new User("John Doe", "09123456789");
        var completedOrder = new Order
        {
            CampaignId = 1,
            Status = "Completed"
        };
        completedOrder.OrderItems.Add(new OrderItem
        {
            Quantity = 6 // Already ordered 6 items
        });
        user.Orders.Add(completedOrder);

        campaignRepoMock
            .Setup(c => c.GetByIdAsync(1))
            .ReturnsAsync(campaign);

        userRepoMock
            .Setup(u => u.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(user);

        var service = new CampaignService(
            campaignRepoMock.Object,
            campaignProductRepoMock.Object,
            userRepoMock.Object,
            supplierRepoMock.Object);

        // Act
        var result = await service.ValidateCampaignAccess(1, 1);

        // Assert
        Assert.False(result);
    }
}
