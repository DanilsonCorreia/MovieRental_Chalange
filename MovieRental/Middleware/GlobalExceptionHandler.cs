using System.Net;
using System.Text.Json;

namespace MovieRental.Middleware
{
	public class GlobalExceptionHandler
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unhandled exception occurred");
				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			
			var response = new
			{
				Message = "An error occurred while processing your request.",
				Details = exception.Message,
				Timestamp = DateTime.UtcNow
			};

			switch (exception)
			{
				case ArgumentException:
					context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
					response = new { Message = "Invalid input provided.", Details = exception.Message, Timestamp = DateTime.UtcNow };
					break;
					
				case InvalidOperationException:
					context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
					response = new { Message = "Invalid operation.", Details = exception.Message, Timestamp = DateTime.UtcNow };
					break;
					
				case UnauthorizedAccessException:
					context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
					response = new { Message = "Access denied.", Details = exception.Message, Timestamp = DateTime.UtcNow };
					break;
					
				default:
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					response = new { Message = "An unexpected error occurred.", Details = "Please try again later.", Timestamp = DateTime.UtcNow };
					break;
			}

			var jsonResponse = JsonSerializer.Serialize(response);
			await context.Response.WriteAsync(jsonResponse);
		}
	}
} 