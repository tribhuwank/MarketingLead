namespace MarketingLead.API.Wrappers
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(bool succeed, string message, Exception errors, T data)
        {
            Succeeded = succeed;
            Message = message;
            Errors = errors;
            Data = data;
        }

        public Response(T data)
        {
            Succeeded = true;
            Message = string.Empty;
            Data = data;
        }
        public T? Data { get; set; }
        public bool Succeeded { get; set; } = false;
        public Exception? Errors { get; set; } 
        public string Message { get; set; } = string.Empty;

    }
}
