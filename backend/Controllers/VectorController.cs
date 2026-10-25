using Microsoft.AspNetCore.Mvc;
using SmartSearch.Infrastructure.Embeddings;
using SmartSearch.Services.Product;
using SmartSearch.Services.Vector;

namespace SmartSearch.Controllers;

[ApiController]
[Route("api/vector")]
public class VectorController : ControllerBase
{
    private readonly IVectorStore _vectorStore;
    private readonly IEmbeddingProvider _embeddings;
    private readonly ProductGenerator _generator;
    private readonly ProductService _products;
    private readonly ILogger<VectorController> _logger;

    public VectorController(
        IVectorStore vectorStore,
        IEmbeddingProvider embeddings,
        ProductGenerator generator,
        ProductService products,
        ILogger<VectorController> logger)
    {
        _vectorStore = vectorStore;
        _embeddings = embeddings;
        _generator = generator;
        _products = products;
        _logger = logger;
    }

    [HttpPost("reindex")]
    public async Task<IActionResult> Reindex()
    {
        try
        {
            var count = await _products.GetCountAsync();
            if (count == 0)
            {
                var generated = _generator.Generate(10000);
                await _products.UpsertAsync(generated);
                count = generated.Count;
            }

            var allProducts = await _products.GetAllAsync(page: 1, pageSize: count);
            if (allProducts.Count == 0)
                return BadRequest(new { message = "No products available to index" });

            await _vectorStore.ClearAsync();

            const int batchSize = 500;
            var totalIndexed = 0;

            for (int i = 0; i < allProducts.Count; i += batchSize)
            {
                var batch = allProducts.Skip(i).Take(batchSize).ToList();
                var texts = batch.Select(p => $"{p.Title} {p.Category} {p.SubCategory1} {p.SubCategory2} {p.Description} {p.Highlights}");
                var vectors = await _embeddings.GetEmbeddingsAsync(texts);

                var indexed = batch.Zip(vectors, (p, v) => (p, v)).ToList();
                await _vectorStore.IndexProductsAsync(indexed);
                totalIndexed += indexed.Count;

                _logger.LogInformation("Indexed batch {BatchEnd}/{Total}", i + batch.Count, allProducts.Count);
            }

            _logger.LogInformation("Reindexed {Count} products", totalIndexed);
            return Ok(new { message = $"Reindexed {totalIndexed} products", count = totalIndexed });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reindex failed");
            return StatusCode(500, new { message = "Indexing error", detail = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Parameter q is required" });

        var vector = await _embeddings.GetEmbeddingAsync(q);
        var results = await _vectorStore.SearchAsync(vector, limit);
        return Ok(new { products = results });
    }

    [HttpGet("health")]
    public async Task<IActionResult> Health()
    {
        var healthy = await _vectorStore.IsHealthyAsync();
        var storeType = _vectorStore.GetType().Name;
        var count = await _vectorStore.GetPointsCountAsync();
        return Ok(new { healthy, storeType, pointsCount = count });
    }
}
