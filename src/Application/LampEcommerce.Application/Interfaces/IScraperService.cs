using LampEcommerce.Application.DTOs;
using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Interfaces;

public interface IScraperService
{
    Task<ApiResponse<IEnumerable<ProductDto>>> ScrapeProductsAsync(ScrapeProductsRequest request);
    Task<ApiResponse> UpdatePricesAsync(UpdatePricesRequest request);
}
