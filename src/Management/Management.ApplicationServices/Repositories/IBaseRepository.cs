using ELTEKAps.Management.Domain;

namespace ELTEKAps.Management.ApplicationServices.Repositories;
public interface IBaseRepository<T> where T : BaseModel
{
    Task<T?> GetById(Guid id);
    Task<ICollection<T>> GetAll();
    Task<T> Upsert(T baseModel);
}
