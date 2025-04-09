    using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
//using BookHotel.Repositories.Admin;
using BookHotel.Services.Mail;
using BookHotel.Repositories.Admin;
using Microsoft.OpenApi.Models;
using BookHotel.Extensions;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository,BookingRepository>();
builder.Services.AddScoped<IBookingRoomRepository,BookingRoomRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

// Cấu hình CORS
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

// Cấu hình JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Cấu hình Swagger token
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
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Cấu hình Controllers với JSON hỗ trợ tiếng Việt (không bị lỗi font)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

//mail
builder.Services.AddTransient<IEmailService, EmailService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAllOrigins");

// Luôn kích hoạt Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookHotel API V1");
    c.RoutePrefix = "swagger"; // Đặt đường dẫn Swagger
});
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

// Cho phép ứng dụng lắng nghe trên cổng 5000
app.Urls.Add("https://*:7242");
//app.Urls.Add("http://*:5000");


app.Run();