using Qdrant.Client;
using Qdrant.Client.Grpc;
using SmartSearch.Models;

namespace SmartSearch.Services.Vector;

public class QdrantVectorStore : IVectorStore
{
    private readonly QdrantClient _client;
    private readonly string _collection;
    private readonly int _vectorSize;
    private readonly ILogger<QdrantVectorStore> _logger;

    public QdrantVectorStore(QdrantClient client, IConfiguration config, ILogger<QdrantVectorStore> logger)
    {
        _client = client;
        _collection = config["Qdrant:CollectionName"] ?? "products";
        _vectorSize = int.Parse(config["Qdrant:VectorSize"] ?? "384");
        _logger = logger;
    }

    public async Task IndexProductsAsync(IEnumerable<(SmartSearch.Models.Product product, float[] vector)> items)
    {
        await EnsureCollectionAsync();

        var points = items.Select(item => new PointStruct
        {
            Id = (ulong)item.product.Id,
            Vectors = item.vector,
            Payload =
            {
                ["title"] = item.product.Title,
                ["description"] = item.product.Description,
                ["category"] = item.product.Category,
                ["sub_category_1"] = item.product.SubCategory1 ?? "",
                ["sub_category_2"] = item.product.SubCategory2 ?? "",
                ["sub_category_3"] = item.product.SubCategory3 ?? "",
                ["price"] = (double)item.product.Price,
                ["currency"] = item.product.Currency ?? "INR",
                ["original_price"] = (double)(item.product.OriginalPrice ?? 0),
                ["discount"] = item.product.Discount ?? "",
                ["f_assured"] = item.product.FAssured,
                ["highlights"] = item.product.Highlights ?? "",
                ["seller"] = item.product.Seller ?? "",
                ["seller_rating"] = item.product.SellerRating ?? 0,
                ["specifications"] = item.product.Specifications ?? "",
                ["formatted_specifications"] = item.product.FormattedSpecifications ?? "",
                ["images"] = item.product.Images ?? "",
                ["return_policy"] = item.product.ReturnPolicy ?? "",
                ["breadcrumbs"] = item.product.Breadcrumbs ?? "",
                ["country_of_origin"] = item.product.CountryOfOrigin ?? "",
                ["generic_name"] = item.product.GenericName ?? "",
                ["manufactured_by"] = item.product.ManufacturedBy ?? "",
                ["avg_ratings"] = item.product.AvgRatings ?? 0,
                ["reviews_count"] = (long)item.product.ReviewsCount,
                ["ratings_count"] = (long)item.product.RatingsCount,
                ["uniq_id"] = item.product.UniqId ?? "",
                ["pid"] = item.product.Pid ?? "",
                ["url"] = item.product.Url ?? "",
                ["formatted_url"] = item.product.FormattedUrl ?? "",
            }
        }).ToList();

        if (points.Count == 0) return;

        const int batchSize = 100;
        for (int i = 0; i < points.Count; i += batchSize)
        {
            var batch = points.Skip(i).Take(batchSize).ToList();
            await _client.UpsertAsync(_collection, batch);
        }

        _logger.LogInformation("Indexed {Count} products in Qdrant collection '{Collection}'", points.Count, _collection);
    }

    public async Task<List<SmartSearch.Models.Product>> SearchAsync(float[] queryVector, int limit = 10)
    {
        var searchResult = await _client.SearchAsync(_collection, queryVector, limit: (ulong)limit);

        var products = searchResult.Select(r => ReconstructProduct(r)).ToList();

        _logger.LogInformation("Search returned {Count} products, first images: {Images}",
            products.Count, products.FirstOrDefault()?.Images);

        return products;
    }

    public async Task<List<(SmartSearch.Models.Product product, float score)>> SearchWithScoreAsync(float[] queryVector, int limit = 10)
    {
        var searchResult = await _client.SearchAsync(_collection, queryVector, limit: (ulong)limit);

        var results = new List<(SmartSearch.Models.Product product, float score)>();

        foreach (var r in searchResult)
        {
            results.Add((ReconstructProduct(r), (float)r.Score));
        }

        return results;
    }

