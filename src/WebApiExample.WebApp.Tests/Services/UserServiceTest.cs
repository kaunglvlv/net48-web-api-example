using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore.Models;
using WebApiExample.WebApp.Services;
using Xunit;

namespace WebApiExample.WebApp.Tests.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork> _uow;
        private readonly UserService _sut;

        public UserServiceTest()
        {
            _uow = new Mock<IUnitOfWork>();
            _sut = new UserService(_uow.Object);
        }

        [Fact]
        public async Task AddNewUserAsync_Adds_A_New_User()
        {
            var name = "John";
            var age = 27;
            var newUser = new User();

            _uow
                .Setup(u => u.Add(
                    It.Is<User>(user => user.Name == name && user.Age == age)))
                .Returns(newUser);

            var result = await _sut.AddUserAsync(name, age);

            _uow.Verify(u => u.CommitAsync());
            result.ShouldBeSameAs(newUser);
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("sa")]
        public async Task AddNewUserAsync_Throws_When_Using_Banned_Names(string bannedName)
        {
            await _sut.AddUserAsync(bannedName, 27).ShouldThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddNewUserAsync_Db_Exception_Bubbles_Up()
        {
            _uow
                .Setup(u => u.CommitAsync())
                .ThrowsAsync(new InvalidOperationException());

            await _sut.AddUserAsync("John", 21).ShouldThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetAllUsersAsync_Returns_Users()
        {
            _uow.Setup(u => u.GetAll<User>())
                .Returns(
                    new List<User>
                    {
                        new User(),
                        new User(),
                        new User(),
                    }.AsQueryable());

            var users = await _sut.GetAllUsersAsync();

            users.Count().ShouldBe(3);
        }

        [Fact]
        public async Task GetAllUsersAsync_Db_Exception_Bubbles_Up()
        {
            _uow.Setup(u => u.GetAll<User>())
                .Throws(new InvalidOperationException());

            await _sut.GetAllUsersAsync().ShouldThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task GetUserAsync_Returns_User()
        {
            var id = Guid.NewGuid();
            var user = new User();

            _uow.Setup(u => u.GetAsync<User>(It.Is<Guid>(curId => curId == id)))
                .ReturnsAsync(user);

            var result = await _sut.GetUserAsync(id);

            result.ShouldBeSameAs(user);
        }

        [Fact]
        public async Task GetUserAsync_Returns_Null_When_Not_Found()
        {
            _uow.Setup(u => u.GetAsync<User>(It.IsAny<Guid>()))
                .ReturnsAsync(default(User));

            var result = await _sut.GetUserAsync(Guid.NewGuid());

            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetUserAsyn_Db_Exception_Bubbles_Up()
        {
            _uow.Setup(u => u.GetAsync<User>(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException());

            await _sut.GetUserAsync(Guid.NewGuid()).ShouldThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RemoveAsync_Removes_Existing_User()
        {
            var id = Guid.NewGuid();
            var user = new User();

            _uow.Setup(u => u.GetAsync<User>(It.Is<Guid>(curId => curId == id)))
                .ReturnsAsync(user);

            await _sut.RemoveAsync(id);

            _uow.Verify(u => u.Remove(It.Is<User>(curUser => curUser.Equals(user))));
            _uow.Verify(u => u.CommitAsync());
        }

        [Fact]
        public async Task RemoveAsync_Returns_If_User_Not_Found()
        {
            var id = Guid.NewGuid();

            await _sut.RemoveAsync(id);

            _uow.Verify(u => u.GetAsync<User>(It.Is<Guid>(curId => curId == id)));
            _uow.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task RemoveAsync_Get_User_Db_Exception_Bubbles_Up()
        {
            _uow.Setup(u => u.GetAsync<User>(It.IsAny<Guid>()))
                .ThrowsAsync(new InvalidOperationException());

            await _sut.RemoveAsync(Guid.NewGuid()).ShouldThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RemoveAsync_Commit_Db_Exception_Bubbles_Up()
        {
            _uow.Setup(u => u.GetAsync<User>(It.IsAny<Guid>()))
                .ReturnsAsync(new User());
            _uow.Setup(u => u.CommitAsync())
                .ThrowsAsync(new InvalidOperationException());

            await _sut.RemoveAsync(Guid.NewGuid()).ShouldThrowAsync<InvalidOperationException>();
        }
    }
}
