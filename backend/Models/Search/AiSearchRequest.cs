using System.ComponentModel.DataAnnotations;

namespace SmartSearch.Models.Search;

public class AiSearchRequest
{
    [Required]
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 200; //new
}
