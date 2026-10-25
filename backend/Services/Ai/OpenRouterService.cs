using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartSearch.Models.Search;

namespace SmartSearch.Services.Ai;

public class OpenRouterService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<OpenRouterService> _logger;

    public OpenRouterService(IHttpClientFactory httpFactory, IConfiguration config, ILogger<OpenRouterService> logger)
    {
        _httpFactory = httpFactory;
        _config = config;
        _logger = logger;
    }

    public async Task<(AiAnalysis analysis, string normalizedQuery)> AnalyzeQueryAsync(string userQuery)
    {
        var systemPrompt = @"You are an expert assistant for analyzing product search queries in an online clothing store.
The user writes in RUSSIAN — with typos, slang, colloquial speech, dialects, abbreviations, and creative descriptions.
Products in the database are in ENGLISH.

Your task:
1. Understand the MEANING of the query — correct typos, decode slang, colloquial and dialectal words
2. Understand SYNONYMS and creative descriptions. Examples (non-exhaustive):
   — «поросячий цвет» / «цвет поросёнка» = pink (розовый)
   — «волосатое» / «волосатая» = fuzzy / fluffy / furry (пушистое)
   — «мужская кофта» = men's sweater / hoodie
   — «тёплые штаны» = warm pants / winter trousers
   — «шмотки для лета» = summer clothes
   — «чё то красное на ноги» = red shoes / red sneakers
   — «куртяга» = jacket
   — «кеды» = sneakers
   — «мастерка» / «олимпийка» = track jacket / windbreaker
   — «треники» / «трениковые штаны» = sweatpants / track pants
   — «лыжи» (slang for shoes) = shoes / sneakers
   — «шапка-ушанка» = fur hat / trapper hat
   — «варежки» = mittens
   — «колготки» = tights
   — «лифчик» / «бюстье» = bra
   — «трусы» = underwear / panties
   — «сланцы» / «шлёпанцы» = flip-flops / slides
   — «бахилы» = overshoes / galoshes
   — «пуховик» = puffer jacket / down jacket
   — «дублёнка» = sheepskin coat
   — «парка» = parka
   — «бомбер» / «бомбардировочная куртка» = bomber jacket
   — «ветровка» = windbreaker
   — «безрукавка» = vest / gilet
   — «майка» / «футболка-алкоголичка» = tank top / singlet
   — «лоферы» = loafers
   — «ботинки» = boots
   — «сапоги» = boots
   — «туфли» = heels / dress shoes
   — «босоножки» = sandals
   — «рюкзак» = backpack
   — «сумка» = bag
   — «портмоне» / «кошелёк» = wallet
   — «ремень» = belt
   — «галстук» = tie
   — «носки» = socks
   — «пижама» = pajamas
   — «халат» = robe / bathrobe
   — «купальник» = swimsuit
   — «боди» = bodysuit
   — «комбинезон» = jumpsuit / overalls
   — «поло» = polo shirt
   — «рубашка» = shirt
   — «блузка» = blouse
   — «жилет» = vest / waistcoat
   — «пальто» = coat
   — «плащ» = raincoat / trench coat
   — «меховая шапка» = fur hat
   — «косынка» = headscarf / kerchief
   — «снуд» = snood / neck gaiter
   — Color synonyms: «поросячий» = pink, «салатовый» = light green, «болотный» = dark green/khaki, «коралловый» = coral, «бирюзовый» = turquoise, «терракотовый» = terracotta, «бордовый» = burgundy/maroon, «винный» = wine/burgundy, «песочный» = sand/beige, «молочный» = cream/off-white, «шоколадный» = chocolate/brown, «морская волна» = teal, «небесный» = sky blue, «васильковый» = cornflower blue, «сиреневый» = lilac/purple, «фиолетовый» = purple, «оранжевый» = orange, «кирпичный» = brick red, «горчичный» = mustard, «хаки» = khaki, «индиго» = indigo, «изумрудный» = emerald green
   — Texture synonyms: «волосатое» = fuzzy/fluffy/furry, «пушистое» = fluffy, «мягкое» = soft, «шершавое» = textured/rough, «гладкое» = smooth, «колючее» = prickly/itchy, «тёплое» = warm, «лёгкое» = lightweight, «тяжёлое» = heavy
   — Style synonyms: «строгое» = formal, «спортивное» = sporty/athletic, «нарядное» = dressy/elegant, «повседневное» = casual, «офисное» = office/workwear, «на свадьбу» = wedding, «на пляж» = beachwear
   — Fit synonyms: «обтягивающее» = slim-fit, «свободное» = loose/relaxed-fit, «оверсайз» = oversized, «приталенное» = fitted
   — Season synonyms: «на зиму» = winter, «на лето» = summer, «демисезон» = spring/autumn, «межсезонье» = spring/autumn
   — Age/gender: «пацанское» = boys/men's, «девчачье» = girls/women's, «детское» = kids
3. Extract ALL relevant search parameters
4. TRANSLATE the normalized query into ENGLISH for searching the database
5. If the query is vague or creative, still produce the best English translation you can

Respond ONLY with a valid JSON object:

{
  ""productType"": ""product type in ENGLISH (t-shirt, jeans, jacket, dress, sneakers, sweater, coat, skirt, shirt, pants, shorts, hoodie, sweatshirt, boots, shoes, hat, bag, scarf, gloves, vest, tank-top, polo, blouse, jumpsuit, swimsuit, pajamas, robe, socks, underwear, bra, tights, mittens, flip-flops, loafers, sandals, heels, backpack, wallet, belt, tie, windbreaker, bomber, parka, puffer, raincoat, trench-coat, overalls)"",
  ""color"": ""color in ENGLISH (red, blue, black, white, green, yellow, pink, gray, brown, beige, navy, orange, purple, turquoise, coral, burgundy, teal, emerald, khaki, mustard, cream, lilac, sand, wine, brick)"",
  ""size"": ""size (XS, S, M, L, XL, XXL, 42-56)"",
  ""gender"": ""male/female/unisex/kids"",
  ""material"": ""material in ENGLISH (cotton, leather, polyester, wool, denim, cashmere, linen, silk, nylon, fleece, fur, suede, velvet, lace, knit, canvas, rubber)"",
  ""season"": ""season in ENGLISH (winter, summer, spring, autumn, all-season)"",
  ""features"": [""features in ENGLISH (warm, waterproof, hooded, printed, striped, checked, oversized, long, short, slim-fit, relaxed-fit, fuzzy, fluffy, furry, soft, lightweight, heavy, formal, sporty, casual, dressy, fitted, loose)""],
  ""normalizedQueryRu"": ""normalized query in RUSSIAN (corrected, with slang decoded)"",
  ""normalizedQueryEn"": ""translated query in ENGLISH for search — use descriptive keywords separated by spaces""
}
";

        try
        {
            var apiKey = _config["OpenRouter:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("OpenRouter API key not configured, using fallback NLP");
                return FallbackAnalysis(userQuery);
            }

            var apiUrl = _config["OpenRouter:ApiUrl"] ?? "https://openrouter.ai/api/v1";
            var model = _config["OpenRouter:ChatModel"] ?? "google/gemini-3.8-flash";

            var client = _httpFactory.CreateClient("OpenRouter");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Add("HTTP-Referer", _config["OpenRouter:Referer"] ?? "https://smartsearch.local");
            client.DefaultRequestHeaders.Add("X-Title", "SmartSearch");

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userQuery }
                },
                temperature = 0.1,
                max_tokens = 500
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{apiUrl}/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("OpenRouter API returned {StatusCode}: {Error}", response.StatusCode, errorText);
                return FallbackAnalysis(userQuery);
            }

            var responseText = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseText);
            var messageContent = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "";

            _logger.LogInformation("OpenRouter response: {Response}", messageContent);

            var analysis = ParseAnalysisJson(messageContent);

            var normalizedQuery = !string.IsNullOrEmpty(analysis.NormalizedQueryEn)
                ? analysis.NormalizedQueryEn
                : (!string.IsNullOrEmpty(analysis.NormalizedQueryRu)
                    ? analysis.NormalizedQueryRu
                    : userQuery);

            return (analysis, normalizedQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenRouter request failed");
            return FallbackAnalysis(userQuery);
        }
    }

    private AiAnalysis ParseAnalysisJson(string raw)
    {
        try
        {
            var start = raw.IndexOf('{');
            var end = raw.LastIndexOf('}');
            if (start >= 0 && end > start)
                raw = raw[start..(end + 1)];

            _logger.LogInformation("Parsing JSON: {Raw}", raw);

            var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            var features = new List<string>();
            if (root.TryGetProperty("features", out var featEl) && featEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var f in featEl.EnumerateArray())
                    if (f.ValueKind == JsonValueKind.String)
                        features.Add(f.GetString() ?? "");
            }

            return new AiAnalysis
            {
                ProductType = root.TryGetProperty("productType", out var pt) ? pt.GetString() ?? "" : "",
                Color = root.TryGetProperty("color", out var c) ? c.GetString() ?? "" : "",
                Size = root.TryGetProperty("size", out var s) ? s.GetString() ?? "" : "",
                Gender = root.TryGetProperty("gender", out var g) ? g.GetString() ?? "" : "",
                Material = root.TryGetProperty("material", out var m) ? m.GetString() ?? "" : "",
                Season = root.TryGetProperty("season", out var sea) ? sea.GetString() ?? "" : "",
                Features = features,
                NormalizedQueryRu = root.TryGetProperty("normalizedQueryRu", out var nqRu) ? nqRu.GetString() ?? "" : "",
                NormalizedQueryEn = root.TryGetProperty("normalizedQueryEn", out var nqEn) ? nqEn.GetString() ?? "" : "",
                NormalizedQuery = root.TryGetProperty("normalizedQuery", out var nq) ? nq.GetString() ?? "" : ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing analysis JSON: {Raw}", raw);
            return new AiAnalysis();
        }
    }

    private (AiAnalysis, string) FallbackAnalysis(string query)
    {
        var normalized = query.ToLower().Replace('ё', 'е').Trim();
        var analysis = new AiAnalysis
        {
            NormalizedQueryRu = normalized,
            NormalizedQueryEn = query,
            NormalizedQuery = normalized,
            Features = new List<string>()
        };
        return (analysis, normalized);
    }

    private class OpenRouterResponse
    {
        [JsonPropertyName("choices")]
        public List<OpenRouterChoice>? Choices { get; set; }
    }

    private class OpenRouterChoice
    {
        [JsonPropertyName("message")]
        public OpenRouterMessage? Message { get; set; }
    }

    private class OpenRouterMessage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
