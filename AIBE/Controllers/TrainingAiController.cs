using AIBE.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AIBE.Controllers;

[ApiController]
[Route("api/training-ai")]
public class TrainingAiController : ControllerBase
{
    [HttpPost]
    [Route("training")]
    [Consumes("multipart/form-data")]
    public IActionResult Post([FromForm] TrainingAiRequestDto request)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("Zip không được rỗng.");

        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt không được rỗng.");

        return Ok();
    }
}
