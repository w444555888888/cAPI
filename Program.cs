using BookingApi.Data;
using BookingApi.Services;
using BookingApi.Utils;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// Web 應用程式的建構器
var builder = WebApplication.CreateBuilder(args);


// appsettings.json as node env

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// appsettings.json 白名單
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new string[0];


//  CORS 設定
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


// 啟用 Controller 支援，處理 API 路由和 JSON 解析
builder.Services.AddControllers();


//  MongoDB Service 
builder.Services.AddSingleton<HotelService>();
builder.Services.AddSingleton<RoomService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<FlightService>();


// WebSocket
builder.Services.AddSingleton<WebSocketService>();
builder.Services.AddHostedService<NewsletterJob>();


// 6建立 App
var app = builder.Build();


// 靜態檔案 /uploads 對應 express.static
app.UseStaticFiles();

// CORS
app.UseCors("CorsPolicy");

// JSON / Controllers
app.UseAuthorization();
app.MapControllers();

// 全域錯誤處理
app.UseMiddleware<ErrorHandlingMiddleware>();


//  MongoDB 連線
var mongoSettings = app.Services.GetRequiredService<IOptions<MongoDbSettings>>().Value;
var client = new MongoClient(mongoSettings.ConnectionString);
var database = client.GetDatabase(mongoSettings.DatabaseName);
Console.WriteLine("MongoDB connected!");


// WebSocket 啟動
var wsService = app.Services.GetRequiredService<WebSocketService>();
wsService.InitWebSocket(app); // 對應 Node initWebSocket(server)


// 啟動 WebServer
var port = builder.Configuration.GetValue<int?>("Port");
if (port.HasValue)
{
    app.Run($"http://0.0.0.0:{port}");
}
else
{
    app.Run(); 
}


