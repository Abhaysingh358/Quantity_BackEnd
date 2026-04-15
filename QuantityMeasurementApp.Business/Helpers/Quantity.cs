using QuantityMeasurementApp.Models.Interfaces;

namespace QuantityMeasurementApp.Business.Helpers
{
    // UC10 - made this class generic so one class can handle all unit types
    // Quantity<LengthUnit>, Quantity<WeightUnit> etc all work with same code
    // the where T : IMeasurable constraint means T must implement IMeasurable
    // so only our unit classes can be used here, nothing random

    // UC12 - added Subtract and Divide operations here

    // UC13 - refactored Add, Subtract, Divide to remove code duplication
    // earlier each method had its own validation and arithmetic logic
    // now i have ValidateArithmeticOperands() and PerformBaseArithmetic() as private helpers
    // all three operations call these helpers — DRY principle

    // UC14 - added ValidateOperationSupport() call inside PerformBaseArithmetic
    // this is how temperature blocks arithmetic — it throws from its override

    // UC15 - moved this class from Core project to Business/Helpers
    // it's marked internal so it's only visible inside Business project
    // outside world only sees QuantityDTO — not this class
    internal class Quantity<T> where T : IMeasurable
    {
        // UC13 - private enum to represent which operation to perform
        // pass this to PerformBaseArithmetic instead of duplicating the switch logic
        // UC14 - operation.ToString() is passed to ValidateOperationSupport for the error message
        private enum ArithmeticOperation
        {
            Add,
            Subtract,
            Divide
        }

        private readonly double _value;
        private readonly T _unit;

        internal Quantity(double value, T unit)
        {
            if (unit == null)
                throw new ArgumentNullException(nameof(unit), "Unit cannot be null");

            if (!double.IsFinite(value))
                throw new ArgumentException($"Value must be finite: {value}");

            _value = value;
            _unit = unit;
        }

        // converts this quantity's value to the base unit
        // example: 12 Inch -> 1 Feet
        private double ToBaseValue() => _unit.ConvertToBaseUnit(_value);

        // UC13 - Step 2: i moved all validation into one place
        // every arithmetic method calls this first so i don't repeat the same checks
        private void ValidateArithmeticOperands(Quantity<T> other, T targetUnit, bool targetUnitRequired)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Operand cannot be null");

            // you can't add LengthUnit and WeightUnit — different types
            if (_unit.GetType() != other._unit.GetType())
                throw new ArgumentException(
                    $"Cannot operate on different measurement categories: " +
                    $"{_unit.GetType().Name} and {other._unit.GetType().Name}");

            if (!double.IsFinite(other._value))
                throw new ArgumentException($"Operand value must be finite: {other._value}");

            if (targetUnitRequired && targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit), "Target unit cannot be null");
        }

        // UC13 - Step 3: this is the core arithmetic helper
        // both operands are converted to base unit first, then operation is applied
        // result is returned as raw base unit value — caller converts it back

        // UC14 - ValidateOperationSupport is called first
        // for temperature this throws InvalidOperationException immediately
        // for other units it's a no-op and everything continues normally
        private double PerformBaseArithmetic(Quantity<T> other, ArithmeticOperation operation)
        {
            // UC14 - this call stops temperature arithmetic before any math happens
            _unit.ValidateOperationSupport(operation.ToString());

            double thisBase = ToBaseValue();
            double otherBase = other.ToBaseValue();

            switch (operation)
            {
                case ArithmeticOperation.Add:
                    return thisBase + otherBase;
                case ArithmeticOperation.Subtract:
                    return thisBase - otherBase;
                case ArithmeticOperation.Divide:
                    if (otherBase == 0)
                        throw new ArithmeticException("Cannot divide by zero quantity");
                    return thisBase / otherBase;
                default:
                    throw new ArgumentException($"Unsupported operation: {operation}");
            }
        }

        // converts this quantity to a different unit
        // UC14 - this works for temperature too because the lambda handles non-linear formula
        // no special case needed — ConvertToBaseUnit and ConvertFromBaseUnit do the right thing
        internal Quantity<T> ConvertTo(T targetUnit)
        {
            if (targetUnit == null)
                throw new ArgumentNullException(nameof(targetUnit));

            double baseValue = ToBaseValue();
            double converted = targetUnit.ConvertFromBaseUnit(baseValue);
            int decimalPlaces = targetUnit.GetUnitName() == "Yard" || targetUnit.GetUnitName() == "Inch" ? 2 : 5;
            return new Quantity<T>(Math.Round(converted, decimalPlaces), targetUnit);
        }

        // UC13 - Step 4: Add with implicit target unit (result is in first operand's unit)
        internal Quantity<T> Add(Quantity<T> other)
        {
            ValidateArithmeticOperands(other, _unit, false);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Add);
            double result = _unit.ConvertFromBaseUnit(baseResult);
            int decimalPlaces = _unit.GetUnitName() == "Yard" || _unit.GetUnitName() == "Inch" ? 2 : 5;
            return new Quantity<T>(Math.Round(result, decimalPlaces), _unit);
        }

        // UC13 - Step 4: Add with explicit target unit
        internal Quantity<T> Add(Quantity<T> other, T targetUnit)
        {
            ValidateArithmeticOperands(other, targetUnit, true);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Add);
            double result = targetUnit.ConvertFromBaseUnit(baseResult);
            int decimalPlaces = targetUnit.GetUnitName() == "Yard" || targetUnit.GetUnitName() == "Inch" ? 2 : 5;
            return new Quantity<T>(Math.Round(result, decimalPlaces), targetUnit);
        }

        // UC13 - Step 4: Subtract with implicit target unit
        internal Quantity<T> Subtract(Quantity<T> other)
        {
            ValidateArithmeticOperands(other, _unit, false);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Subtract);
            double result = _unit.ConvertFromBaseUnit(baseResult);
            int decimalPlaces = _unit.GetUnitName() == "Yard" || _unit.GetUnitName() == "Inch" ? 2 : 5;
            return new Quantity<T>(Math.Round(result, decimalPlaces), _unit);
        }

        // UC13 - Step 4: Subtract with explicit target unit
        internal Quantity<T> Subtract(Quantity<T> other, T targetUnit)
        {
            ValidateArithmeticOperands(other, targetUnit, true);
            double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.Subtract);
            double result = targetUnit.ConvertFromBaseUnit(baseResult);
            int decimalPlaces = targetUnit.GetUnitName() == "Yard" || targetUnit.GetUnitName() == "Inch" ? 2 : 5;
            return new Quantity<T>(Math.Round(result, decimalPlaces), targetUnit);
        }

        // UC13 - Step 4: Divide returns a dimensionless scalar — no unit on the result
        internal double Divide(Quantity<T> other)
        {
            ValidateArithmeticOperands(other, default, false);
            return PerformBaseArithmetic(other, ArithmeticOperation.Divide);
        }

        // compared by converting both to base unit and checking difference is within epsilon
        // using 0.0001 as tolerance to handle floating point precision issues
        public override bool Equals(object? obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            Quantity<T> other = (Quantity<T>)obj;
            return Math.Abs(ToBaseValue() - other.ToBaseValue()) < 0.0001;
        }

        public override int GetHashCode() => ToBaseValue().GetHashCode();

        public override string ToString() => $"{_value} {_unit.GetUnitName()}";

        internal double GetValue() => _value;
        internal T GetUnit() => _unit;
    }
}