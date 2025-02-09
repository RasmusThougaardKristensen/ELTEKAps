using ELTEKAps.Management.ApplicationServices.Repositories.Operations;
using ELTEKAps.Management.Domain.Operations;
using ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace ELTEKAps.Management.Infrastructure.Repositories.Operations;
public class OperationRepository : BaseRepository<Operation, OperationEntity>, IOperationRepository
{
    public OperationRepository(TaskContext context) : base(context)
    {
    }

    public async Task<Operation?> GetByRequestId(string requestId)
    {
        var operationEntity = await GetOperationsDbSet()
            .Where(x => x.RequestId == requestId)
            .AsNoTracking()
            .SingleOrDefaultAsync();
        return operationEntity is null ? null : Map(operationEntity);
    }

    public async Task<ICollection<Operation>> GetTaskOperations(Guid taskId)
    {
        var task = await GetOperationsDbSet()
                    .Where(x => x.TaskId == taskId)
                    .AsNoTracking()
                    .ToListAsync();
        return task.Select(Map).ToImmutableHashSet();
    }

    private DbSet<OperationEntity> GetOperationsDbSet()
    {
        if (Context.Operations is null)
            throw new InvalidOperationException("Database configuration not setup correctly");
        return Context.Operations;
    }

    protected override Operation Map(OperationEntity entity)
    {
        return OperationMapper.Map(entity);
    }

    protected override OperationEntity Map(Operation model)
    {
        return OperationMapper.Map(model);
    }


}
