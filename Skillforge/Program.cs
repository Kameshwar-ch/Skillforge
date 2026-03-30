using Microsoft.EntityFrameworkCore;
using Skillforge.Repository;
using Skillforge.Service;
using SkillForgeLibrary.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
    app.MapOpenApi();
}
 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
