using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

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

    public async Task<ResMessage> ExecuteStoredProcedureAsync(string spName, Dictionary<string, string> parameters)
    {
        var result = new ResMessage();
        var parameterString = string.Join(", ", parameters.Select(p => $"@{p.Key}=@{p.Key}"));
        var sql = $"EXEC {spName} {parameterString}";

        using (var connection = _dbcontext.Database.GetDbConnection())
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                foreach (var param in parameters)
                {
                    command.Parameters.Add(new SqlParameter(param.Key, param.Value));
                }

                try
                {
                    await connection.OpenAsync();

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
                            result.Msg.Add(table);
                        } while (await reader.NextResultAsync());
                    };                    
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
