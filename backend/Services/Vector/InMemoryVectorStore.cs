using SmartSearch.Models;
using SmartSearch.Models.Search;

namespace SmartSearch.Services.Vector;

public class InMemoryVectorStore : IVectorStore
{
    private readonly List<(SmartSearch.Models.Product product, float[] vector)> _store = new();
    private readonly object _lock = new object();

    public Task IndexProductsAsync(IEnumerable<(SmartSearch.Models.Product product, float[] vector)> items)
    {
        lock (_lock)
        {
            _store.Clear();
            _store.AddRange(items);
        }
        return Task.CompletedTask;
    }

    public Task<List<SmartSearch.Models.Product>> SearchAsync(
        float[] queryVector, int limit = 10, ProductFilter? filter = null)
    {
        List<(SmartSearch.Models.Product product, float[] vector)> snapshot;
        lock (_lock)
        {
            snapshot = _store.ToList();
        }

        var results = snapshot
            .Where(item => MatchesFilter(item.product, filter))
            .Select(item => (item.product, score: CosineSimilarity(queryVector, item.vector)))
            .OrderByDescending(x => x.score)
            .Take(limit)
            .Select(x => x.product)
            .ToList();

        return Task.FromResult(results);
    }

    public Task<List<(SmartSearch.Models.Product product, float score)>> SearchWithScoreAsync(
        float[] queryVector, int limit = 10, ProductFilter? filter = null)
    {
        List<(SmartSearch.Models.Product product, float[] vector)> snapshot;
        lock (_lock)
        {
            snapshot = _store.ToList();
        }

        var results = snapshot
            .Where(item => MatchesFilter(item.product, filter))
            .Select(item => (item.product, score: CosineSimilarity(queryVector, item.vector)))
            .OrderByDescending(x => x.score)
            .Take(limit)
            .Select(x => (x.product, x.score))
            .ToList();

        return Task.FromResult(results);
    }

    public Task<long> GetPointsCountAsync()
    {
        lock (_lock)
        {
            return Task.FromResult((long)_store.Count);
        }
    }

    public Task ClearAsync()
    {
        lock (_lock) _store.Clear();
        return Task.CompletedTask;
    }

    public Task<bool> IsHealthyAsync() => Task.FromResult(true);

    private static bool MatchesFilter(SmartSearch.Models.Product p, ProductFilter? f)
    {
        if (f == null) return true;

        var haystack = string.Join(" | ",
            p.Title,
            p.Description,
            p.Highlights,
            p.FormattedSpecifications,
            p.Specifications,
            p.GenericName,
            p.SubCategory2,
            p.SubCategory3);

        if (!string.IsNullOrWhiteSpace(f.Color) &&
            !haystack.Contains(f.Color, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(f.Gender) &&
            !haystack.Contains(f.Gender, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(f.ProductType) &&
            !haystack.Contains(f.ProductType, StringComparison.OrdinalIgnoreCase))
            return false;

        if (f.PriceMin.HasValue && p.Price < f.PriceMin.Value) return false;
        if (f.PriceMax.HasValue && p.Price > f.PriceMax.Value) return false;

        return true;
    }

    private static float CosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length) return 0f;
        float dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        var denom = MathF.Sqrt(normA) * MathF.Sqrt(normB);
        return denom == 0 ? 0f : dot / denom;
    }
}