using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace WebAPI;

public class StoredProcedureService : IStoredProcedureService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILoggerService _loggerService;
    public StoredProcedureService(ApplicationDbContext dbcontext, ILoggerService loggerService)
    {
        _dbcontext = dbcontext;
        _loggerService = loggerService;
    }

    private async Task<bool> OpenConnectWaitTime(DbConnection dbConnection)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(AppSettings.ConnectionOpenWaitTime);

        try
        {
            await dbConnection.OpenAsync(cts.Token);
            return true; // Connection opened successfully
        }
        catch (OperationCanceledException)
        {
            // Connection opening timed out
            return false;
        }
    }

    public async Task<ResultData> ExecuteStoredProcedureAsync(StoredProcedureRequest request)
    {
        var result = new ResultData();

        using (var connection = _dbcontext.Database.GetDbConnection())
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = request.ProcedureName;
                command.CommandType = CommandType.StoredProcedure;

                foreach (var param in request.Parameters)
                {
                    command.Parameters.Add(new SqlParameter(param.Key, param.Value));
                }

                try
                {
                    if (await OpenConnectWaitTime(connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            do
                            {
                                var table = new List<Dictionary<string, object>>();
                                var columnSchema = await reader.GetColumnSchemaAsync();

                                while (await reader.ReadAsync())
                                {
                                    var row = new Dictionary<string, object>(reader.FieldCount);
                                    foreach (var column in columnSchema)
                                    {
#pragma warning disable CS8601 // 可能有 Null 參考指派。
                                        int ordinal = reader.GetOrdinal(column.ColumnName);
                                        row[column.ColumnName] = reader.IsDBNull(ordinal)
                                            ? null
                                            : reader.GetValue(ordinal);
#pragma warning restore CS8601 // 可能有 Null 參考指派。
                                    }
                                    table.Add(row);
                                }

                                if(table.Count > 0)
                                {
                                    result.Data.Add(table);
                                }

                            } while (await reader.NextResultAsync());
                        };
                    }               
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        await connection.CloseAsync();
                }
            };
        };
        
        return result;
    }
}
