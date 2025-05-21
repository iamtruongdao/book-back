using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Exceptions;

namespace BackEnd.Middleware
{
    public class ErrorMiddleware(ILogger<ErrorMiddleware> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
             catch (UnAuthorizeException ex)
            {
                logger.LogError(ex.Message);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
             catch (NotFoundException ex)
            {
                logger.LogError(ex.Message);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
               catch (BadRequestException ex)
            {
                logger.LogError(ex.Message);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                logger.LogError(ex.Message);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { message = "Internal Server Error" });
            }
        }
    }
}