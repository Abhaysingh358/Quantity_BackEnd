namespace QuantityMeasurementApp.Models.Entities
{
    // This is NOT your domain entity (QuantityMeasurementEntity).
    // This is a simple flat class that EF Core maps to the DB table.
    //
    // Why a separate class?
    // Your domain QuantityMeasurementEntity has readonly properties and complex
    // constructors — EF Core needs a class with a parameterless constructor and
    // settable properties to do its mapping magic.
    //
    // The repository will convert between the two.

    public class MeasurementHistoryRecord
    {
        // Primary key — EF Core auto-increments this (IDENTITY column in SQL Server)
        public int Id { get; set; }

        // FK to Users table — null means record was saved without a logged-in user
        public int? UserId { get; set; }
        public User? User { get; set; }   // EF Core navigation property

        // Operation name: "Compare", "Convert", "Add", "Subtract", "Divide"
        public string Operation { get; set; } = string.Empty;

        //  Operand 1 (always present) 
        public double Operand1Value { get; set; }
        public string Operand1Unit { get; set; } = string.Empty;
        public string Operand1MeasurementType { get; set; } = string.Empty;

        //  Operand 2 (null for single-operand ops like Convert) 
        public double? Operand2Value { get; set; }
        public string? Operand2Unit { get; set; }
        public string? Operand2MeasurementType { get; set; }

        // Result type tag
        // "Quantity" | "Scalar" | "Comparison" | "Error"
        public string ResultType { get; set; } = string.Empty;

        //  Quantity result (Add / Subtract / Convert) 
        public double? ResultQuantityValue { get; set; }
        public string? ResultQuantityUnit { get; set; }
        public string? ResultQuantityMeasurementType { get; set; }

        //  Scalar result (Divide) 
        public double? ResultScalar { get; set; }

        //  Comparison result (Compare) 
        public bool? ResultComparison { get; set; }

        //  Error 
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }

        // Timestamp set automatically in the repository before saving
        // Must use UtcNow — PostgreSQL "timestamp with time zone" requires UTC
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}