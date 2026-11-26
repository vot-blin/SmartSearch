using SmartSearch.Models;

namespace SmartSearch.Models.Search;

public class ProductFilter
{
    public string? Color { get; set; }
    public string? Gender { get; set; }
    public string? ProductType { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
}