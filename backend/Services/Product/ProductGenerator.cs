using SmartSearch.Models;

namespace SmartSearch.Services.Product;

public class ProductGenerator
{
    private readonly ILogger<ProductGenerator> _logger;

    // Category definitions with subcategories, types, and image pools
    private static readonly CategoryDef[] Categories = new[]
    {
        new CategoryDef
        {
            Name = "Clothing and Accessories",
            SubCategories = new[]
            {
                ("Topwear", new[] { "T-shirts", "Shirts", "Polos", "Tank Tops" }),
                ("Bottomwear", new[] { "Jeans", "Pants", "Shorts", "Trousers" }),
                ("Winter Wear", new[] { "Jackets", "Coats", "Sweaters", "Hoodies" }),
                ("Dresses", new[] { "Casual Dresses", "Evening Dresses", "Summer Dresses" }),
                ("Skirts", new[] { "Mini Skirts", "Midi Skirts", "Maxi Skirts" }),
                ("Innerwear and Sleepwear", new[] { "Underwear", "Pajamas", "Nightwear" }),
                ("Sportswear", new[] { "Tracksuits", "Sports T-shirts", "Sports Shorts" }),
            },
            ProductTypes = new[] { "t-shirt", "shirt", "jeans", "pants", "shorts", "jacket", "coat", "sweater", "hoodie", "sweatshirt", "dress", "skirt", "polo", "tank top", "tracksuit" },
            Images = new[]
            {
                "https://images.pexels.com/photos/8146450/pexels-photo-8146450.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/8146448/pexels-photo-8146448.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/966067/pexels-photo-966067.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/4295985/pexels-photo-4295985.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/15564085/pexels-photo-15564085.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/19461552/pexels-photo-19461552.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/6857728/pexels-photo-6857728.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/35240860/pexels-photo-35240860.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
            }
        },
        new CategoryDef
        {
            Name = "Footwear",
            SubCategories = new[]
            {
                ("Sports Shoes", new[] { "Running Shoes", "Sneakers", "Training Shoes" }),
                ("Casual Shoes", new[] { "Loafers", "Canvas Shoes", "Slip-ons" }),
                ("Boots", new[] { "Ankle Boots", "Knee-High Boots", "Combat Boots", "Winter Boots" }),
                ("Formal Shoes", new[] { "Oxfords", "Derby Shoes", "Brogues" }),
                ("Sandals", new[] { "Flip Flops", "Slides", "Strapped Sandals" }),
            },
            ProductTypes = new[] { "sneakers", "boots", "shoes", "loafers", "sandals", "oxfords", "flip flops", "slippers" },
            Images = new[]
            {
                "https://images.pexels.com/photos/19845610/pexels-photo-19845610.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/4296075/pexels-photo-4296075.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/26852497/pexels-photo-26852497.png?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/27113455/pexels-photo-27113455.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/27516985/pexels-photo-27516985.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/26954378/pexels-photo-26954378.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/19166246/pexels-photo-19166246.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/26732212/pexels-photo-26732212.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/27256470/pexels-photo-27256470.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/27141823/pexels-photo-27141823.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
            }
        },
        new CategoryDef
        {
            Name = "Accessories",
            SubCategories = new[]
            {
                ("Bags", new[] { "Handbags", "Backpacks", "Wallets", "Tote Bags" }),
                ("Headwear", new[] { "Caps", "Hats", "Beanies", "Fedora" }),
                ("Winter Accessories", new[] { "Scarves", "Gloves", "Mittens", "Earmuffs" }),
                ("Belts", new[] { "Leather Belts", "Casual Belts", "Formal Belts" }),
                ("Sunglasses", new[] { "Aviators", "Wayfarers", "Round Sunglasses" }),
            },
            ProductTypes = new[] { "bag", "backpack", "wallet", "cap", "hat", "scarf", "gloves", "belt", "sunglasses", "beanie" },
            Images = new[]
            {
                "https://images.pexels.com/photos/3808229/pexels-photo-3808229.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/19869755/pexels-photo-19869755.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/27204288/pexels-photo-27204288.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/26736140/pexels-photo-26736140.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/27046146/pexels-photo-27046146.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/5306338/pexels-photo-5306338.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/13580693/pexels-photo-13580693.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/38702945/pexels-photo-38702945.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
                "https://images.pexels.com/photos/6342793/pexels-photo-6342793.jpeg?auto=compress&cs=tinysrgb&h=650&w=940",
            }
        },
    };

