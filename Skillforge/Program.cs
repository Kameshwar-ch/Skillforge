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
<<<<<<< HEAD
 
=======

>>>>>>> ce7febd16c783b760f1b3098179f2a10e0120a1c
var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
<<<<<<< HEAD
 
 
=======


>>>>>>> ce7febd16c783b760f1b3098179f2a10e0120a1c
}
 
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
<<<<<<< HEAD
app.Run();
=======
app.Run();
>>>>>>> ce7febd16c783b760f1b3098179f2a10e0120a1c
