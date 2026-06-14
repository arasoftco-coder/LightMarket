using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class ScraperService : IScraperService
{
    public Task<ApiResponse<IEnumerable<ProductDto>>> ScrapeProductsAsync(ScrapeProductsRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse> UpdatePricesAsync(UpdatePricesRequest request)
    {
        throw new NotImplementedException();
    }
}
