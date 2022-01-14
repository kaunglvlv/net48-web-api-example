using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore.Models;

namespace WebApiExample.WebApp.Services
{
    public interface IUserService
    {
        Task<User> AddUserAsync(string name, int age);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserAsync(Guid id);
        Task RemoveAsync(Guid id);
    }

    public class UserService : IUserService
    {
        private readonly IUnitOfWork _db;
        private readonly IDictionary<string, string> _bannedNames;

        public UserService(IUnitOfWork db)
        {
            _db = db;
            _bannedNames = new Dictionary<string, string>
            {
                ["admin"] = "admin",
                ["sa"] = "sa",
            };
        }

        public async Task<User> AddUserAsync(string name, int age)
        {
            if (_bannedNames.ContainsKey(name))
                throw new ArgumentException($"The name {name} is not allowed");

            var user = new User
            {
                Name = name,
                Age = age,
            };

            var newUser = _db.Add(user);

            await _db.CommitAsync();

            return newUser;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await Task.Run(() => _db.GetAll<User>());
            return users;
        }

        public Task<User> GetUserAsync(Guid id)
        {
            return _db.GetAsync<User>(id);
        }

        public async Task RemoveAsync(Guid id)
        {
            var existingUser = await _db.GetAsync<User>(id);
            if (existingUser == null)
                return;

            _db.Remove(existingUser);

            await _db.CommitAsync();
        }
    }
}
