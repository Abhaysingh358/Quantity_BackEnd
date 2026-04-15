namespace QuantityMeasurementApp.Models.Exceptions
{
    public class DatabaseException : Exception
    {
        public int? ErrorCode { get; }

        public DatabaseException(string message)
            : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DatabaseException(string message, int errorCode, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}