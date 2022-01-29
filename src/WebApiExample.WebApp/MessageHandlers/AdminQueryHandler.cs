using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiExample.WebApp.MessageHandlers
{
    public class AdminQueryHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var query = request
                .GetQueryNameValuePairs()
                .ToList()
                .ToDictionary(x => x.Key, x => x.Value);

            var db = query.ContainsKey("db") ? query["db"] : string.Empty;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, db),
            };

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims));

            Thread.CurrentPrincipal = principal;

            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;

            return base.SendAsync(request, cancellationToken);
        }
    }
}