namespace Infrastructure.Helpers
{
    public class ResponseError
    {
        public ResponseError(int? errorTypeCode, string message, string errorDetails = null)
        {
            ErrorCode = errorTypeCode ?? 0;
            Message = message;
            Message = errorDetails;
        }
        public ResponseError(string message, string errorDetails = null)
        {
            Message = message;
            DetailMessage = errorDetails;
        }
        public string Message { get; set; }
        public string DetailMessage { get; set; }
        public int ErrorCode { get; set; }
        public override string ToString()
        {
            return Message;
        }
    }
}