namespace QuantityMeasurementApp.Models.Exceptions
{
    // UC15 - this custom exception showso all errors from service layer
    // are wrapped in one consistent exception type
    // instead of letting ArgumentException, InvalidOperationException etc bubble up
    // the service catches those and throws this instead

    // i extended Exception (not Exception with checked handling like Java's checked exceptions)
    // in C# all exceptions are unchecked so you don't have to declare throws
    // second constructor is useful when you want to keep the original exception as inner exception
    // that way the full stack trace is preserved for debugging
    public class QuantityMeasurementException : Exception
    {
        public QuantityMeasurementException(string message)
            : base(message)
        {
        }

        // used when wrapping another exception — keeps original error info
        public QuantityMeasurementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}