    private static readonly string[] Colors = { "red", "blue", "black", "white", "green", "yellow", "pink", "gray", "brown", "beige", "navy", "orange", "purple", "maroon", "teal", "olive", "khaki", "burgundy", "charcoal", "cream" };
    private static readonly string[] Sizes = { "XS", "S", "M", "L", "XL", "XXL", "XXXL", "42", "44", "46", "48", "50", "52", "38", "39", "40", "41", "42", "43", "44" };
    private static readonly string[] Genders = { "male", "female", "unisex", "kids" };
    private static readonly string[] Materials = { "cotton", "leather", "polyester", "wool", "denim", "cashmere", "linen", "silk", "nylon", "fleece", "suede", "canvas", "rubber", "viscose", "acrylic" };
    private static readonly string[] Seasons = { "winter", "summer", "spring", "autumn", "all-season" };
    private static readonly string[] Features = { "warm", "waterproof", "hooded", "printed", "striped", "checked", "oversized", "slim-fit", "relaxed-fit", "breathable", "lightweight", "windproof", "quick-dry", "stretchable", "padded", "fleece-lined", "long", "short", "vintage", "retro" };
    private static readonly string[] Brands = { "UrbanStyle", "NordWear", "ClassicFit", "ModernThread", "StreetLine", "PrimeWear", "EcoFashion", "LuxLeather", "ComfortZone", "ActiveLife", "HeritageCo", "UrbanTrek", "PureCotton", "TrendyFit", "BoldStep", "MinimalWear", "RetroStyle", "FlexWear", "EliteCraft", "DailyWear" };
    private static readonly string[] Countries = { "India", "China", "Vietnam", "Bangladesh", "Turkey", "Italy", "Portugal", "Russia", "USA", "Germany" };
    private static readonly string[] Sellers = { "FashionHub", "StyleMart", "TrendZone", "WearHouse", "ClosetCo", "ApparelDirect", "FashionFirst", "StyleStop", "WearWell", "DressRight" };

    private readonly Random _rng = new(42);

    public ProductGenerator(ILogger<ProductGenerator> logger)
    {
        _logger = logger;
    }

    public List<Models.Product> Generate(int count = 10000)
    {
        var products = new List<Models.Product>(count);

        for (int i = 0; i < count; i++)
        {
            var category = Categories[_rng.Next(Categories.Length)];
            var (sub1, sub2List) = category.SubCategories[_rng.Next(category.SubCategories.Length)];
            var sub2 = sub2List[_rng.Next(sub2List.Length)];
            var productType = category.ProductTypes[_rng.Next(category.ProductTypes.Length)];
            var brand = Brands[_rng.Next(Brands.Length)];
            var color = Colors[_rng.Next(Colors.Length)];
            var material = Materials[_rng.Next(Materials.Length)];
            var gender = Genders[_rng.Next(Genders.Length)];
            var size = Sizes[_rng.Next(Sizes.Length)];
            var season = Seasons[_rng.Next(Seasons.Length)];

            // Generate 1-3 features
            var featureCount = _rng.Next(1, 4);
            var productFeatures = new List<string>();
            for (int f = 0; f < featureCount; f++)
            {
                var feat = Features[_rng.Next(Features.Length)];
                if (!productFeatures.Contains(feat))
                    productFeatures.Add(feat);
            }

            var title = GenerateTitle(brand, productType, color, gender);
            var description = GenerateDescription(productType, color, material, gender, season, productFeatures);
            var specs = GenerateSpecifications(color, material, size, season, gender, productFeatures);
            var formattedSpecs = FormatSpecifications(color, material, size, season, gender, productFeatures);
            var price = GeneratePrice(productType, material);
            var originalPrice = (decimal?)(price * (1.2m + (decimal)_rng.NextDouble() * 0.5m));
            var discount = $"{_rng.Next(10, 60)}% off";
            var images = GenerateImages(category.Images, _rng.Next(1, 4));
            var rating = Math.Round(_rng.NextDouble() * 2 + 3, 1);
            var reviews = _rng.Next(0, 5000);
            var breadcrumbs = $"{category.Name} | {sub1} | {sub2} | {gender} {sub2} | {title}";

            products.Add(new Models.Product
            {
                Title = title,
                Url = "",
                Pid = $"pid_{i + 1:D6}",
                FormattedUrl = "",
                Price = price,
                Currency = "INR",
                OriginalPrice = originalPrice,
                Discount = discount,
                FAssured = _rng.NextDouble() > 0.7,
                Highlights = string.Join(", ", productFeatures),
                Seller = Sellers[_rng.Next(Sellers.Length)],
                SellerRating = Math.Round(_rng.NextDouble() * 2 + 3, 1),
                Description = description,
                Specifications = specs,
                FormattedSpecifications = formattedSpecs,
                Images = images,
                ReturnPolicy = _rng.NextDouble() > 0.5 ? "10 Days Return Policy" : "No Returns",
                Category = category.Name,
                SubCategory1 = sub1,
                SubCategory2 = sub2,
                SubCategory3 = $"{gender}'s {sub2}",
                Breadcrumbs = breadcrumbs,
                CountryOfOrigin = Countries[_rng.Next(Countries.Length)],
                GenericName = productType,
                ManufacturedBy = brand,
                AvgRatings = rating,
                ReviewsCount = reviews,
                RatingsCount = reviews + _rng.Next(0, 1000),
                UniqId = $"gen_{i + 1:D6}_{Guid.NewGuid():N}",
                ScrapedAt = DateTime.UtcNow
            });
        }

        _logger.LogInformation("Generated {Count} products", products.Count);
        return products;
    }

