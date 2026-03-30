using Skillforge.Repository;
using Skillforge.Service;
using Skillforge.Data;
using Skillforge.Domain;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Services
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SkillForgeDB>();
//builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IUserRepository,UserRepository>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SkillForgeDB>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Skillforge")));

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();


}

// Middlewares
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
