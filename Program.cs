using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Niwahana_backend.Business;
using Niwahana_backend.Data.Repository.BaseSQLDbConnection;
using Niwahana_backend.Repository.Data;
using Niwahana_backend.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Niwahana backend v1",
        Version = "v1",
        Description = "Niwahana Backend API"
    });


    // JWT Authentication for Swagger

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token."
    });


    // JWT Security Requirement

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});


// Database

builder.Services.AddScoped<BaseSqlDL>();


// Repositories

builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProfileImageRepository>();
builder.Services.AddScoped<BungalowsRepository>();


// Business

builder.Services.AddScoped<AuthBusiness>();
builder.Services.AddScoped<UserBusiness>();
builder.Services.AddScoped<ProfileImageBusiness>();
builder.Services.AddScoped<BungalowsBusiness>();


// JWT Service

builder.Services.AddScoped<JwtService>();


// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]?? throw new Exception("JWT Key is missing.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"]?? throw new Exception("JWT Issuer is missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"]?? throw new Exception("JWT Audience is missing.");


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
    });


builder.Services.AddAuthorization();


// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAngular",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});


var app = builder.Build();


// Swagger

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Niwahana backend v1"
        );
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();