using Nexter.Fintech.Core;
using System;
using System.ComponentModel;

namespace Nexter.Fintech.Web.Core
{
	public class BusinessViolation : Exception
	{

        public BusinessViolation(BusinessViolationStatusCodes statusCode,
                                 params object[] parameters)
            :base(string.Format(statusCode.GetDescription(), parameters ?? new object[]{}))
        {
            StatusCode = statusCode.ToString();
        }

        public BusinessViolation(BusinessViolationStatusCodes statusCode,
                                 object[] parameters = null,
                                 Exception innerException = null,
                                 bool needRetry = false,
                                 DateTime? retryTime = null)
        :base(string.Format(statusCode.GetDescription(), parameters ?? new object[]{}), 
              innerException)
        {
            StatusCode = statusCode.ToString();
            NeedRetry = needRetry;
            RetryTime = retryTime;
        }

		public BusinessViolation(string message = null,
                                 string statusCode = null,
                                 string statusReason = null,
                                 Exception innerException = null,
                                 bool needRetry = false, 
                                 DateTime? retryTime = null) 
            : base(message, innerException)
		{
			StatusCode = statusCode ?? BusinessViolationStatusCodes.RuleViolated.ToString();
			StatusReason = statusReason;
            NeedRetry = needRetry;
            RetryTime = retryTime;
        }
		public string StatusCode { get; set; }
		public string StatusReason { get; set; }
        /// <summary>
        /// 是否需要重试执行 (当前仅消息队列消费者使用)
        /// </summary>
        public bool NeedRetry { get; set; }
        /// <summary>
        /// 当需要重试时, 该时间决定何时重试(消息队列根据此时间重新发送消息)
        /// </summary>
        public DateTime? RetryTime { get; set; }
	}
    public enum StatusCode
    {
        Unauthorized = 401,
        InternalServerError = 500
    }

    public enum BusinessViolationStatusCodes
	{
        [Description("RuleViolated")]
		RuleViolated = 1,
        [Description("{0} can not be found.")]
        NotFound
    }
}
