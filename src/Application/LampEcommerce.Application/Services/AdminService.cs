using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Interfaces;

namespace LampEcommerce.Application.Services;

public interface IAdminService
{
    Task<IEnumerable<UserDto>> GetAllUsers();
    Task<IEnumerable<OrderDto>> GetAllOrders();
    Task<CampaignReportDto?> GetCampaignReport(int campaignId);
}

public class AdminService : IAdminService
{
    public Task<IEnumerable<UserDto>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<OrderDto>> GetAllOrders()
    {
        throw new NotImplementedException();
    }

    public Task<CampaignReportDto?> GetCampaignReport(int campaignId)
    {
        throw new NotImplementedException();
    }
}
