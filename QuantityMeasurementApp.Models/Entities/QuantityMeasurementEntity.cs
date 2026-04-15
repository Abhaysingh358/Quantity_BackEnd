using QuantityMeasurementApp.Models.DTO;

namespace QuantityMeasurementApp.Models.Entities
{
    // UC15 - created this entity class to store history of every operation
    // right now we are not saving it anywhere because repo layer is not implemented yet
    // but the service layer creates this entity after every operation
    // when repo layer comes in future, we just pass this entity to repo.Save()
    // and nothing else will change — that's whole point of designing it now

    // i made multiple constructors because different operations have different results
    // convert has single operand, add/subtract has two operands with quantity result
    // divide has two operands but result is a scalar (just a number, no unit)
    // compare has two operands but result is true or false
    // and error case is separate constructor when something goes wrong

    // i didn't make fields final/readonly even though it should be immutable
    // because if we serialize this in future, serialization doesn't work with all readonly fields

    public class QuantityMeasurementEntity
    {
        public QuantityDTO Operand1 { get; }
        public QuantityDTO Operand2 { get; }
        public string Operation { get; }
        public QuantityDTO ResultQuantity { get; }
        public double ResultScalar { get; }
        public bool ResultComparison { get; }
        public bool IsError { get; }
        public string ErrorMessage { get; }

        // for single operand operations like Convert
        // example: 100 Celsius -> 212 Fahrenheit
        public QuantityMeasurementEntity(QuantityDTO operand1, string operation, QuantityDTO result)
        {
            Operand1 = operand1;
            Operation = operation;
            ResultQuantity = result;
            IsError = false;
        }

        // for two operand operations like Add and Subtract
        // result is still a QuantityDTO because it has a unit
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, string operation, QuantityDTO result)
        {
            Operand1 = operand1;
            Operand2 = operand2;
            Operation = operation;
            ResultQuantity = result;
            IsError = false;
        }

        // for Divide — result is just a number, no unit (dimensionless scalar)
        // example: 10 Feet / 2 Feet = 5 (no unit)
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, string operation, double scalarResult)
        {
            Operand1 = operand1;
            Operand2 = operand2;
            Operation = operation;
            ResultScalar = scalarResult;
            IsError = false;
        }

        // for Compare — result is just true or false
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, string operation, bool comparisonResult)
        {
            Operand1 = operand1;
            Operand2 = operand2;
            Operation = operation;
            ResultComparison = comparisonResult;
            IsError = false;
        }

        // for error cases — when something throws an exception
        // we store error message so it can be logged or shown to user later
        public QuantityMeasurementEntity(QuantityDTO operand1, QuantityDTO operand2, string operation, string errorMessage, bool isError)
        {
            Operand1 = operand1;
            Operand2 = operand2;
            Operation = operation;
            ErrorMessage = errorMessage;
            IsError = isError;
        }

        // Convenience constructor for single operand operations with string representations
        // example: new QuantityMeasurementEntity("Convert", "1 Feet", null, "12 Inches")
        public QuantityMeasurementEntity(string operation, string firstOperand, string secondOperand, string result)
        {
            Operation = operation;
            Operand1 = ParseQuantityFromString(firstOperand);
            Operand2 = ParseQuantityFromString(secondOperand);
            ResultQuantity = ParseQuantityFromString(result);
            IsError = false;
        }

        // Convenience constructor for error cases with string representation
        // example: new QuantityMeasurementEntity("Invalid Operation")
        public QuantityMeasurementEntity(string errorMessage)
        {
            Operation = "Error";
            ErrorMessage = errorMessage;
            IsError = true;
        }

        // Helper method to parse string representation of quantity into QuantityDTO
        private QuantityDTO ParseQuantityFromString(string quantityString)
        {
            if (string.IsNullOrEmpty(quantityString))
                return null;

            // Parse format: "value unit" e.g., "1 Feet", "12 Inches"
            string[] parts = quantityString.Trim().Split(new[] { ' ' }, 2);
            if (parts.Length < 2)
                return null;

            if (double.TryParse(parts[0], out double value))
            {
                string unit = parts[1];
                // Determine measurement type based on unit name
                string measurementType = GetMeasurementTypeFromUnit(unit);
                return new QuantityDTO(value, unit, measurementType);
            }

            return null;
        }

        // Helper method to determine measurement type from unit
        private string GetMeasurementTypeFromUnit(string unit)
        {
            // Length units
            if (new[] { "Feet", "Inch", "Yard", "Centimeter", "Kilometer", "Meter" }.Contains(unit))
                return "Length";
            
            // Weight units  
            if (new[] { "Kilogram", "Gram", "Pound", "Ounce" }.Contains(unit))
                return "Weight";
            
            // Volume units
            if (new[] { "Litre", "Gallon", "Millilitre" }.Contains(unit))
                return "Volume";
            
            // Temperature units
            if (new[] { "Celsius", "Fahrenheit", "Kelvin" }.Contains(unit))
                return "Temperature";
            
            // Default fallback
            return "Unknown";
        }

        public override string ToString()
        {
            if (IsError)
                return $"[ERROR] {Operation}: {ErrorMessage}";

            if (Operation == "Compare")
                return $"{Operation}: {Operand1} == {Operand2} --> {ResultComparison}";

            if (Operation == "Divide")
                return $"{Operation}: {Operand1} / {Operand2} = {ResultScalar}";

            if (Operand2 != null)
                return $"{Operation}: {Operand1} and {Operand2} --> {ResultQuantity}";

            return $"{Operation}: {Operand1} --> {ResultQuantity}";
        }
    }
}