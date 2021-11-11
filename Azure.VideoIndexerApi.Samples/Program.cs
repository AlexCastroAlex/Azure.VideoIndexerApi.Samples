using Azure.VideoIndexerApi.Samples;
using Azure.VideoIndexerApi.Samples.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<VideoIndexerConfig>(builder.Configuration.GetSection("VideoIndexerConfig"));
builder.Services.Configure<BlobConfig>(builder.Configuration.GetSection("BlobConfig"));
builder.Services.AddTransient<IAzureVideoIndexerService, AzureVideoIndexerService>();
builder.Services.AddTransient<IBlobContainerService, BlobContainerService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGet("/AVS/GetAccount", async (IAzureVideoIndexerService service) => await  service.GetAccountAccessTokenAsync()).WithName("GetAccount");

app.MapGet("/AVS/IndexVideosWithUploadFolder", async (IAzureVideoIndexerService Azureservice , IBlobContainerService blobService , string path) =>
{
    var files = Directory.GetFiles(path);
    foreach (var file in files)
    {
        var name = Path.GetFileName(file);
        await blobService.UploadFileAsync(file, name);
        string sasUrl = blobService.GetBlobReadSas(name);
        await Azureservice.IndexAsync(sasUrl, name);
 
    }

}).WithName("IndexVideosWithUploadFolder");

app.MapGet("/AVS/IndexVideosWithFilePath", async (IAzureVideoIndexerService Azureservice, IBlobContainerService blobService, string filepath) =>
{
    var name = Path.GetFileName(filepath);
    await blobService.UploadFileAsync(filepath, name);
    string sasUrl = blobService.GetBlobReadSas(name);
    await Azureservice.IndexAsync(sasUrl, name);

}).WithName("IndexVideosWithFilePath");


app.MapGet("/AVS/ListVideos", async (IAzureVideoIndexerService service) =>
{
    var results = await service.ListVideosAsync();
    return Results.Ok(results.results);
}).WithName("ListVideos");

app.MapGet("/AVS/SearchVideo", async (IAzureVideoIndexerService service,string name) => 
{
    var results = await service.SearchAsync(name);
    return Results.Ok(results.results);
}).WithName("SearchVideo");




app.Run();

