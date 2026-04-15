using System.ComponentModel.DataAnnotations;
namespace QuantityMeasurementApp.Models.DTO
{
    // UC15 - i created this DTO class to pass data between layers
    // the console app or future API will create a QuantityDTO and send it to the service layer
    // service layer reads the Value, Unit and MeasurementType from it

    // i used strings for Unit and MeasurementType instead of actual unit objects
    // because when we move to REST API in future, the request body will have strings
    // it's easier to deserialize JSON strings than complex objects

    // this is a POCO — no methods, no logic, just data
    public class QuantityDTO
    {
        [Required]
        public double Value { get;}
        [Required]
        public string Unit { get;}

        // tells which category this is — "Length", "Weight", "Volume" or "Temperature"
        [RegularExpression("Length|Weight|Volume|Temperature")]
        public string MeasurementType { get; }

        public QuantityDTO(double value, string unit, string measurementType)
        {
            Value = value;
            Unit = unit;
            MeasurementType = measurementType;
        }

        public override string ToString()
        {
            return $"{Value} {Unit} ({MeasurementType})";
        }
    }
}