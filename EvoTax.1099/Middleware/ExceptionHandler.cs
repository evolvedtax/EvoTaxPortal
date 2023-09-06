using Newtonsoft.Json;
using System.Net;

namespace EvolvedTax.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly string _logFilePath;

        public ExceptionHandler(RequestDelegate next, IWebHostEnvironment hostingEnvironment)
        {
            _next = next;
            _hostingEnvironment = hostingEnvironment;
            _logFilePath = Path.Combine(_hostingEnvironment.ContentRootPath, "exceptions.txt");
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception to a text file
            LogExceptionToFile(exception);

            int statusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.StatusCode = statusCode;

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                IsSuccessful = false,
                Response = exception.Message ?? "Something went wrong. Please try again",
                StatusCode = statusCode
            }));
        }

        private void LogExceptionToFile(Exception exception)
        {
            string logMessage = $"[{DateTime.Now}] Exception: {exception.Message}\nStack Trace: {exception.StackTrace}\n";

            // Create the log file if it doesn't exist
            if (!File.Exists(_logFilePath))
            {
                using (var file = File.CreateText(_logFilePath))
                {
                    file.WriteLine(logMessage);
                }
            }
            else
            {
                // Append the log message to the existing file
                File.AppendAllText(_logFilePath, logMessage);
            }
        }
    }
}
