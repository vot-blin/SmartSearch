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
        var systemPrompt = @"You are a query normalizer for an online clothing store. Products in the database are in ENGLISH. Users write queries in RUSSIAN, ENGLISH, or a mix, with typos, slang, colloquial words, dialects, abbreviations and creative descriptions.

    RULES:
    1. Detect the input language. If the query is in English, translate it into natural Russian for normalizedQueryRu and clean it up for normalizedQueryEn. Never leave normalizedQueryRu empty.
    2. Correct typos and decode slang using the synonym table below (non-exhaustive — use your own knowledge for anything not listed).
    3. Extract ALL parameters present in the query. NEVER invent values that are not there. If a parameter is absent, output null (for features — an empty array).
    4. If the query contains several products, put the MAIN product (head noun / most specific) in productType; mention the rest in features and in the normalized queries.
    5. Map values to the canonical vocabulary of the store (product types, colors, materials as listed below). Keep sizes as written (S, M, L, 42-56 etc.).
    6. If the query is vague or creative, still produce your best guess — never refuse.
    7. Output ONLY a raw JSON object. No markdown code fences, no explanations, no comments, no trailing text.

    JSON SCHEMA (values may be string or null; ""features"" is an array of strings):
    {
      ""productType"": string|null,   // canonical English: t-shirt, jeans, jacket, dress, sneakers, sweater, coat, skirt, shirt, pants, shorts, hoodie, sweatshirt, boots, shoes, hat, bag, scarf, gloves, vest, tank-top, polo, blouse, jumpsuit, swimsuit, pajamas, robe, socks, underwear, bra, tights, mittens, flip-flops, loafers, sandals, heels, backpack, wallet, belt, tie, windbreaker, bomber, parka, puffer, raincoat, trench-coat, overalls
      ""color"": string|null,           // red, blue, black, white, green, yellow, pink, gray, brown, beige, navy, orange, purple, turquoise, coral, burgundy, teal, emerald, khaki, mustard, cream, lilac, sand, wine, brick, maroon, olive, charcoal
      ""size"": string|null,            // XS, S, M, L, XL, XXL, 42-56
      ""gender"": ""male""|""female""|""unisex""|""kids""|null,
      ""material"": string|null,        // cotton, leather, polyester, wool, denim, cashmere, linen, silk, nylon, fleece, fur, suede, velvet, lace, knit, canvas, rubber
      ""season"": string|null,          // winter, summer, spring, autumn, all-season
      ""features"": [string],           // warm, waterproof, hooded, printed, striped, checked, oversized, long, short, slim-fit, relaxed-fit, fuzzy, fluffy, furry, soft, lightweight, heavy, formal, sporty, casual, dressy, fitted, loose
      ""normalizedQueryRu"": string,    // corrected, slang decoded, natural Russian
      ""normalizedQueryEn"": string     // descriptive English keywords separated by spaces, for DB search
    }

    SYNONYM TABLE (non-exhaustive):
    Product types:
    - «куртяга» = jacket; «кеды» = sneakers; «мастерка»/«олимпийка» = track jacket / windbreaker; «треники» = sweatpants / track pants; «лыжи» (slang for shoes) = shoes / sneakers; «шапка-ушанка» = fur hat / trapper hat; «варежки» = mittens; «колготки» = tights; «лифчик»/«бюстье» = bra; «трусы» = underwear / panties; «сланцы»/«шлёпанцы» = flip-flops / slides; «бахилы» = overshoes / galoshes; «пуховик» = puffer jacket / down jacket; «дублёнка» = sheepskin coat; «парка» = parka; «бомбер» = bomber jacket; «ветровка» = windbreaker; «безрукавка» = vest / gilet; «майка»/«футболка-алкоголичка» = tank top; «лоферы» = loafers; «ботинки»/«сапоги» = boots; «туфли» = heels / dress shoes; «босоножки» = sandals; «рюкзак» = backpack; «сумка» = bag; «портмоне»/«кошелёк» = wallet; «ремень» = belt; «галстук» = tie; «носки» = socks; «пижама» = pajamas; «халат» = robe / bathrobe; «купальник» = swimsuit; «боди» = bodysuit; «комбинезон» = jumpsuit / overalls; «поло» = polo shirt; «рубашка» = shirt; «блузка» = blouse; «жилет» = vest / waistcoat; «пальто» = coat; «плащ» = raincoat / trench coat; «меховая шапка» = fur hat; «косынка» = headscarf; «снуд» = snood / neck gaiter; «мужская кофта» = men's sweater / hoodie; «тёплые штаны» = warm pants / winter trousers; «шмотки для лета» = summer clothes; «чё то красное на ноги» = red shoes / sneakers
    Colors:
    - «поросячий» = pink; «салатовый» = light green; «болотный» = dark green / khaki; «коралловый» = coral; «бирюзовый» = turquoise; «терракотовый» = terracotta; «бордовый»/«винный» = burgundy / wine; «песочный» = sand / beige; «молочный» = cream / off-white; «шоколадный» = chocolate / brown; «морская волна» = teal; «небесный» = sky blue; «васильковый» = cornflower blue; «сиреневый» = lilac / purple; «оранжевый» = orange; «кирпичный» = brick red; «горчичный» = mustard; «хаки» = khaki; «индиго» = indigo; «изумрудный» = emerald green
    Textures / features:
    - «волосатое» = fuzzy / fluffy / furry; «пушистое» = fluffy; «мягкое» = soft; «шершавое» = textured / rough; «гладкое» = smooth; «колючее» = prickly / itchy; «тёплое» = warm; «лёгкое» = lightweight; «тяжёлое» = heavy
    Styles:
    - «строгое» = formal; «спортивное» = sporty / athletic; «нарядное» = dressy / elegant; «повседневное» = casual; «офисное» = office / workwear; «на свадьбу» = wedding; «на пляж» = beachwear
    Fit:
    - «обтягивающее» = slim-fit; «свободное» = loose / relaxed-fit; «оверсайз» = oversized; «приталенное» = fitted
    Seasons:
    - «на зиму» = winter; «на лето» = summer; «демисезон»/«межсезонье» = spring / autumn
    Age / gender:
    - «пацанское» = boys / men's; «девчачье» = girls / women's; «детское» = kids

    FEW-SHOT EXAMPLES:

    User: мужская куртяга поросячьего цвета на зиму, размер 52
    Assistant:
    {
      ""productType"": ""jacket"",
      ""color"": ""pink"",
      ""size"": ""52"",
      ""gender"": ""male"",
      ""material"": null,
      ""season"": ""winter"",
      ""features"": [""warm""],
      ""normalizedQueryRu"": ""мужская розовая зимняя куртка, размер 52"",
      ""normalizedQueryEn"": ""mens pink winter jacket size 52""
    }

    User: oversized black fleece hoodie
    Assistant:
    {
      ""productType"": ""hoodie"",
      ""color"": ""black"",
      ""size"": null,
      ""gender"": null,
      ""material"": ""fleece"",
      ""season"": null,
      ""features"": [""oversized""],
      ""normalizedQueryRu"": ""чёрное оверсайз худи из флиса"",
      ""normalizedQueryEn"": ""oversized black fleece hoodie""
    }

    User: хочу кеды и мастерку под джинсы, чёто молодёжное
    Assistant:
    {
      ""productType"": ""sneakers"",
      ""color"": null,
      ""size"": null,
      ""gender"": null,
      ""material"": null,
      ""season"": null,
      ""features"": [""casual"", ""track jacket""],
      ""normalizedQueryRu"": ""кеды и спортивная куртка (мастерка) под джинсы, молодёжный повседневный стиль"",
      ""normalizedQueryEn"": ""sneakers and track jacket casual youth style for jeans""
    }

    User: чёто волосатое поросячьего цвета
    Assistant:
    {
      ""productType"": null,
      ""color"": ""pink"",
      ""size"": null,
      ""gender"": null,
      ""material"": null,
      ""season"": null,
      ""features"": [""fuzzy"", ""fluffy""],
      ""normalizedQueryRu"": ""что-то пушистое розового цвета"",
      ""normalizedQueryEn"": ""pink fuzzy fluffy clothing""
    }";

        try
        {
            var apiKey = _config["DeepSeek:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("DeepSeek API key not configured, using fallback NLP");
                return FallbackAnalysis(userQuery);
            }

            var apiUrl = _config["DeepSeek:ApiUrl"] ?? "https://api.deepseek.com";
            var model = _config["DeepSeek:ChatModel"] ?? "deepseek-chat";

            var client = _httpFactory.CreateClient("DeepSeek");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            

            var requestBody = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userQuery }
                },
                temperature = 0.1,
                max_tokens = 200,
                response_format = new { type = "json_object" }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{apiUrl}/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("DeepSeek API returned {StatusCode}: {Error}", response.StatusCode, errorText);
                return FallbackAnalysis(userQuery);
            }

            var responseText = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseText);
            var messageContent = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "";

            _logger.LogInformation("DeepSeek response: {Response}", messageContent);

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
            _logger.LogError(ex, "DeepSeek request failed");
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
