using Npgsql;
using Repositories.Interfaces;
using Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure NpgsqlConnection
builder.Services.AddSingleton<NpgsqlConnection>((service) => {
    string connectionString = service.GetRequiredService<IConfiguration>().GetConnectionString("pgconn") ?? throw new ArgumentNullException("pgconn connection string is null.");
    return new NpgsqlConnection(connectionString);
});

// Configure Repositories
builder.Services.AddSingleton<IStudentInterface, StudentRepository>();
builder.Services.AddSingleton<ITeacherInterface, TeacherRepository>();
builder.Services.AddSingleton<ITimeTableInterface, TimeTableRepository>();
builder.Services.AddSingleton<IExamInterface, ExamRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("corsapp");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();


app.Run();