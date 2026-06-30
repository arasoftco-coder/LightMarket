using Xunit;
using Moq;
using LampEcommerce.Application.Services;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Application.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task GenerateOTP_WhenSmsSucceeds_DoesNotTriggerMessengerFallback()
    {
        // Arrange
        var cacheMock = new Mock<IMemoryCache>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        var smsServiceMock = new Mock<ISmsService>();
        smsServiceMock
            .Setup(s => s.SendTemporaryPassword(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var userRepositoryMock = new Mock<IUserRepository>();
        var botServiceMock = new Mock<IMessengerBotService>();

        var authService = new AuthService(
            cacheMock.Object, 
            smsServiceMock.Object, 
            userRepositoryMock.Object, 
            botServiceMock.Object);

        var phoneNumber = "09123456789";

        // Act
        var result = await authService.GenerateOTP(phoneNumber);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(phoneNumber, result.PhoneNumber);
        
        smsServiceMock.Verify(s => s.SendTemporaryPassword(phoneNumber, It.IsAny<string>()), Times.Once);
        botServiceMock.Verify(b => b.SendOtpFallbackAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GenerateOTP_WhenSmsFails_TriggersMessengerFallback()
    {
        // Arrange
        var cacheMock = new Mock<IMemoryCache>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        var smsServiceMock = new Mock<ISmsService>();
        smsServiceMock
            .Setup(s => s.SendTemporaryPassword(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var userRepositoryMock = new Mock<IUserRepository>();
        var botServiceMock = new Mock<IMessengerBotService>();
        botServiceMock
            .Setup(b => b.SendOtpFallbackAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var authService = new AuthService(
            cacheMock.Object, 
            smsServiceMock.Object, 
            userRepositoryMock.Object, 
            botServiceMock.Object);

        var phoneNumber = "09123456789";

        // Act
        var result = await authService.GenerateOTP(phoneNumber);

        // Assert
        Assert.NotNull(result);
        
        smsServiceMock.Verify(s => s.SendTemporaryPassword(phoneNumber, It.IsAny<string>()), Times.Once);
        botServiceMock.Verify(b => b.SendOtpFallbackAsync(phoneNumber, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GenerateOTP_WhenBaleRequestedAndNotConnected_ThrowsArgumentException()
    {
        // Arrange
        var cacheMock = new Mock<IMemoryCache>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        var smsServiceMock = new Mock<ISmsService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(u => u.GetByPhoneNumberAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var botServiceMock = new Mock<IMessengerBotService>();

        var authService = new AuthService(
            cacheMock.Object, 
            smsServiceMock.Object, 
            userRepositoryMock.Object, 
            botServiceMock.Object);

        var phoneNumber = "09123456789";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => authService.GenerateOTP(phoneNumber, "Bale"));
    }

    [Fact]
    public async Task GenerateOTP_WhenBaleRequestedAndConnected_SendsViaBale()
    {
        // Arrange
        var cacheMock = new Mock<IMemoryCache>();
        var cacheEntryMock = new Mock<ICacheEntry>();
        cacheMock
            .Setup(m => m.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntryMock.Object);

        var smsServiceMock = new Mock<ISmsService>();
        var userRepositoryMock = new Mock<IUserRepository>();
        
        var user = new User("Test User", "09123456789") { BaleChatId = "123456" };
        userRepositoryMock
            .Setup(u => u.GetByPhoneNumberAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var botServiceMock = new Mock<IMessengerBotService>();
        botServiceMock
            .Setup(b => b.SendMessageAsync("Bale", "123456", It.IsAny<string>()))
            .ReturnsAsync(true);

        var authService = new AuthService(
            cacheMock.Object, 
            smsServiceMock.Object, 
            userRepositoryMock.Object, 
            botServiceMock.Object);

        var phoneNumber = "09123456789";

        // Act
        var result = await authService.GenerateOTP(phoneNumber, "Bale");

        // Assert
        Assert.NotNull(result);
        botServiceMock.Verify(b => b.SendMessageAsync("Bale", "123456", It.IsAny<string>()), Times.Once);
    }
}
