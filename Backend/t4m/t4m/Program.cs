using Microsoft.Extensions.Configuration;
using t4m.Models;
using t4m.Services;
using Swashbuckle.AspNetCore;
using Microsoft.OpenApi.Models;
using AutoMapper;
using t4m.Helpers;
using System.Text.Json;
using static t4m.Helpers.JsonSerializerHelper;
using MongoDB.Driver;
using t4m.Providers.PronunciationData.DateTimeProviders;
using t4m.Providers.GuidProviders;
using t4m.Providers.FileSystemProviders;
using t4m.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var translatiorApiBaseUrl = builder.Configuration[Constants.Appsettings.TranslatorApiBaseApi]
    ?? throw new MissingFieldException($"{Constants.Appsettings.TranslatorApiBaseApi} property in appsettings is null or does not exist.");

var databaseName = builder.Configuration[Constants.Appsettings.DatabaseNameKey]
    ?? throw new MissingFieldException($"{Constants.Appsettings.DatabaseNameKey} property in appsettings is null or does not exist.");

var mongoConnectionString = builder.Configuration[Constants.Appsettings.ConnectionStringKey]
    ?? throw new MissingFieldException($"{Constants.Appsettings.ConnectionStringKey} property in appsettings is null or does not exist.");


builder.Services.AddHttpClient(Constants.API.DefaultHttpClientName,
    client => client.BaseAddress = new Uri(translatiorApiBaseUrl))
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            AllowAutoRedirect = false,
            UseCookies = false
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "t4m API", Version = "v1" });
});

builder.Services.AddLogging(loggingBuilder => { loggingBuilder.AddDebug(); });

builder.Services.AddAutoMapper(typeof(Program));



builder.Services.AddSingleton(new MongoClient(mongoConnectionString)
    .GetDatabase(databaseName));

builder.Services.AddTransient<ITranslationService, TranslationService>();
builder.Services.AddTransient<IPronunciationService, PronunciationService>();

builder.Services.AddTransient<IPronunciationCacheRepository, PronunciationCacheRepository>();
builder.Services.AddTransient<ITranslationRepository, TranslationRepository>();

builder.Services.AddScoped<IFileProvider, FileProvider>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IGuidProvider, GuidProvider>();
builder.Services.AddScoped<IDirectoryProvider, DirectoryProvider>();

builder.Services.AddSingleton<JsonSerializerOptions>(GetDefaultJsonSerializerOptions);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.Run();

