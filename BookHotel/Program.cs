using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using BookHotel.Data;
using BookHotel.Extensions;
using BookHotel.Middlewares;
using BookHotel.Repositories.Admin;
using BookHotel.Services.Mail;
using BookHotel.Services;
using BookHotel.Profiles;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<ITypeRoomRepository, TypeRoomRepository>();
builder.Services.AddScoped<IAmenityRepository, AmenityRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingRoomRepository, BookingRoomRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();


builder.Services.AddAutoMapper(typeof(MappingProfile));

// File upload
builder.Services.AddScoped<FileService>();

// Mail service
builder.Services.AddTransient<IEmailService, EmailService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// JSON UTF-8 support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });


//Cấu hình JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Swagger + JWT token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookHotel", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Middlewares
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAllOrigins");

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookHotel API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

// Listen on port
app.Urls.Add("https://*:7242");
// app.Urls.Add("http://*:5000");

app.Run();