    private static string GenerateTitle(string brand, string type, string color, string gender)
    {
        var genderPrefix = gender switch
        {
            "male" => "Men's",
            "female" => "Women's",
            "kids" => "Kids'",
            _ => "Unisex"
        };
        return $"{brand} {genderPrefix} {color} {type}";
    }

    private static string GenerateDescription(string type, string color, string material, string gender, string season, List<string> features)
    {
        var featStr = features.Count > 0 ? string.Join(", ", features) : "everyday";
        var genderStr = gender switch
        {
            "male" => "men",
            "female" => "women",
            "kids" => "kids",
            _ => "everyone"
        };

        return $"{char.ToUpper(type[0])}{type[1..]} made from high-quality {material}. " +
               $"This {color} {type} is designed for {genderStr} and perfect for {season} wear. " +
               $"Features: {featStr}. Comfortable fit and durable construction for long-lasting use.";
    }

    private static string GenerateSpecifications(string color, string material, string size, string season, string gender, List<string> features)
    {
        var specDict = new List<string>
        {
            $"{{\"Color\": \"{color}\"}}",
            $"{{\"Material\": \"{material}\"}}",
            $"{{\"Size\": \"{size}\"}}",
            $"{{\"Season\": \"{season}\"}}",
            $"{{\"Gender\": \"{gender}\"}}",
            $"{{\"Features\": \"{string.Join(", ", features)}\"}}"
        };
        return $"[{string.Join(", ", specDict)}]";
    }

    private static string FormatSpecifications(string color, string material, string size, string season, string gender, List<string> features)
    {
        return $"{color}: Color | {material}: Material | {size}: Size | {season}: Season | {gender}: Gender | {string.Join(", ", features)}: Features";
    }

    private decimal GeneratePrice(string productType, string material)
    {
        var basePrice = productType switch
        {
            "t-shirt" or "tank top" or "polo" => 200m,
            "shirt" => 400m,
            "jeans" or "pants" or "trousers" => 600m,
            "shorts" => 300m,
            "jacket" or "coat" => 1200m,
            "sweater" or "hoodie" or "sweatshirt" => 500m,
            "dress" or "skirt" => 500m,
            "sneakers" or "shoes" => 800m,
            "boots" => 1500m,
            "sandals" or "flip flops" or "slippers" => 250m,
            "bag" or "backpack" => 700m,
            "wallet" => 300m,
            "cap" or "hat" or "beanie" => 200m,
            "scarf" or "gloves" => 250m,
            "belt" => 350m,
            "sunglasses" => 400m,
            "tracksuit" => 600m,
            _ => 500m
        };

        var materialMultiplier = material switch
        {
            "leather" or "suede" => 2.5m,
            "cashmere" or "silk" => 2.0m,
            "wool" => 1.5m,
            "linen" => 1.3m,
            _ => 1.0m
        };

        var variance = (decimal)(_rng.NextDouble() * 0.5 + 0.75);
        return Math.Round(basePrice * materialMultiplier * variance, 0);
    }

    private static string GenerateImages(string[] pool, int count)
    {
        var selected = new HashSet<string>();
        var rng = new Random();
        while (selected.Count < Math.Min(count, pool.Length))
            selected.Add(pool[rng.Next(pool.Length)]);
        return string.Join(" | ", selected);
    }

    private class CategoryDef
    {
        public string Name { get; set; } = "";
        public (string, string[])[] SubCategories { get; set; } = Array.Empty<(string, string[])>();
        public string[] ProductTypes { get; set; } = Array.Empty<string>();
        public string[] Images { get; set; } = Array.Empty<string>();
    }
}
