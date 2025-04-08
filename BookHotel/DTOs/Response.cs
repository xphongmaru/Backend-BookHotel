namespace BookHotel.DTOs
{
    public class Response
    {

    }
    public class ApiResponse
    {
        public bool Success { get; set; }
        public object Response { get; set; }
        public ErrorResponse Error { get; set; }

        public ApiResponse(bool success, object response, ErrorResponse error)
        {
            Success = success;
            Response = response;
            Error = error;
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public int Status { get; set; }

        public ErrorResponse(string message, int status)
        {
            Message = message;
            Status = status;
        }
    }
}
