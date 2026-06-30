using Xunit;
using LampEcommerce.Application.DTOs;

namespace Application.Tests;

public class CartServiceTests
{
    [Fact]
    public void AddItemToCart_IncreasesTotalCount()
    {
        // Arrange
        var cart = new CartDto();
        var item = new CartItemDto
        {
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 10.0m
        };

        // Act
        cart.Items.Add(item);

        // Assert
        Assert.Equal(2, cart.TotalCount);
        Assert.Equal(20.0m, cart.TotalAmount);
    }
}
