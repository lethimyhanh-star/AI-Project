namespace AIBE.Services.Training;

public interface ITrainingPipelineService
{
    Task ExecuteAsync(Stream zipStream, string prompt, CancellationToken cancellationToken = default);
}
