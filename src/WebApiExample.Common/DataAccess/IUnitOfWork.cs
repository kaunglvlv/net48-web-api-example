using System;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiExample.Common.DataAccess
{
    public interface IUnitOfWork
    {
        T Add<T>(T entity) where T : class;
        IQueryable<T> GetAll<T>() where T : class;
        Task<T> GetAsync<T>(Guid id) where T : class, IRootEntity;
        void Remove<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        Task CommitAsync();
    }
}
