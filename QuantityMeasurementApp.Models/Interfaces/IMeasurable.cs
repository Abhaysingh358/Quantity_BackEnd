namespace QuantityMeasurementApp.Models.Interfaces
{
    // UC10 - i created this interface so that Quantity<T> class can work with any unit type
    // instead of making separate classes for length, weight etc.
    // every unit class like LengthUnit, WeightUnit must implement this

    // UC14 - i added two default methods here so that TemperatureUnit can block arithmetic
    // the good thing about default methods is LengthUnit, WeightUnit, VolumeUnit
    // don't need to change anything — they just inherit the default behavior

    // UC15 - i added GetMeasurementType() so the service layer can check
    // if both quantities belong to same category before doing any operation
    // for example you can't add Length + Weight, so we check the type first
    public interface IMeasurable
    {
        // converts value from this unit to the base unit
        // example: 12 Inch -> 1 Feet (because Feet is the base unit for length)
        double ConvertToBaseUnit(double value);

        // converts back from base unit to this unit
        // example: 1 Feet-> 12 Inch
        double ConvertFromBaseUnit(double value);

        // just returns the name like "Feet", "Kilogram", "Celsius"
        string GetUnitName();

        // UC15 - returns which category this unit belongs to
        // LengthUnit returns "Length", WeightUnit returns "Weight" and so on
        // i need this in service layer to stop cross category operations
        string GetMeasurementType();

        // UC14 - by default every unit supports arithmetic so this returns true
        // only TemperatureUnit overrides this and returns false
        bool SupportsArithmetic() => true;

        // UC14 - by default this does nothing for length, weight, volume
        // TemperatureUnit overrides this to throw InvalidOperationException
        // this gets called inside PerformBaseArithmetic before doing any math
        void ValidateOperationSupport(string operation) { }
    }
}