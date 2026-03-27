using Skillforge.Repository;
using Skillforge.Service;
using Skillforge.Data;
using Skillforge.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();// Add these two lines

builder.Services.AddDbContext<SkillForgeDB>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed data - remove this after first run

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
