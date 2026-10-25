using SmartSearch.Infrastructure.Embeddings;
using SmartSearch.Models;
using SmartSearch.Models.Search;
using SmartSearch.Services.Vector;

namespace SmartSearch.Services.Ai;

public class AiSearchService
{
    private readonly OpenRouterService _openRouter;
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingProvider _embeddings;
    private readonly ILogger<AiSearchService> _logger;

    public AiSearchService(
        OpenRouterService openRouter,
        IVectorStore vectorStore,
        IEmbeddingProvider embeddings,
        ILogger<AiSearchService> logger)
    {
        _openRouter = openRouter;
        _vectorStore = vectorStore;
        _embeddings = embeddings;
        _logger = logger;
    }

    public async Task<AiSearchResponse> SearchAsync(string userQuery, int limit = 500)
    {
        _logger.LogInformation("=== AI SEARCH ===");
        _logger.LogInformation("User query: '{UserQuery}'", userQuery);

        var (analysis, normalizedQuery) = await _openRouter.AnalyzeQueryAsync(userQuery);

        _logger.LogInformation("Analysis - ProductType: {ProductType}, Color: {Color}, Features: {Features}",
            analysis.ProductType, analysis.Color, string.Join(",", analysis.Features));
        _logger.LogInformation("NormalizedQueryEn: '{NormalizedQueryEn}'", analysis.NormalizedQueryEn);

        var searchQuery = !string.IsNullOrEmpty(analysis.NormalizedQueryEn)
            ? analysis.NormalizedQueryEn
            : (!string.IsNullOrEmpty(analysis.NormalizedQueryRu)
                ? analysis.NormalizedQueryRu
                : userQuery);

        _logger.LogInformation("Search query for vector: '{SearchQuery}'", searchQuery);

        var queryVector = await _embeddings.GetEmbeddingAsync(searchQuery);

        var products = await _vectorStore.SearchAsync(queryVector, limit);

        _logger.LogInformation("Found {Count} products", products.Count);

        if (products.Count > 0)
        {
            _logger.LogInformation("First product: {Title}, Images: {Images}",
                products[0].Title, products[0].Images);
        }

        return new AiSearchResponse
        {
            Products = products,
            Analysis = analysis
        };
    }
}
