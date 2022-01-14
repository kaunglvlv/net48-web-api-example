using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using WebApiExample.DataStore.Models;
using WebApiExample.WebApp.Controllers;
using WebApiExample.WebApp.Services;
using Xunit;

namespace WebApiExample.WebApp.Tests.Controllers
{
    public class UsersControllerTest
    {
        private readonly Mock<IUserService> _userService;
        private readonly UsersController _sut;

        public UsersControllerTest()
        {
            _userService = new Mock<IUserService>();
            _sut = new UsersController(_userService.Object);
        }

        [Fact]
        public async Task Get_All_Returns_200()
        {
            _userService
                .Setup(u => u.GetAllUsersAsync())
                .ReturnsAsync(new List<User>
                {
                    new User(),
                    new User(),
                    new User(),
                });

            var result = await _sut.Get() as OkNegotiatedContentResult<IEnumerable<User>>;

            result.ShouldNotBeNull();
            result.Content.Count().ShouldBe(3);
        }

        [Fact]
        public async Task Get_All_Returns_500()
        {
            _userService.Setup(u => u.GetAllUsersAsync())
                .ThrowsAsync(new Exception());

            var result = await _sut.Get();

            result.ShouldBeOfType<InternalServerErrorResult>();
        }

        [Fact]
        public async Task Get_By_Id_Returns_200()
        {
            var id = Guid.NewGuid();
            var user = new User();

            _userService
                .Setup(u => u.GetUserAsync(It.Is<Guid>(curId => curId == id)))
                .ReturnsAsync(user);

            var result = await _sut.Get(id) as OkNegotiatedContentResult<User>;

            result.ShouldNotBeNull();
            result.Content.ShouldBeSameAs(user);
        }

        [Fact]
        public async Task Get_By_Id_Returns_404()
        {
            var id = Guid.NewGuid();

            _userService
                .Setup(u => u.GetUserAsync(It.Is<Guid>(curId => curId == id)))
                .ReturnsAsync(new User());

            var result = await _sut.Get(Guid.NewGuid());

            result.ShouldBeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Get_By_Id_Returns_500()
        {
            _userService
                .Setup(u => u.GetUserAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception());

            var result = await _sut.Get(Guid.NewGuid());

            result.ShouldBeOfType<InternalServerErrorResult>();
        }

        [Fact]
        public async Task Post_New_User_Returns_201()
        {
            var user = new User
            {
                Name = "John Doe",
                Age = 27,
            };

            _userService
                .Setup(u => u.AddUserAsync(
                    It.Is<string>(name => name == user.Name),
                    It.Is<int>(age => age == user.Age)))
                .ReturnsAsync(user);

            var result = await _sut.Post(
                new Models.NewUser
                {
                    Name = user.Name,
                    Age = user.Age,
                }) as CreatedNegotiatedContentResult<User>;

            result.ShouldNotBeNull();
            result.Content.ShouldBeSameAs(user);
        }

        [Fact]
        public async Task Post_New_User_With_Banned_Name_Returns_400()
        {
            var message = "Error: Name is banned!";

            // Just mock to throw the error as if it detected the banned name
            _userService
                .Setup(u => u.AddUserAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .ThrowsAsync(new ArgumentException(message));

            var result = await _sut.Post(
                new Models.NewUser
                {
                    Name = "admin",
                    Age = 21,
                }) as BadRequestErrorMessageResult;

            result.ShouldNotBeNull();
            result.Message.ShouldBe(message);
        }

        [Fact]
        public async Task Post_New_User_Returns_500()
        {
            _userService
                .Setup(u => u.AddUserAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            var result = await _sut.Post(
                new Models.NewUser
                {
                    Name = "John",
                    Age = 37
                });

            result.ShouldBeOfType<InternalServerErrorResult>();
        }

        [Fact]
        public async Task Delete_User_Returns_204()
        {
            var id = Guid.NewGuid();

            var result = await _sut.Delete(id) as ResponseMessageResult;

            _userService.Verify(u => u.RemoveAsync(It.Is<Guid>(curId => curId == id)));

            result.ShouldNotBeNull();
            result.Response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task Delete_User_Returns_500()
        {
            _userService
                .Setup(u => u.RemoveAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception());

            var result = await _sut.Delete(Guid.NewGuid());

            result.ShouldBeOfType<InternalServerErrorResult>();
        }
    }
}
