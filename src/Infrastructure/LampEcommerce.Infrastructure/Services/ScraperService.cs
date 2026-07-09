using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using LampEcommerce.Application.Interfaces;
using LampEcommerce.Domain.Entities;

namespace LampEcommerce.Infrastructure.Services;

public class ScraperService : IScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICampaignProductRepository _campaignProductRepository;
    private readonly IFuzzyMatchingService _fuzzyMatchingService;

    public ScraperService(
        IHttpClientFactory httpClientFactory,
        ICampaignRepository campaignRepository,
        ICampaignProductRepository campaignProductRepository,
        IFuzzyMatchingService fuzzyMatchingService)
    {
        _httpClientFactory = httpClientFactory;
        _campaignRepository = campaignRepository;
        _campaignProductRepository = campaignProductRepository;
        _fuzzyMatchingService = fuzzyMatchingService;
    }

    public async Task<List<ProductScrapeResult>> ScrapeFromUrl(int supplierId, string url, Dictionary<string, string> extractionConfig)
    {
        var results = new List<ProductScrapeResult>();
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            
            var html = await client.GetStringAsync(url);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var containerSelector = extractionConfig.GetValueOrDefault("ContainerSelector", ".product-item");
            var nameSelector = extractionConfig.GetValueOrDefault("NameSelector", ".product-title");
            var priceSelector = extractionConfig.GetValueOrDefault("PriceSelector", ".product-price");
            var stockSelector = extractionConfig.GetValueOrDefault("StockSelector", ".product-stock");

            var nodes = GetNodesBySelector(doc.DocumentNode, containerSelector);
            foreach (var node in nodes)
            {
                var name = GetTextBySelector(node, nameSelector);
                var priceText = GetTextBySelector(node, priceSelector);
                var stockText = GetTextBySelector(node, stockSelector);

                if (!string.IsNullOrWhiteSpace(name))
                {
                    results.Add(new ProductScrapeResult
                    {
                        Name = name,
                        Price = ParsePrice(priceText),
                        Stock = ParseStock(stockText)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"خطا در اسکرپ وب‌سایت: {ex.Message}", ex);
        }

        return results;
    }

    public async Task<bool> UpdateCampaignPrices(int campaignId)
    {
        var campaign = await _campaignRepository.GetByIdAsync(campaignId);
        if (campaign == null || campaign.Supplier == null || string.IsNullOrWhiteSpace(campaign.Supplier.Website))
        {
            return false;
        }

        var config = new Dictionary<string, string>
        {
            { "ContainerSelector", ".product-item" },
            { "NameSelector", ".product-title" },
            { "PriceSelector", ".product-price" },
            { "StockSelector", ".product-stock" }
        };

        var scrapedProducts = await ScrapeFromUrl(campaign.SupplierId, campaign.Supplier.Website, config);
        if (!scrapedProducts.Any()) return false;

        var campaignProducts = await _campaignProductRepository.GetProductsByCampaignIdAsync(campaignId);
        var productNames = campaignProducts.Select(cp => cp.Product.Name).ToList();

        foreach (var scraped in scrapedProducts)
        {
            var matchResult = await _fuzzyMatchingService.FindBestMatch(scraped.Name, productNames);
            var bestMatch = matchResult.FirstOrDefault();

            if (bestMatch != null && bestMatch.ConfidenceScore >= 0.7)
            {
                var cp = campaignProducts.FirstOrDefault(x => x.Product.Name == bestMatch.MatchedName);
                if (cp != null)
                {
                    cp.PurchasePrice = scraped.Price;
                    cp.SellingPrice = scraped.Price * 1.2m;
                    cp.Stock = scraped.Stock;
                    await _campaignProductRepository.UpdateAsync(cp);
                }
            }
        }

        return true;
    }

    private static IEnumerable<HtmlNode> GetNodesBySelector(HtmlNode root, string selector)
    {
        if (string.IsNullOrWhiteSpace(selector)) return Enumerable.Empty<HtmlNode>();

        if (selector.StartsWith("/") || selector.StartsWith("./") || selector.StartsWith(".//") || selector.StartsWith("("))
        {
            var nodes = root.SelectNodes(selector);
            return nodes ?? Enumerable.Empty<HtmlNode>();
        }
        else
        {
            if (selector.StartsWith("."))
            {
                var className = selector.Substring(1);
                return root.Descendants().Where(n => n.GetAttributeValue("class", "").Split(' ').Contains(className));
            }
            else if (selector.StartsWith("#"))
            {
                var id = selector.Substring(1);
                return root.Descendants().Where(n => n.GetAttributeValue("id", "") == id);
            }
            else
            {
                return root.Descendants(selector);
            }
        }
    }

    private static string GetTextBySelector(HtmlNode root, string selector)
    {
        if (string.IsNullOrWhiteSpace(selector)) return string.Empty;

        HtmlNode? node = null;
        if (selector.StartsWith("/") || selector.StartsWith("./") || selector.StartsWith(".//") || selector.StartsWith("("))
        {
            node = root.SelectSingleNode(selector);
        }
        else
        {
            if (selector.StartsWith("."))
            {
                var className = selector.Substring(1);
                node = root.Descendants().FirstOrDefault(n => n.GetAttributeValue("class", "").Split(' ').Contains(className));
            }
            else if (selector.StartsWith("#"))
            {
                var id = selector.Substring(1);
                node = root.Descendants().FirstOrDefault(n => n.GetAttributeValue("id", "") == id);
            }
            else
            {
                node = root.Descendants(selector).FirstOrDefault();
            }
        }

        return node?.InnerText?.Trim() ?? string.Empty;
    }

    private static decimal ParsePrice(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        var clean = new string(text.Where(c => char.IsDigit(c) || c == '.').ToArray());
        if (decimal.TryParse(clean, out var price)) return price;
        return 0;
    }

    private static int ParseStock(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;
        var clean = new string(text.Where(char.IsDigit).ToArray());
        if (int.TryParse(clean, out var stock)) return stock;
        return 0;
    }
}
