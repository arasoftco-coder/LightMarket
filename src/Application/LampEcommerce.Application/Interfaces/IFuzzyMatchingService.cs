namespace LampEcommerce.Application.Interfaces;

public interface IFuzzyMatchingService
{
    Task<List<FuzzyMatchResult>> FindBestMatch(string productName, List<string> productList);
}

public class FuzzyMatchResult
{
    public string MatchedName { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
}
