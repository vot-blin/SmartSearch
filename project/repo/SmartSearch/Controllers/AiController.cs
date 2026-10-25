using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSearch.Models.Search;
using SmartSearch.Services.Ai;

namespace SmartSearch.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly AiSearchService _aiSearch;

    public AiController(AiSearchService aiSearch) => _aiSearch = aiSearch;

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] AiSearchRequest req)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(req.Query))
            return BadRequest(new { message = "Запрос не может быть пустым" });

        var limit = req.Limit > 0 ? Math.Min(req.Limit, 500) : 500;
        var result = await _aiSearch.SearchAsync(req.Query, limit);
        return Ok(result);

        //var result = await _aiSearch.SearchAsync(req.Query);
        //return Ok(result); old code
    }
}
