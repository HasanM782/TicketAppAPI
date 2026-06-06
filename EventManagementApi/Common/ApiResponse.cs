namespace EventManagementApi.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success") => new()
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data
        };

        public static ApiResponse<T> BadRequest(string message = "Bad request") => new()
        {
            Success = false,
            StatusCode = 400,
            Message = message,
            Data = default
        };

        public static ApiResponse<T> NotFound(string message = "Not found") => new()
        {
            Success = false,
            StatusCode = 404,
            Message = message,
            Data = default
        };

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized") => new()
        {
            Success = false,
            StatusCode = 401,
            Message = message,
            Data = default
        };

        public static ApiResponse<T> Forbidden(string message = "Forbidden") => new()
        {
            Success = false,
            StatusCode = 403,
            Message = message,
            Data = default
        };

        public static ApiResponse<T> ServerError(string message = "Internal server error") => new()
        {
            Success = false,
            StatusCode = 500,
            Message = message,
            Data = default
        };
    }
}