using Microsoft.EntityFrameworkCore;
using WebAPI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 設置 Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7) // 每日滾動新文件
    .CreateLogger();

var aps = new AppSettings();
aps.DefaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
aps.CommectionTimeOut = Convert.ToInt32(builder.Configuration.GetConnectionString("CommectionTimeOut"));
aps.ConnectionOpenWaitTime = TimeSpan.FromSeconds(Convert.ToInt32(builder.Configuration.GetConnectionString("ConnectionOpenWaitTime"))); 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(aps.DefaultConnection, 
    sqlServerOptions => sqlServerOptions.CommandTimeout(aps.CommectionTimeOut)));

builder.Services.AddScoped<AppSettings>();
builder.Services.AddScoped<IStoredProcedureService, StoredProcedureService>();
builder.Services.AddScoped<IResultFormatter, JsonResultFormatter>();
builder.Services.AddScoped<MyRepository>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok());

app.MapPost("/execute", async (StoredProcedureRequest request, MyRepository repository, IResultFormatter resultformatter, ILoggerService logger) =>
{
    if (request == null || string.IsNullOrEmpty(request.ProcedureName) || request.Parameters == null)
    {
        return resultformatter.FormatDefaultResult();
    }

    return await repository.ExecuteStoredProcedureAsync(request.ProcedureName, request.Parameters);
});

app.Run();
