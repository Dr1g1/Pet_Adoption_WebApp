using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver;
using PetAdoptionApp.Interfaces;
using PetAdoptionApp.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Citamo podatke iz appsettings.json
var neo4jUri = builder.Configuration["Neo4j:Uri"];
var neo4jUser = builder.Configuration["Neo4j:Username"];
var neo4jPass = builder.Configuration["Neo4j:Password"];
var jwtSecret = builder.Configuration["Jwt:Secret"];

// Pravimo IDriver i registrujemo ga kao singleton
builder.Services.AddSingleton<IDriver>(GraphDatabase.Driver(
    neo4jUri,
    AuthTokens.Basic(neo4jUser, neo4jPass)
    ));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        }
        ;
    });
builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddScoped<IShelterService, ShelterService>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IAdoptionRequestService, AdoptionRequestService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); //testiranje autentifikovanih endopointa ide preko postmana, ne preko swagger-a.

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
