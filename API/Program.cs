using Npgsql;
using EduFusion.Repositories.Interfaces;
using EduFusion.Repositories.Models;
using EduFusion.Repositories.Implementations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<NpgsqlConnection>((service) => {
    string connectionString = service.GetRequiredService<IConfiguration>().GetConnectionString("pgconn") ?? throw new ArgumentNullException("pgconn connection string is null.");
    return new NpgsqlConnection(connectionString);
});
builder.Services.AddSingleton<IUserInterface, UserRepository>();
builder.Services.AddSingleton<IStudentInterface, StudentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
