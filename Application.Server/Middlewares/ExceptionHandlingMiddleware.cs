using Application.Model;
using System.Net;
using System.Text.Json;

namespace Application.Server.Middlewares
{
    /// <summary>
    /// Класс Middleware для глобально обработки ошибок
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        /// <summary>
        /// Функция обрабатывающая HTTP запрос
        /// </summary>
        private readonly RequestDelegate _next;  
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Метод отлавливающий необработанные исключения
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //вызываем RequestDelegate, чтобы не прервать цепочку вызовов
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAync(httpContext, ex.Message, HttpStatusCode.InternalServerError, "Iternal servert error");
            }
        }

        private async Task HandleExceptionAync(
            HttpContext httpContext,
            string exMsg,
            HttpStatusCode httpStatusCode,
            string message)
        {
            _logger.LogError(exMsg);

            HttpResponse response = httpContext.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;

            ResponseMessage responseMessage = new()
            {
                Message = message,
                StatusCode = (int)httpStatusCode
            };

            string result = JsonSerializer.Serialize(responseMessage);

            await response.WriteAsJsonAsync(result);
        }
    }
}
