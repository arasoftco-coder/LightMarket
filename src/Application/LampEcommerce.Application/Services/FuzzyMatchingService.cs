using LampEcommerce.Application.Models;

namespace LampEcommerce.Application.Services;

public class FuzzyMatchingService
{
    public Task<List<FuzzyMatchResult>> FindBestMatchAsync(string productName, List<string> productList)
    {
        // Normalize strings (remove extra spaces, convert to lowercase)
        var normalizedSearch = NormalizeString(productName);
        
        var results = new List<FuzzyMatchResult>();
        
        foreach (var product in productList)
        {
            var normalizedProduct = NormalizeString(product);
            var similarity = CalculateSimilarity(normalizedSearch, normalizedProduct);
            
            if (similarity >= 0.7) // 70% confidence threshold
            {
                results.Add(new FuzzyMatchResult
                {
                    ProductName = product,
                    ConfidenceScore = similarity
                });
            }
        }
        
        // Sort by confidence score descending and return top 3
        var topMatches = results.OrderByDescending(r => r.ConfidenceScore).Take(3).ToList();
        
        return Task.FromResult(topMatches);
    }
    
    private static string NormalizeString(string input)
    {
        return input.ToLower().Trim().Replace("  ", " ");
    }
    
    /// <summary>
    /// Calculate similarity using Levenshtein distance
    /// </summary>
    private static double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;
            
        if (source == target)
            return 1.0;
            
        int[,] matrix = new int[source.Length + 1, target.Length + 1];
        
        // Initialize first row and column
        for (int i = 0; i <= source.Length; i++)
            matrix[i, 0] = i;
        for (int j = 0; j <= target.Length; j++)
            matrix[0, j] = j;
        
        // Fill the matrix
        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost
                );
            }
        }
        
        int distance = matrix[source.Length, target.Length];
        int maxLen = Math.Max(source.Length, target.Length);
        
        return 1.0 - ((double)distance / maxLen);
    }
}

public class FuzzyMatchResult
{
    public string ProductName { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
}
