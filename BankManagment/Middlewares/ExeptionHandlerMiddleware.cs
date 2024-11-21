using Core.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace BankManagment.Middlewares;

public class ExeptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private const string _contentType = "application/json";

    public ExeptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException sqlEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = _contentType;
            var Error = new ErrorModel
            {
                Message = sqlEx.InnerException?.InnerException?.Message ??
                    "Ocurrió un error inesperado en la base de datos."
            };

            var ErrorJson = JsonSerializer.Serialize(Error);
            await context.Response.WriteAsync(ErrorJson);
        }
        catch (FluentValidation.ValidationException validEx)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = _contentType;

            var errorValidationModel = new ErrorValidationModel
            {
                Message = validEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    )
            };

            var errorJson = JsonSerializer.Serialize(errorValidationModel);
            await context.Response.WriteAsync(errorJson);
        }

        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = _contentType;
            var Error = new ErrorModel
            {
                Message = ex.Message
            };

            var ErrorJson = JsonSerializer.Serialize(Error);
            await context.Response.WriteAsync(ErrorJson);
        }
    }
}