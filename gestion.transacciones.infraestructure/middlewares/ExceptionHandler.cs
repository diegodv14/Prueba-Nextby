using gestion.transacciones.domain.exceptions;
using gestion.transacciones.domain.response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace gestion.transacciones.infraestructure.middlewares
{
    public static class ExceptionHandler
    {
        public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    int statusCode = 500;
                    string message = "Ocurrió un error inesperado.";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    if (exception != null)
                    {
                        if (exception is BaseCustomException customException)
                        {
                            statusCode = customException.Code;
                            message = customException.Message;
                        }

                        if (exception is Exception Exception)
                        {
                            statusCode = 500;
                            message = Exception.Message;
                        }
                    }
                    var response = new ErrorResponse
                    {
                        Code = statusCode,
                        Message = message,
                        Error = true,
                    };

                    var jsonResponse = JsonSerializer.Serialize(response);
                    context.Response.StatusCode = statusCode;
                    await context.Response.WriteAsync(jsonResponse);
                });
            });
            return app;
        }
    }
}
