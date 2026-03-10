var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddUserSecrets<Program>(optional: true);

// Add services to the container.
builder.Services.Configure<AIBE.Services.Embedding.OpenAISettings>(
    builder.Configuration.GetSection(AIBE.Services.Embedding.OpenAISettings.SectionName));
builder.Services.Configure<AIBE.Services.VectorStore.QdrantSettings>(
    builder.Configuration.GetSection(AIBE.Services.VectorStore.QdrantSettings.SectionName));
builder.Services.Configure<AIBE.Services.Training.TrainingSettings>(
    builder.Configuration.GetSection(AIBE.Services.Training.TrainingSettings.SectionName));

builder.Services.AddHttpClient<AIBE.Services.Embedding.IEmbeddingService, AIBE.Services.Embedding.OpenAIEmbeddingService>();
builder.Services.AddSingleton<AIBE.Services.Extraction.IZipExtractorService, AIBE.Services.Extraction.ZipExtractorService>();
builder.Services.AddSingleton<AIBE.Services.Chunking.IChunkingService, AIBE.Services.Chunking.ChunkingService>();
builder.Services.AddSingleton<AIBE.Services.VectorStore.IVectorStoreService, AIBE.Services.VectorStore.QdrantVectorStoreService>();
builder.Services.AddScoped<AIBE.Services.Training.ITrainingPipelineService, AIBE.Services.Training.TrainingPipelineService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
