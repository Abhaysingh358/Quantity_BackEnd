using QuantityMeasurementApp.Models.DTO;

namespace QuantityMeasurementApp.Business.Interfaces
{
    public interface IQuantityMeasurementService
    {
        /// <summary>
        /// Compares two quantities and returns true if equal.
        /// Example: 1 Feet == 12 Inch
        /// </summary>
        bool Compare(QuantityDTO first, QuantityDTO second, int? userId = null);

        /// <summary>
        /// Converts a quantity to a target unit within the same category.
        /// </summary>
        QuantityDTO Convert(QuantityDTO quantity, string targetUnit, int? userId = null);

        /// <summary>
        /// Adds two quantities. Result is returned in unit of first operand.
        /// </summary>
        QuantityDTO Add(QuantityDTO first, QuantityDTO second, int? userId = null);

        /// <summary>
        /// Subtracts second quantity from first.
        /// </summary>
        QuantityDTO Subtract(QuantityDTO first, QuantityDTO second, int? userId = null);

        /// <summary>
        /// Divides first quantity by second and returns scalar value.
        /// </summary>
        double Divide(QuantityDTO first, QuantityDTO second, int? userId = null);
    }
}