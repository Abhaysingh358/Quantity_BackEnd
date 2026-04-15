using QuantityMeasurementApp.Models.Interfaces;

namespace QuantityMeasurementApp.Models.Enums
{
    // UC10 - it is  changed  from enum to a wrapper class so it can implement IMeasurable
    // enums can't have methods so wrapper class pattern was the right approach here
    // private constructor means no one can create a LengthUnit from outside
    // only the static instances like Feet, Inch etc are accessible

    // UC15 - i added GetMeasurementType() and GetByName() here
    // GetMeasurementType() tells the service layer this is a Length unit
    // GetByName() is used by service layer to convert QuantityDTO string to actual unit instance

      /// <summary>
    /// Represents a length unit with conversion responsibility.
    /// Implements IMeasurable interface — Single Responsibility Principle.
    /// Base unit is Feet.
    /// UC10 — Wrapper class pattern replaces enum to allow interface implementation.
    /// </summary>
    public class LengthUnit : IMeasurable
    {
        private readonly double _conversionFactor;
        private readonly string _unitName;

        public LengthUnit(double conversionFactor, string unitName)
        {
            _conversionFactor = conversionFactor;
            _unitName = unitName;
        }

        // base unit is Feet, so Feet has factor 1.0
        // all other units are defined relative to Feet
        public static readonly LengthUnit Feet = new LengthUnit(1.0, "Feet");
        public static readonly LengthUnit Inch = new LengthUnit(1.0 / 12.0, "Inch");
        public static readonly LengthUnit Yard = new LengthUnit(3.0, "Yard");
        public static readonly LengthUnit Centimeter = new LengthUnit(0.0328084, "Centimeter");
        public static readonly LengthUnit Meter = new LengthUnit(3.28084, "Meter");

        // multiply by factor to get Feet (base unit)
        public double ConvertToBaseUnit(double value) => value * _conversionFactor;

        // divide by factor to convert from Feet back to this unit
        public double ConvertFromBaseUnit(double baseValue) => baseValue / _conversionFactor;

        public string GetUnitName() => _unitName;

        // UC15 - tells the service layer this unit belongs to Length category
        public string GetMeasurementType() => "Length";

        // UC15 - the service layer sends unit name as string from QuantityDTO
        // this method converts that string back to the actual LengthUnit instance
        // throws ArgumentException if someone passes an unknown unit name
        public static LengthUnit GetByName(string name)
        {
            switch (name.ToLower())
            {
                case "feet": return Feet;
                case "inch": return Inch;
                case "yard": return Yard;
                case "centimeter": return Centimeter;
                case "meter": return Meter;
                default: throw new ArgumentException($"Unknown length unit: {name}");
            }
        }

        public override string ToString() => _unitName;
    }
}