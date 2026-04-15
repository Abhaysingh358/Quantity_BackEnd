using QuantityMeasurementApp.Models.Interfaces;
namespace QuantityMeasurementApp.Models.Enums
{
    public class TemperatureUnit : IMeasurable
    {
        /// <summary>
         // UC14 - temperature is different from other units because conversion is not linear
        /// for length you just multiply or divide by a factor
        /// but for temperature you need a formula like (F - 32) * 5/9 for Fahrenheit to Celsius
        /// so i can't use conversionFactor approach here, i used lambda functions instead
 
        /// i also needed to block Add, Subtract, Divide for temperature
        /// because adding 100C + 50C = 150C doesn't make physical sense
        /// so i overrode SupportsArithmetic() and ValidateOperationSupport() from IMeasurable
 
        /// UC15 - added GetMeasurementType() and GetByName() same as other unit classes
        /// </summary>
        private readonly string _unitName;

        /// <summary>
        /// Function<double, double> (System.Func<double, double>): 
        /// This is a delegate representing a method that takes a double as input and returns a double as output. 
        /// 
        /* Example usage:
        private readonly Func<double, double> Celsius = (f) => (f - 32) * 5 / 9;
        Later in the class, you can use it:
        double celsiusTemp = Celsius(98.6);  */
        /// </summary>
        /// 
        private Func<double , double> _toCelsius;

        private readonly Func<double, double> _fromCelsius;

        /// <summary>
        /// UC14 - this lambda is used by SupportsArithmetic()
        /// all temperature units return false because arithmetic doesn't make sense for temperature
        /// </summary>
        private readonly Func<bool> _supportsArithmetic = () => false;

        public  TemperatureUnit (string unitName , Func<double,double> toCelsius ,Func<double,double> fromCelsius)
        {
             _unitName = unitName;
            _toCelsius = toCelsius;
            _fromCelsius = fromCelsius;
        }


        /// <summary>
        /// This code is a static field initialization for a class called TemperatureUnit.
        /// It creates a predefined "Celsius" object that knows how to convert values to and from a "base" unit
        /// </summary>
        public static readonly TemperatureUnit Celsius = new TemperatureUnit("Celsius" ,celsius => celsius , celsius => celsius); 

        // Fahrenheit conversion formulas
        // to Celsius: (F - 32) * 5 / 9
        // from Celsius: C * 9 / 5 + 32
        public static readonly TemperatureUnit Fahrenheit = new TemperatureUnit(
            "Fahrenheit",
            fahrenheit => (fahrenheit - 32) * 5.0 / 9.0,
            celsius => celsius * 9.0 / 5.0 + 32.0);
 
        // Kelvin conversion formulas
        // to Celsius: K - 273.15
        // from Celsius: C + 273.15
        public static readonly TemperatureUnit Kelvin = new TemperatureUnit(
            "Kelvin",
            kelvin => kelvin - 273.15,
            celsius => celsius + 273.15);

        // calls the stored lambda to convert to Celsius
        public double ConvertToBaseUnit(double value) => _toCelsius(value);

        // calls the stored lambda to convert from Celsius
        public double ConvertFromBaseUnit(double baseValue) => _fromCelsius(baseValue);

        public string GetUnitName() => _unitName;

        // UC15 - tells the service layer this unit belongs to Temperature category
        public string GetMeasurementType() => "Temperature";

         public double GetConversionFactor() => 1.0;

        // UC14 - overrides the default from IMeasurable, returns false for all temperature units
         public bool SupportsArithmetic() => _supportsArithmetic();

        // UC14 - overrides the default no-op from IMeasurable
        // if someone tries to Add, Subtract or Divide temperature this will throw
        // the error message explains clearly what went wrong
        public void ValidateOperationSupport(string operation)
        {
            throw new InvalidOperationException(
                $"Temperature does not support {operation}. " +
                $"Only equality comparison and unit conversion are supported.");
        }

        // UC15 - converts string unit name from QuantityDTO to actual TemperatureUnit instance
        public static TemperatureUnit GetByName(string name)
        {
            switch (name.ToLower())
            {
                case "celsius": return Celsius;
                case "fahrenheit": return Fahrenheit;
                case "kelvin": return Kelvin;
                default: throw new ArgumentException($"Unknown temperature unit: {name}");
            }
        }
 
        public override string ToString() => _unitName;
    }
}