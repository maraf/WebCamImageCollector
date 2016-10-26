using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace WebCamImageCollector.Background.Http
{
    internal interface IHttpHandler
    {
        Task<bool> TryHandleAsync(HttpRequest request, HttpResponse response);
    }
}
