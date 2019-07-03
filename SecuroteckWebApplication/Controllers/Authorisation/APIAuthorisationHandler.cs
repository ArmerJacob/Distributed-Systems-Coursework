using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SecuroteckWebApplication.Models;

namespace SecuroteckWebApplication.Controllers
{
    public class APIAuthorisationHandler : DelegatingHandler
    {
        UserDatabaseAccess userDatabase = new UserDatabaseAccess();
        protected override Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then authorise the principle on the current thread using a claim, claimidentity and claimsprinciple
            IEnumerable<string> key;
            request.Headers.TryGetValues("x-api-key", out key);
            if (key != null)
            {
                User user = userDatabase.GetUser(key.First());

                Claim claimName = new Claim(ClaimTypes.Name, user.UserName);
                Claim claimRole = new Claim(ClaimTypes.Role, user.Role);
                ClaimsIdentity claimKey = new ClaimsIdentity(user.ApiKey);

                claimKey.AddClaim(claimName);
                claimKey.AddClaim(claimRole);

                ClaimsPrincipal principal = new ClaimsPrincipal(claimKey);

                Thread.CurrentPrincipal = principal;
            }
                #endregion
                return base.SendAsync(request, cancellationToken);
            
        }
    }
}