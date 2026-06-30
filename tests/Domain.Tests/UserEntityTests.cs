using Xunit;
using LampEcommerce.Domain.Entities;

namespace Domain.Tests;

public class UserEntityTests
{
    [Fact]
    public void User_CanBeCreated_WithValidPhoneNumber()
    {
        // Arrange
        var phoneNumber = "+1234567890";

        // Act
        var user = new User("John Doe", phoneNumber);

        // Assert
        Assert.NotNull(user);
        Assert.Equal(phoneNumber, user.PhoneNumber);
    }

    [Fact]
    public void User_CannotBeCreated_WithEmptyPhoneNumber()
    {
        // Arrange
        var emptyPhoneNumber = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new User("John Doe", emptyPhoneNumber));
    }
}
