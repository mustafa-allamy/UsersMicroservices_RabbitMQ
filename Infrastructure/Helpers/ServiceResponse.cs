namespace Infrastructure.Helpers
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T value)
        {
            Value = value;
        }
        public ServiceResponse(T value, int totalCount)
        {
            Value = value;
            TotalCount = totalCount;
        }
        public T Value { get; set; }
        public string ResponseMessage { get; set; }
        public ResponseError Error { get; set; }
        public int TotalCount { get; set; }
        public void SetSuccessResponse(T value, int totalCount = 1, string msg = null)
        {
            ResponseMessage = msg;
            Value = value;
            TotalCount = totalCount;
        }
    }
}