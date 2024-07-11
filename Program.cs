using Microsoft.EntityFrameworkCore;
using WebAPI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 設置 Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // 每日滾動新文件
    .CreateLogger();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IStoredProcedureService, StoredProcedureService>();
builder.Services.AddScoped<IResultFormatter, JsonResultFormatter>();
builder.Services.AddScoped<MyRepository>();
builder.Services.AddSingleton<ILoggerService, LoggerService>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(connectionString));

app.MapPost("/execute", async (StoredProcedureRequest request, MyRepository repository, IResultFormatter resultformatter, ILoggerService logger) =>
{
    if (request == null || string.IsNullOrEmpty(request.ProcedureName) || request.Parameters == null)
    {
        return resultformatter.FormatDefaultResult();
    }

    return await repository.ExecuteStoredProcedureAsync(request.ProcedureName, request.Parameters);
});

app.Run();