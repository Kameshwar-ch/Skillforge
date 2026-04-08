using Skillforge.Repository;
using Skillforge.Service;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// this is for fetching the data from the env file.
// This looks in the folder above the current one
DotNetEnv.Env.Load(Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, ".env"));

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string is missing from .env file!");
}

builder.Services.AddDbContext<SkillForgeDB>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuditService, EFAuditRepository>();
builder.Services.AddScoped<IJWTProviderService, JWTProviderService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ISkillGapService,SkillGapService>();
builder.Services.AddScoped<ISkillGapRepository,SkillGapRepository>();
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (secretKey == null)
{
    throw new Exception("Secret key is NUll");
}

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Skillforge",
        ValidAudience = "SkillForgeUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});


builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();
 
app.MapControllers();

app.Run();
