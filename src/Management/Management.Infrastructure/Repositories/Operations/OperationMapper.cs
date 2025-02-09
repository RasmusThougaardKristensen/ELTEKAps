using ELTEKAps.Management.Domain.Operations;
using System.Text.Json;

namespace ELTEKAps.Management.Infrastructure.Repositories.Operations;
internal static class OperationMapper
{
    internal static OperationEntity Map(Operation operation)
    {
        var entity = new OperationEntity(operation.Id, operation.CreatedUtc, operation.ModifiedUtc, operation.Deleted,
            operation.RequestId, operation.TaskId, operation.CreatedBy, operation.Name, operation.Status,
            operation.CompletedUtc,
            DictionaryToString(operation.Data));
        return entity;
    }

    internal static Operation Map(OperationEntity operation)
    {
        var model = new Operation(operation.Id, operation.RequestId, operation.CreatedBy, operation.TaskId,
            operation.OperationName, operation.Status,
            operation.CreatedUtc, operation.ModifiedUtc,
            operation.CompletedUtc, StringToDictionary(operation.Data));
        return model;
    }

    private static Dictionary<string, string>? StringToDictionary(string? data)
    {
        if (data is null) return null;
        return JsonSerializer.Deserialize<Dictionary<string, string>>(data);
    }

    private static string? DictionaryToString(Dictionary<string, string>? data)
    {
        if (data is null) return null;
        return JsonSerializer.Serialize(data);
    }
}
