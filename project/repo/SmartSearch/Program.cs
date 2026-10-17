using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Qdrant.Client;
using SmartSearch.Data;
using SmartSearch.Infrastructure.Embeddings;
using SmartSearch.Middleware;
using SmartSearch.Services.Ai;
using SmartSearch.Services.Auth;
using SmartSearch.Services.Product;
using SmartSearch.Services.Vector;

var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envPath))
    Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
    }));

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "SmartSearch",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "SmartSearchUsers",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var corsOrigins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueFrontend", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ── Qdrant (with in-memory fallback) ─────────────────────────────────────────
builder.Services.AddSingleton<IVectorStore>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<QdrantVectorStore>>();
    var config = sp.GetRequiredService<IConfiguration>();
    var qdrantUrl = Environment.GetEnvironmentVariable("QDRANT_URL")
        ?? config["Qdrant:Url"]
        ?? "http://localhost:6334";

    try
    {
        var uri = new Uri(qdrantUrl);
        var client = new QdrantClient(host: uri.Host, port: uri.Port, https: uri.Scheme == "https");
        client.ListCollectionsAsync().GetAwaiter().GetResult();

        logger.LogInformation("Connected to Qdrant at {Url}", qdrantUrl);
        return new QdrantVectorStore(client, config, logger);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Qdrant unavailable, using in-memory store");
        return new InMemoryVectorStore();
    }
});

// ── Embedding Provider (hash-based, deterministic, no external API needed) ────
builder.Services.AddSingleton<IEmbeddingProvider, HashEmbeddingProvider>();

// ── OpenRouter HTTP Client ───────────────────────────────────────────────────
builder.Services.AddHttpClient("OpenRouter");

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddSingleton<OpenRouterService>();
builder.Services.AddScoped<AiSearchService>();
builder.Services.AddSingleton<ProductGenerator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("VueFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
        app.Logger.LogInformation("Database migrations applied");

        // Auto-generate and load products if database is empty
        if (!db.Products.Any())
        {
            var generator = scope.ServiceProvider.GetRequiredService<ProductGenerator>();
            var products = generator.Generate(10000);
            app.Logger.LogInformation("Auto-generating {Count} products...", products.Count);

            // Batch insert in chunks of 500
            foreach (var chunk in products.Chunk(500))
            {
                db.Products.AddRange(chunk);
                db.SaveChanges();
            }
            app.Logger.LogInformation("Products loaded into database");
        }

        // Auto-reindex Qdrant if the vector store is empty
        var vectorStore = scope.ServiceProvider.GetRequiredService<IVectorStore>();
        var embeddings = scope.ServiceProvider.GetRequiredService<IEmbeddingProvider>();
        var pointsCount = await vectorStore.GetPointsCountAsync();
        if (pointsCount == 0)
        {
            app.Logger.LogInformation("Qdrant is empty, auto-indexing products...");
            var productCount = await db.Products.CountAsync();
            var allProducts = await db.Products.OrderBy(p => p.Id).ToListAsync();

            await vectorStore.ClearAsync();
            const int batchSize = 500;
            for (int i = 0; i < allProducts.Count; i += batchSize)
            {
                var batch = allProducts.Skip(i).Take(batchSize).ToList();
                var texts = batch.Select(p => $"{p.Title} {p.Category} {p.SubCategory1} {p.SubCategory2} {p.Description} {p.Highlights}");
                var vectors = await embeddings.GetEmbeddingsAsync(texts);
                var indexed = batch.Zip(vectors, (p, v) => (p, v)).ToList();
                await vectorStore.IndexProductsAsync(indexed);
                app.Logger.LogInformation("Indexed batch {BatchEnd}/{Total}", i + batch.Count, allProducts.Count);
            }
            app.Logger.LogInformation("Qdrant indexing complete");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning("Migration/startup failed: {Message}", ex.Message);
    }
}

app.Run();
