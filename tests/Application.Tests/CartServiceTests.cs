using Xunit;
using Moq;

namespace Application.Tests;

public class CartServiceTests
{
    [Fact]
    public void AddItemToCart_IncreasesTotalCount()
    {
        // Arrange
        var cart = new Cart();
        var item = new CartItem("Product1", 1, 10.0m);

        // Act
        cart.AddItem(item);

        // Assert
        Assert.Equal(1, cart.TotalCount);
    }
}
