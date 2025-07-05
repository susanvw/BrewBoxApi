using System.Linq.Expressions;
using System.Text.Json;

namespace BrewBoxApi.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string id)
        : base($"Entity '{entityName}' with ID '{id}' was not found.")
    {
        EntityName = entityName;
        Id = id;
    }

    public NotFoundException(string entityName, Expression expression)
        : base($"Entity '{entityName}' not found for expression '{JsonSerializer.Serialize(expression.ToString())}'.")
    {
        EntityName = entityName;
        Expression = expression.ToString();
    }

    public string EntityName { get; }
    public string? Id { get; }
    public string? Expression { get; }
}