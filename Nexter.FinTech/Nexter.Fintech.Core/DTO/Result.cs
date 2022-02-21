namespace Nexter.Fintech.Core
{
    public class Result<T> : Result
    {
        public T Data { get; set; }
    }

    public class Result
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }

        public static Result Complete()
        {
            return new Result()
            {
                StatusCode = "Ok"
            };
        }

        public static Result<T> Complete<T>(T data)
        {
            return new Result<T>
            {
                Data = data,
                StatusCode = "Ok"
            };
        }
        public static Result Fail(string statusCode, string message = null)
        {
            return new Result
            {
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}