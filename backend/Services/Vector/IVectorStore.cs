using SmartSearch.Models;
using SmartSearch.Models.Search;

namespace SmartSearch.Services.Vector;

public interface IVectorStore
{
    Task IndexProductsAsync(IEnumerable<(SmartSearch.Models.Product product, float[] vector)> items);
    Task<List<SmartSearch.Models.Product>> SearchAsync(float[] queryVector, int limit = 10, ProductFilter? filter = null);
    Task<List<(SmartSearch.Models.Product product, float score)>> SearchWithScoreAsync(float[] queryVector, int limit = 10, ProductFilter? filter = null);
    Task ClearAsync();
    Task<bool> IsHealthyAsync();
    Task<long> GetPointsCountAsync();
}