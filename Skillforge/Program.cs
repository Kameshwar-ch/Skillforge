using Skillforge.Repository;
using Skillforge.Service;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Skillforge.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// this is for fetching the data from the env file.
DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(),".." ,".env"));

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string is missing from .env file!");
}

builder.Services.AddDbContext<SkillForgeDB>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAssessmentRepository, AssessmentRepository>();
builder.Services.AddScoped<IAssessmentService, AssessmentService>();
builder.Services.AddScoped<IAuditService, EFAuditRepository>();
builder.Services.AddScoped<IJWTProviderService, JWTProviderService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ICompetencyRepository, CompetencyRepository>();
builder.Services.AddScoped<ICompetencyService, CompetencyService>();
builder.Services.AddScoped<ISkillGapService,SkillGapService>();
builder.Services.AddScoped<ISkillGapRepository,SkillGapRepository>();
builder.Services.AddScoped<IResultRepository,ResultRepository>();
builder.Services.AddScoped<IResultService,ResultService>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();

var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
if (secretKey == null)
{
    throw new Exception("Secret key is NUll");
}
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authentication using Bearer scheme"
    });
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
    });
});
 

builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "Skillforge",
        ValidAudience = "SkillForgeUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        RoleClaimType = "role"
    };
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
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
