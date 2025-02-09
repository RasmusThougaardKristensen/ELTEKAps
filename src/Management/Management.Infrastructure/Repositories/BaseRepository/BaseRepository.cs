using ELTEKAps.Management.ApplicationServices.Repositories;
using ELTEKAps.Management.Domain;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace ELTEKAps.Management.Infrastructure.Repositories.BaseRepository;
public abstract class BaseRepository<T, TE> : IBaseRepository<T> where T : BaseModel where TE : BaseEntity
{
    private readonly DbSet<TE> _dbSet;
    protected readonly TaskContext Context;

    protected BaseRepository(TaskContext context)
    {
        Context = context;
        _dbSet = context.Set<TE>();
    }

    public async Task<T?> GetById(Guid id)
    {
        var fetchedEntity = await _dbSet
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == id);
        return fetchedEntity is null ? null : Map(fetchedEntity);
    }

    public async Task<ICollection<T>> GetAll()
    {
        var all = await _dbSet.ToListAsync();
        return all.Select(Map).ToImmutableHashSet();
    }

    public async Task<T> Upsert(T baseModel)
    {
        var existingEntity = await GetTracked(baseModel.Id);

        if (existingEntity == null) return await Add(baseModel);
        // right now existing is tracked by ef core - use this to apply updates
        var updatedEntity = Map(baseModel);

        Context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity); // all simple values on entity
        existingEntity.ModifiedUtc = DateTime.UtcNow;

        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        return Map(existingEntity);
    }

    internal async Task<T> Add(T baseModel)
    {
        var entity = Map(baseModel);
        await _dbSet.AddAsync(entity);
        await SaveAsync(entity);

        Context.ChangeTracker.Clear();

        return Map(entity);
    }

    private async Task<TE?> GetTracked(Guid id)
    {
        var fetchedEntity = await _dbSet
            .SingleOrDefaultAsync(t => t.Id == id);
        return fetchedEntity;
    }

    protected abstract T Map(TE entity);
    protected abstract TE Map(T model);

    private async Task SaveAsync(TE baseEntity)
    {
        baseEntity.ModifiedUtc = DateTime.UtcNow;

        await Context.SaveChangesAsync();
    }
}
