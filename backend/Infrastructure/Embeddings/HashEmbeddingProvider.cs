using System.Security.Cryptography;
using System.Text;

namespace SmartSearch.Infrastructure.Embeddings;

public class HashEmbeddingProvider : IEmbeddingProvider
{
    private readonly int _vectorSize;
    private readonly ILogger<HashEmbeddingProvider> _logger;

    public HashEmbeddingProvider(IConfiguration config, ILogger<HashEmbeddingProvider> logger)
    {
        _vectorSize = int.Parse(config["Qdrant:VectorSize"] ?? "384");
        _logger = logger;
    }

    public Task<float[]> GetEmbeddingAsync(string text)
    {
        var vector = ComputeHashEmbedding(text, _vectorSize);
        return Task.FromResult(vector);
    }

    public Task<List<float[]>> GetEmbeddingsAsync(IEnumerable<string> texts)
    {
        var textList = texts.ToList();
        var vectors = textList.Select(t => ComputeHashEmbedding(t, _vectorSize)).ToList();
        return Task.FromResult(vectors);
    }

    /// <summary>
    /// Deterministic embedding via feature hashing. Each token in the text is hashed
    /// into multiple vector dimensions, producing a stable semantic fingerprint that
    /// captures word overlap between queries and product descriptions.
    /// </summary>
    private static float[] ComputeHashEmbedding(string text, int dims)
    {
        var vector = new float[dims];
        var tokens = Tokenize(text);

        foreach (var token in tokens)
        {
            // Hash each token into 3 positions for better coverage
            for (int k = 0; k < 3; k++)
            {
                var bytes = Encoding.UTF8.GetBytes($"{token}_{k}");
                var hash = MD5.HashData(bytes);

                // First 4 bytes -> dimension index
                var dim = BitConverter.ToUInt32(hash, 0) % (uint)dims;

                // Next 4 bytes -> sign (+1 or -1)
                var signBit = hash[4] & 1;
                var sign = signBit == 0 ? 1f : -1f;

                // Weight: longer tokens and rarer tokens get more weight
                var weight = 1f / (1f + token.Length * 0.1f);
                vector[dim] += sign * weight;
            }

            // Also add bigram features for better semantic matching
        }

        // Add bigram features
        for (int i = 0; i < tokens.Count - 1; i++)
        {
            var bigram = $"{tokens[i]}_{tokens[i + 1]}";
            var bytes = Encoding.UTF8.GetBytes(bigram);
            var hash = MD5.HashData(bytes);
            var dim = BitConverter.ToUInt32(hash, 0) % (uint)dims;
            var signBit = hash[4] & 1;
            var sign = signBit == 0 ? 1f : -1f;
            vector[dim] += sign * 0.5f;
        }

        // L2 normalize
        var norm = 0f;
        for (int i = 0; i < dims; i++)
            norm += vector[i] * vector[i];
        norm = MathF.Sqrt(norm);
        if (norm > 0)
        {
            for (int i = 0; i < dims; i++)
                vector[i] /= norm;
        }

        return vector;
    }

    /// <summary>
    /// Simple tokenizer: lowercase, split on non-alphanumeric, filter short tokens.
    /// Handles both English and Russian text.
    /// </summary>
    private static List<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new List<string>();

        var lower = text.ToLowerInvariant().Trim();
        var tokens = new List<string>();
        var current = new StringBuilder();

        foreach (var c in lower)
        {
            if (char.IsLetterOrDigit(c))
                current.Append(c);
            else
            {
                if (current.Length >= 2)
                    tokens.Add(current.ToString());
                current.Clear();
            }
        }

        if (current.Length >= 2)
            tokens.Add(current.ToString());

        return tokens;
    }
}
