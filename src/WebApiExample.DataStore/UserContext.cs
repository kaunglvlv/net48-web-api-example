using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore.Models;

namespace WebApiExample.DataStore
{
    public class UserContext : DbContext, IUnitOfWork
    {
        public UserContext()
        {
            Database.SetInitializer(
                new CreateDatabaseIfNotExists<UserContext>());
        }

        #region DbSets

        public DbSet<User> Users { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<User>()
                .Property(u => u.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        public T Add<T>(T entity) where T : class
        {
            return base.Set<T>().Add(entity);
        }

        public Task CommitAsync()
        {
            return base.SaveChangesAsync();
        }

        public IQueryable<T> GetAll<T>() where T : class
        {
            return base.Set<T>();
        }

        public Task<T> GetAsync<T>(Guid id) where T : class, IRootEntity
        {
            return this.GetAll<T>().FirstOrDefaultAsync(e => e.Id == id);
        }

        public void Remove<T>(T entity) where T : class
        {
            base.Set<T>().Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            base.Entry(entity).State = EntityState.Modified;
        }
    }
}
