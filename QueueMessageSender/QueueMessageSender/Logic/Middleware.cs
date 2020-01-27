using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public class Middleware
    {
        private readonly RequestDelegate _next;

        public Middleware(RequestDelegate next, IQueueMessageSender sender)
        {
            _next = next;
            IQueueMessageSender _sender = sender;
        }

        public Task Invoke(HttpContext httpContext)
        {
            return _next(httpContext);
        }
    }
}
