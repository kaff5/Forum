using Forum.Exceptions;


namespace Forum.Middleware
{
	public static class MiddlewareExtensions
	{
		public static void UseExceptionHandlingMiddlewares(this WebApplication app)
		{
			app.UseMiddleware<ExceptionMiddleware>();
		}
	}


	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (ValidationException exception)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;
				await context.Response.WriteAsJsonAsync(new { message = exception.Message });
			}
			catch (ObjectNotFoundException exception)
			{
				context.Response.StatusCode = StatusCodes.Status404NotFound;
				await context.Response.WriteAsJsonAsync(new { message = exception.Message });
			}
			catch (NotPermissionException exception)
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				await context.Response.WriteAsJsonAsync(new { message = exception.Message });
			}
			catch (Exception)
			{
				context.Response.StatusCode = StatusCodes.Status500InternalServerError;
				await context.Response.WriteAsJsonAsync(new { message = "Внутренняя ошибка сервера" });
			}
		}
	}
}