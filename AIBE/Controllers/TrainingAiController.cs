using AIBE.Dtos;
using AIBE.Services.Training;
using Microsoft.AspNetCore.Mvc;

namespace AIBE.Controllers;

[ApiController]
[Route("api/training-ai")]
public class TrainingAiController : ControllerBase
{
    private readonly ITrainingPipelineService _pipeline;
    private readonly ILogger<TrainingAiController> _logger;

    public TrainingAiController(ITrainingPipelineService pipeline, ILogger<TrainingAiController> logger)
    {
        _pipeline = pipeline;
        _logger = logger;
    }

    [HttpPost("training")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Post([FromForm] TrainingAiRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.File == null || request.File.Length == 0)
            return BadRequest("Zip không được rỗng.");

        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest("Prompt không được rỗng.");

        try
        {
            await using var stream = request.File.OpenReadStream();
            await _pipeline.ExecuteAsync(stream, request.Prompt!, cancellationToken);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Training pipeline failed");
            return StatusCode(500, "Lỗi xử lý training.");
        }
    }
}
