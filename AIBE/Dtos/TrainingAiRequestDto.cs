namespace AIBE.Dtos;

public class TrainingAiRequestDto
{
    public IFormFile? File { get; set; }
    public string? Prompt { get; set; }
}
