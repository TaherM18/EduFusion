using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Net.Http.Headers;
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


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new
    SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
    "token",
    new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Name = HeaderNames.Authorization
    }
    );
    c.AddSecurityRequirement(
    new OpenApiSecurityRequirement {
{ new OpenApiSecurityScheme {
Reference = new OpenApiReference {
Type = ReferenceType.SecurityScheme,
Id = "token"
},
},Array.Empty<string>()
}
    }
    );
});

// Configure NpgsqlConnection
builder.Services.AddSingleton<NpgsqlConnection>((service) =>
{
    string connectionString = service.GetRequiredService<IConfiguration>().GetConnectionString("pgconn") ?? throw new ArgumentNullException("pgconn connection string is null.");
    return new NpgsqlConnection(connectionString);
});

// Configure Repositories
builder.Services.AddSingleton<IStudentInterface, StudentRepository>();
builder.Services.AddSingleton<ITeacherInterface, TeacherRepository>();
// builder.Services.AddSingleton<ITimeTableInterface, TimeTableRepository>();
builder.Services.AddSingleton<IExamInterface, ExamRepository>();
builder.Services.AddSingleton<IUserInterface, AuthRepository>();
builder.Services.AddSingleton<IStandardInterface, StandardRepository>();
builder.Services.AddSingleton<ISubjectTrackingInterface, SubjectTrackingRepository>();

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