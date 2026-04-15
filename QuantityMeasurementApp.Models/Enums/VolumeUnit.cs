using QuantityMeasurementApp.Models.Interfaces;

namespace QuantityMeasurementApp.Models.Enums
{
    // UC11 - added VolumeUnit with same wrapper class pattern as LengthUnit and WeightUnit
    // base unit is Litre here

    // UC15 - added GetMeasurementType() and GetByName() same as other unit classes
    public class VolumeUnit : IMeasurable
    {
        private readonly double _conversionFactor;
        private readonly string _unitName;

        public  VolumeUnit(double conversionFactor, string unitName)
        {
            _conversionFactor = conversionFactor;
            _unitName = unitName;
        }

        // base unit is Litre, so Litre has factor 1.0
        public static readonly VolumeUnit Litre = new VolumeUnit(1.0, "Litre");
        public static readonly VolumeUnit Millilitre = new VolumeUnit(0.001, "Millilitre");
        public static readonly VolumeUnit Gallon = new VolumeUnit(3.78541, "Gallon");

        public double ConvertToBaseUnit(double value) => value * _conversionFactor;
        public double ConvertFromBaseUnit(double baseValue) => baseValue / _conversionFactor;
        public string GetUnitName() => _unitName;

        // UC15 - tells the service layer this unit belongs to Volume category
        public string GetMeasurementType() => "Volume";

        // UC15 - converts string unit name from QuantityDTO to actual VolumeUnit instance
        public static VolumeUnit GetByName(string name)
        {
            switch (name.ToLower())
            {
                case "litre": return Litre;
                case "millilitre": return Millilitre;
                case "gallon": return Gallon;
                default: throw new ArgumentException($"Unknown volume unit: {name}");
            }
        }

        public override string ToString() => _unitName;
    }
}