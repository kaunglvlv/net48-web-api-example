using System.Configuration;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web;
using Unity;
using Unity.Resolution;
using WebApiExample.Common.DataAccess;
using WebApiExample.DataStore;

namespace WebApiExample.WebApp.Services
{
    public class ClaimsUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IUnityContainer _container;

        public ClaimsUnitOfWorkFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IUnitOfWork Create()
        {
            var connStrName = nameof(UserContext);
            var connectionString = ConfigurationManager.ConnectionStrings[connStrName]?.ConnectionString;
            var principal = (ClaimsPrincipal)HttpContext.Current.User;
            var customerName = principal.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(customerName))
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                builder.InitialCatalog = customerName;

                connectionString = builder.ToString();
            }

            return _container.Resolve<UserContext>(
                new DependencyOverride<string>(connectionString));
        }
    }
}