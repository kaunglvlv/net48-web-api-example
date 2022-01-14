using System;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiExample.DataStore.Models;
using WebApiExample.WebApp.Models;
using WebApiExample.WebApp.Services;

namespace WebApiExample.WebApp.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return base.Ok(users);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.InternalServerError();
            }
        }

        public async Task<IHttpActionResult> Get(Guid id)
        {
            try
            {
                var user = await _userService.GetUserAsync(id);
                if (user == null)
                    return base.NotFound();

                return base.Ok(user);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.InternalServerError();
            }
        }

        public async Task<IHttpActionResult> Post([FromBody] NewUser newUser)
        {
            try
            {
                if (!base.ModelState.IsValid)
                    return base.BadRequest("Name and age are required");

                var user = await _userService.AddUserAsync(newUser.Name, newUser.Age);

                return base.Created("/api/users", user);
            }
            catch (ArgumentException ex)
            {
                // Log
                return base.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.InternalServerError();
            }
        }

        public async Task<IHttpActionResult> Delete(Guid id)
        {
            try
            {
                await _userService.RemoveAsync(id);
                return base.ResponseMessage(
                    new System.Net.Http.HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.NoContent,
                    });
            }
            catch (Exception ex)
            {
                // Should handle in ExceptionFilter
                return base.InternalServerError();
            }
        }
    }
}