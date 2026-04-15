using QuantityMeasurementApp.Models.Interfaces;

namespace QuantityMeasurementApp.Models.Enums
{
    // UC10 - same wrapper class pattern as LengthUnit
    // base unit is Kilogram here

    // UC15 - added GetMeasurementType() and GetByName() same as LengthUnit
    public class WeightUnit : IMeasurable
    {
        private readonly double _conversionFactor;
        private readonly string _unitName;

        public WeightUnit(double conversionFactor, string unitName)
        {
            _conversionFactor = conversionFactor;
            _unitName = unitName;
        }

        // base unit is Kilogram, so Kilogram has factor 1.0
        // Static instances — replaces enum constants
        public static readonly WeightUnit Kilogram = new WeightUnit(1.0, "Kilogram");
        public static readonly WeightUnit Gram = new WeightUnit(0.001, "Gram");

        public static readonly WeightUnit Pound = new WeightUnit(0.453592, "Pound");
        public double ConvertToBaseUnit(double value) => value * _conversionFactor;
        
        /// <summary>
        /// Converts a value from base unit (feet) to this unit.
        /// Formula: baseValue / conversionFactor
        /// </summary>

        public double ConvertFromBaseUnit(double baseValue) => baseValue / _conversionFactor;
        public string GetUnitName() => _unitName;

        // UC15 - tells the service layer this unit belongs to Weight category
        public string GetMeasurementType() => "Weight";

        // UC15 - converts string unit name from QuantityDTO to actual WeightUnit instance
        public static WeightUnit GetByName(string name)
        {
            switch (name.ToLower())
            {
                case "kilogram": return Kilogram;
                case "gram": return Gram;
                
                
                case "pound": return Pound;
                default: throw new ArgumentException($"Unknown weight unit: {name}");
            }
        }

        public override string ToString() => _unitName;
    }
}