using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolvedTax.Data.EFRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetReadOnlyList();
        IQueryable<TEntity> GetNotDeleted();
        Task<TEntity> Find<T>(T id);

        Task<int> Add(TEntity entity, bool isSaveChanges = true);

        Task Change(TEntity entity, bool isSaveChanges = true);
        Task ChangeRange(List<TEntity> entity, bool isSaveChanges = true);
        Task SeederChangeRange(List<TEntity> entity, bool isSaveChanges = true);


        Task Delete<T>(T id, bool isSaveChanges = true);
        Task DeleteRange<T>(List<T> ids, bool isSaveChanges = true);
        Task RemoveRangeAsync(List<TEntity> entities, bool isSaveChanges = true);
        Task AddRangeAsync(List<TEntity> entities, bool isSaveChanges = true);
        Task<int> Save();
        //Task SoftDelete(BaseEntity entity);
        Task SeederAddRangeAsync(List<TEntity> entities, bool isSaveChanges = true);
    }
}
