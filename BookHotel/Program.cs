using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
//using BookHotel.Repositories.Admin;
using BookHotel.Services.Mail;
using BookHotel.Repositories.Admin;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

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

builder.Services.AddControllers();

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Cho phép ứng dụng lắng nghe trên cổng 5000
// app.Urls.Add("https://*:7242");
app.Urls.Add("http://*:5000");


app.Run();