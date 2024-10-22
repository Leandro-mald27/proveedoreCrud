namespace proveedoresCrud.models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(T data, string message = null)
        {
            Success = true;
            Message = message ?? "Request successful";
            Data = data;
        }
        public ApiResponse(string message, bool success = false)
        {
            Success = success;
            Message = message;
            Data = default;
        }
    }
}
