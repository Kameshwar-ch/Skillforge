using Skillforge.Repository;
using Skillforge.Service;
using Skillforge.Data;
using Skillforge.Domain;
<<<<<<< HEAD
using FluentValidation.AspNetCore;
using FluentValidation;
=======
using Microsoft.EntityFrameworkCore;
>>>>>>> 55e6e8186bf93cb46c07bbabf4f5611d773cec65

var builder = WebApplication.CreateBuilder(args);
// Services
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SkillForgeDB>();
<<<<<<< HEAD
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
=======
>>>>>>> 55e6e8186bf93cb46c07bbabf4f5611d773cec65
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
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
