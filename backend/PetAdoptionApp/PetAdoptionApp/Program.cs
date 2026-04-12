using Microsoft.Extensions.FileProviders;
using Neo4j.Driver;
using PetAdoptionApp.Services;
using PetAdoptionApp.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Citamo podatke iz appsettings.json
var neo4jUri = builder.Configuration["Neo4j:Uri"];
var neo4jUser = builder.Configuration["Neo4j:Username"];
var neo4jPass = builder.Configuration["Neo4j:Password"];

// Pravimo IDriver i registrujemo ga kao singleton
builder.Services.AddSingleton<IDriver>(GraphDatabase.Driver(
    neo4jUri,
    AuthTokens.Basic(neo4jUser, neo4jPass)
    ));

// Add services to the container.

builder.Services.AddScoped<IShelterService, ShelterService>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();

builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// kreira images folder ako ne postoji
var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "images");
Directory.CreateDirectory(imagesPath); // ne baca gresku ako  postoji

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
});

//builder.Services.AddOpenApi();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseStaticFiles(new StaticFileOptions
{
    // Da ASP.NET moze da servira fajlove iz foldera na disku:
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/images"
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