    public async Task ClearAsync()
    {
        var exists = await CollectionExistsAsync();
        if (exists)
        {
            await _client.DeleteCollectionAsync(_collection);
            _logger.LogInformation("Deleted Qdrant collection '{Collection}'", _collection);
        }
    }

    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            await _client.ListCollectionsAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<long> GetPointsCountAsync()
    {
        try
        {
            var collectionInfo = await _client.GetCollectionInfoAsync(_collection);
            return (long)collectionInfo.PointsCount;
        }
        catch
        {
            return 0;
        }
    }

    private async Task EnsureCollectionAsync()
    {
        if (!await CollectionExistsAsync())
        {
            await _client.CreateCollectionAsync(_collection, new VectorParams
            {
                Size = (ulong)_vectorSize,
                Distance = Distance.Cosine
            });
            _logger.LogInformation("Created Qdrant collection '{Collection}' with vector size {Size}", _collection, _vectorSize);
        }
    }

    private async Task<bool> CollectionExistsAsync()
    {
        try
        {
            var collections = await _client.ListCollectionsAsync();
            return collections.Any(c => c == _collection);
        }
        catch
        {
            return false;
        }
    }

    private static SmartSearch.Models.Product ReconstructProduct(ScoredPoint r)
    {
        return new SmartSearch.Models.Product
        {
            Id = (int)r.Id.Num,
            Title = GetPayloadString(r.Payload, "title"),
            Description = GetPayloadString(r.Payload, "description"),
            Category = GetPayloadString(r.Payload, "category"),
            SubCategory1 = GetPayloadString(r.Payload, "sub_category_1"),
            SubCategory2 = GetPayloadString(r.Payload, "sub_category_2"),
            SubCategory3 = GetPayloadString(r.Payload, "sub_category_3"),
            Price = (decimal)GetPayloadDouble(r.Payload, "price"),
            Currency = GetPayloadString(r.Payload, "currency"),
            OriginalPrice = (decimal)GetPayloadDouble(r.Payload, "original_price"),
            Discount = GetPayloadString(r.Payload, "discount"),
            FAssured = GetPayloadBool(r.Payload, "f_assured"),
            Highlights = GetPayloadString(r.Payload, "highlights"),
            Seller = GetPayloadString(r.Payload, "seller"),
            SellerRating = GetPayloadDouble(r.Payload, "seller_rating"),
            Specifications = GetPayloadString(r.Payload, "specifications"),
            FormattedSpecifications = GetPayloadString(r.Payload, "formatted_specifications"),
            Images = GetPayloadString(r.Payload, "images"),
            ReturnPolicy = GetPayloadString(r.Payload, "return_policy"),
            Breadcrumbs = GetPayloadString(r.Payload, "breadcrumbs"),
            CountryOfOrigin = GetPayloadString(r.Payload, "country_of_origin"),
            GenericName = GetPayloadString(r.Payload, "generic_name"),
            ManufacturedBy = GetPayloadString(r.Payload, "manufactured_by"),
            AvgRatings = GetPayloadDouble(r.Payload, "avg_ratings"),
            ReviewsCount = (int)GetPayloadLong(r.Payload, "reviews_count"),
            RatingsCount = (int)GetPayloadLong(r.Payload, "ratings_count"),
            UniqId = GetPayloadString(r.Payload, "uniq_id"),
            Pid = GetPayloadString(r.Payload, "pid"),
            Url = GetPayloadString(r.Payload, "url"),
            FormattedUrl = GetPayloadString(r.Payload, "formatted_url"),
            ScrapedAt = DateTime.UtcNow
        };
    }

    private static string GetPayloadString(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload, string key)
    {
        return payload.TryGetValue(key, out var value) ? value.StringValue : "";
    }

    private static double GetPayloadDouble(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload, string key)
    {
        return payload.TryGetValue(key, out var value) ? value.DoubleValue : 0.0;
    }

    private static long GetPayloadLong(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload, string key)
    {
        return payload.TryGetValue(key, out var value) ? value.IntegerValue : 0;
    }

    private static bool GetPayloadBool(Google.Protobuf.Collections.MapField<string, Qdrant.Client.Grpc.Value> payload, string key)
    {
        return payload.TryGetValue(key, out var value) && value.BoolValue;
    }
}
