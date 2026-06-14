namespace LampEcommerce.Application.Interfaces;

public interface IScraperService
{
    Task<List<ProductScrapeResult>> ScrapeFromUrl(int supplierId, string url, Dictionary<string, string> extractionConfig);
    Task<bool> UpdateCampaignPrices(int campaignId);
}

public class ProductScrapeResult
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}
