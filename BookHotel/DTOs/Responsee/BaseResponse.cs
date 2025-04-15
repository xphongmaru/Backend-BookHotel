namespace BookHotel.Responsee
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public T Response { get; set; }
        public ErrorResponse Error { get; set; }

        public BaseResponse(T response)
        {
            Success = true;
            Response = response;
            Error = null;
        }

        public BaseResponse(string message, int statusCode)
        {
            Success = false;
            Response = default!;
            Error = new ErrorResponse { Message = message, Status = statusCode };
        }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public int Status { get; set; }
    }

}


