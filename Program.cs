using Microsoft.EntityFrameworkCore;
using WebAPI;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 設置 Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // 每日滾動新文件
    .CreateLogger();

// 設置 AppSettings
AppSettings.DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
AppSettings.CommectionTimeOut = Convert.ToInt32(builder.Configuration.GetConnectionString("CommectionTimeOut"));
AppSettings.ConnectionOpenWaitTime = TimeSpan.FromSeconds(Convert.ToInt32(builder.Configuration.GetConnectionString("ConnectionOpenWaitTime"))); 

// 設定 DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(AppSettings.DefaultConnection, 
    sqlServerOptions => sqlServerOptions.CommandTimeout(AppSettings.CommectionTimeOut)));

// 設定服務
builder.Services.AddScoped<MyRepository>();
builder.Services.AddScoped<IStoredProcedureService, StoredProcedureService>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGet("/", () => Results.Ok());


app.MapPost("/execute", async (HttpContext context, MyRepository repository) =>
{
    return await repository.ExecuteStoredProcedureAsync(context);
});

app.Run();
