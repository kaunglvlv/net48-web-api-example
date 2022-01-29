using System.Web.Http;
using Unity;
using Unity.WebApi;
using WebApiExample.Common.DataAccess;
using WebApiExample.WebApp.Services;

namespace WebApiExample.WebApp
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container
                .RegisterType<IUnitOfWorkFactory, ClaimsUnitOfWorkFactory>()
                .RegisterType<IUserService, UserService>()
                .RegisterFactory<IUnitOfWork>(c => c
                    .Resolve<IUnitOfWorkFactory>()
                    .Create())
                ;
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}