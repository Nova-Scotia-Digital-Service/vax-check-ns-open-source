using System;
namespace VaxCheckNS.Rules.Support
{
    public class Result<T>
    {
        protected Result(bool Success, bool Retryable, T Message)
        {
            this.Success = Success;
            this.Message = Message;
            this.Retryable = Retryable;
        }

        /// <summary>
        /// The action was Successful and the result value property will be set. 
        /// </summary>
        public bool Success { get; private set; }
        /// <summary>
        /// Indicates that the operation failed and no Result value can be returned. Refer the ErrorMessage for more detail.   
        /// </summary>
        public bool Failure => !Success;
        /// <summary>
        /// Indicate that this result believes the operation could be retried again. 
        /// It is a decision of the caller to check Retryable or not, checking Success or Failure is usualy enough.
        /// /// If Retryable is True then Success SHALL BE False and Failure SHALL BE True 
        /// If Success is True then Retryable will be False
        /// </summary>    
        public bool Retryable { get; private set; }
        /// <summary>
        /// An message that gives details on the result.
        /// </summary>
        public T Message { get; private set; }

        public static Result<T> Ok(T message)
        {
            return new Result<T>(Success: true, Retryable: false, Message: message);
        }

        public static Result<T> Fail(T message)
        {
            return new Result<T>(Success: false, Retryable: false, Message: message);
        }
    }
}
