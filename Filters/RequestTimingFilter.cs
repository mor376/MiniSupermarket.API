using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace MiniSupermarket.API.Filters
{
    // Action filter to measure execution time and add header
    public class RequestTimingFilter : ActionFilterAttribute
    {
        private const string StopwatchKey = "__request_stopwatch";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sw = Stopwatch.StartNew();
            context.HttpContext.Items[StopwatchKey] = sw;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.HttpContext.Items[StopwatchKey] is Stopwatch sw)
            {
                sw.Stop();
                var ms = sw.ElapsedMilliseconds;
                context.HttpContext.Response.Headers.Add("X-Execution-Time-ms", ms.ToString());
                // simple console log
                System.Console.WriteLine($"[RequestTiming] {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} executed in {ms} ms");
            }
        }
    }
}
