using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCamImageCollector.Http
{
    internal class AuthenticationHttpHandler : IHttpHandler
    {
        private readonly string authenticationToken;
        private readonly IHttpHandler inner;

        public AuthenticationHttpHandler(string authenticationToken, IHttpHandler inner)
        {
            this.authenticationToken = authenticationToken;
            this.inner = inner;
        }

        public Task<bool> TryHandleAsync(HttpRequest request, HttpResponse response)
        {
            string authenticationToken = request.Headers["X-Authentication-Token"];
            if (String.IsNullOrEmpty(authenticationToken) || this.authenticationToken != authenticationToken)
            {
                response.StatusCode = 401;
                return Task.FromResult(true);
            }

            return inner.TryHandleAsync(request, response);
        }
    }
}
