namespace SeaQuail.DiagramTool.Domain.Shared.Exceptions
{
    public class ServiceException : Exception
    {
        public enum Reasons { NotFound, InvalidRequest, NotPermitted, Unknown, NoIdentity, Internal }

        public Reasons Reason { get; }
           
        public ServiceException(Reasons reason, string? message, Exception? innerEx = null) : base(message, innerEx)
        {
            Reason = reason;
        }

        public static ServiceException NotFound(string? message = null, Exception? innerEx = null)
            => new(Reasons.NotFound, message, innerEx);
        public static ServiceException InvalidRequest(string? message = null, Exception? innerEx = null)
            => new(Reasons.InvalidRequest, message, innerEx);
        public static ServiceException NotPermitted(string? message = null, Exception? innerEx = null)
            => new(Reasons.NotPermitted, message, innerEx);
        public static ServiceException Unknown(string? message = null, Exception? innerEx = null)
            => new(Reasons.Unknown, message, innerEx);
        public static ServiceException NoIdentity(string? message = null, Exception? innerEx = null)
            => new(Reasons.Unknown, message, innerEx);
        public static ServiceException Internal(string? message = null, Exception? innerEx = null)
            => new(Reasons.Unknown, message, innerEx);
    }
}
