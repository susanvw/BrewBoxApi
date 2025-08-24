using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BrewBoxApi.Infrastructure.Interceptors;

public class QueryLoggingInterceptor() : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
    {
        if (eventData.Duration.TotalMilliseconds > 500) // Log slow queries
        {
            Serilog.Log.Warning($"Slow query ({eventData.Duration}): {command.CommandText}");
        }
        return base.ReaderExecuted(command, eventData, result);
    }
}
