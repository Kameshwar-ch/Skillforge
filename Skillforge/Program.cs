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
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SkillForgeDB>();
    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User { Name = "Rahul Kumar", Role = UserRole.Admin, Email = "rahul@skillforge.com", Phone = "9876543210", Status = true, Password = BCrypt.Net.BCrypt.HashPassword("Admin@123") },
            new User { Name = "Priya Sharma", Role = UserRole.Manager, Email = "priya@skillforge.com", Phone = "9876543211", Status = true, Password = BCrypt.Net.BCrypt.HashPassword("Manager@123") },
            new User { Name = "Amit Singh", Role = UserRole.Trainer, Email = "amit@skillforge.com", Phone = "9876543212", Status = true, Password = BCrypt.Net.BCrypt.HashPassword("Trainer@123") },
            new User { Name = "Sneha Patel", Role = UserRole.Employee, Email = "sneha@skillforge.com", Phone = "9876543213", Status = true, Password = BCrypt.Net.BCrypt.HashPassword("Employee@123") },
            new User { Name = "Deepak Verma", Role = UserRole.HR, Email = "deepak@skillforge.com", Phone = "9876543214", Status = true, Password = BCrypt.Net.BCrypt.HashPassword("Hr@12345") }
        );
        context.SaveChanges();
        Console.WriteLine("Seed data added!");
    }
}
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
