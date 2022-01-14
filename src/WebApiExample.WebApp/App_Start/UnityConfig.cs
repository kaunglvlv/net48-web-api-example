using System.Web.Http;
using Unity;
using Unity.WebApi;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore;
using WebApiExample.WebApp.Services;

namespace WebApiExample.WebApp
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container
                .RegisterType<IUserService, UserService>()
                .RegisterType<IUnitOfWork, UserContext>()
                ;
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}