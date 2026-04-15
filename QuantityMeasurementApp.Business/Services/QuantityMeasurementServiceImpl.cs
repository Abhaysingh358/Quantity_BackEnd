using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Models.DTO;
using QuantityMeasurementApp.Models.Entities;
using QuantityMeasurementApp.Repositories.Interfaces;
using QuantityMeasurementApp.Models.Enums;
using QuantityMeasurementApp.Models.Exceptions;
using QuantityMeasurementApp.Models.Interfaces;

namespace QuantityMeasurementApp.Business.Services
{
    public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
    {
        private readonly IQuantityMeasurementRepository _repository;

        public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository), "Repository cannot be null");

            _repository = repository;
        }

        // COMPARE
        public bool Compare(QuantityDTO dto1, QuantityDTO dto2, int? userId = null)
        {
            ValidateNotNull(dto1, "First quantity");
            ValidateNotNull(dto2, "Second quantity");

            try
            {
                if (!string.Equals(dto1.MeasurementType, dto2.MeasurementType, StringComparison.OrdinalIgnoreCase))
                    return false;

                IMeasurable unit1 = ResolveUnit(dto1);
                IMeasurable unit2 = ResolveUnit(dto2);

                double base1 = unit1.ConvertToBaseUnit(dto1.Value);
                double base2 = unit2.ConvertToBaseUnit(dto2.Value);

                bool result = Math.Abs(base1 - base2) < 0.0001;

                _repository.Save(new QuantityMeasurementEntity(dto1, dto2, "Compare", result), userId);

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException($"Compare failed: {ex.Message}", ex);
            }
        }

        // CONVERT
        public QuantityDTO Convert(QuantityDTO dto, string targetUnit, int? userId = null)
        {
            ValidateNotNull(dto, "Input quantity");

            if (string.IsNullOrWhiteSpace(targetUnit))
                throw new QuantityMeasurementException("Target unit cannot be empty");

            try
            {
                IMeasurable sourceUnit = ResolveUnit(dto);
                IMeasurable targetUnitInstance = ResolveUnitByTypeAndName(dto.MeasurementType, targetUnit);

                double baseValue = sourceUnit.ConvertToBaseUnit(dto.Value);
                double converted = Math.Round(targetUnitInstance.ConvertFromBaseUnit(baseValue), 5);

                QuantityDTO result = new QuantityDTO(converted, targetUnit, dto.MeasurementType);

                _repository.Save(new QuantityMeasurementEntity(dto, "Convert", result), userId);

                return result;
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException($"Convert failed: {ex.Message}", ex);
            }
        }

        // ADD
        public QuantityDTO Add(QuantityDTO dto1, QuantityDTO dto2, int? userId = null)
        {
            ValidateNotNull(dto1, "First quantity");
            ValidateNotNull(dto2, "Second quantity");
            ValidateSameCategory(dto1, dto2, "Add");

            try
            {
                IMeasurable unit1 = ResolveUnit(dto1);
                IMeasurable unit2 = ResolveUnit(dto2);

                unit1.ValidateOperationSupport("Add");

                double base1 = unit1.ConvertToBaseUnit(dto1.Value);
                double base2 = unit2.ConvertToBaseUnit(dto2.Value);
                double baseResult = base1 + base2;

                QuantityDTO result = new QuantityDTO(
                    Math.Round(unit1.ConvertFromBaseUnit(baseResult), 5),
                    dto1.Unit,
                    dto1.MeasurementType);

                _repository.Save(new QuantityMeasurementEntity(dto1, dto2, "Add", result), userId);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException($"Add failed: {ex.Message}", ex);
            }
        }

        // SUBTRACT
        public QuantityDTO Subtract(QuantityDTO dto1, QuantityDTO dto2, int? userId = null)
        {
            ValidateNotNull(dto1, "First quantity");
            ValidateNotNull(dto2, "Second quantity");
            ValidateSameCategory(dto1, dto2, "Subtract");

            try
            {
                IMeasurable unit1 = ResolveUnit(dto1);
                IMeasurable unit2 = ResolveUnit(dto2);

                unit1.ValidateOperationSupport("Subtract");

                double base1 = unit1.ConvertToBaseUnit(dto1.Value);
                double base2 = unit2.ConvertToBaseUnit(dto2.Value);
                double baseResult = base1 - base2;

                QuantityDTO result = new QuantityDTO(
                    Math.Round(unit1.ConvertFromBaseUnit(baseResult), 5),
                    dto1.Unit,
                    dto1.MeasurementType);

                _repository.Save(new QuantityMeasurementEntity(dto1, dto2, "Subtract", result), userId);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException($"Subtract failed: {ex.Message}", ex);
            }
        }

        // DIVIDE
        public double Divide(QuantityDTO dto1, QuantityDTO dto2, int? userId = null)
        {
            ValidateNotNull(dto1, "First quantity");
            ValidateNotNull(dto2, "Second quantity");
            ValidateSameCategory(dto1, dto2, "Divide");

            try
            {
                IMeasurable unit1 = ResolveUnit(dto1);
                IMeasurable unit2 = ResolveUnit(dto2);

                unit1.ValidateOperationSupport("Divide");

                double base1 = unit1.ConvertToBaseUnit(dto1.Value);
                double base2 = unit2.ConvertToBaseUnit(dto2.Value);

                if (base2 == 0)
                    throw new QuantityMeasurementException("Cannot divide by zero quantity");

                double result = base1 / base2;

                _repository.Save(new QuantityMeasurementEntity(dto1, dto2, "Divide", result), userId);

                return result;
            }
            catch (InvalidOperationException ex)
            {
                throw new QuantityMeasurementException(ex.Message, ex);
            }
            catch (QuantityMeasurementException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new QuantityMeasurementException($"Divide failed: {ex.Message}", ex);
            }
        }

        // PRIVATE HELPERS
        private IMeasurable ResolveUnit(QuantityDTO dto)
        {
            return ResolveUnitByTypeAndName(dto.MeasurementType, dto.Unit);
        }

        private IMeasurable ResolveUnitByTypeAndName(string measurementType, string unitName)
        {
            switch (measurementType.ToLower())
            {
                case "length":      return LengthUnit.GetByName(unitName);
                case "weight":      return WeightUnit.GetByName(unitName);
                case "volume":      return VolumeUnit.GetByName(unitName);
                case "temperature": return TemperatureUnit.GetByName(unitName);
                default:
                    throw new QuantityMeasurementException($"Unknown measurement type: {measurementType}");
            }
        }

        private void ValidateNotNull(QuantityDTO dto, string name)
        {
            if (dto == null)
                throw new QuantityMeasurementException($"{name} cannot be null");
        }

        private void ValidateSameCategory(QuantityDTO dto1, QuantityDTO dto2, string operation)
        {
            if (!string.Equals(dto1.MeasurementType, dto2.MeasurementType, StringComparison.OrdinalIgnoreCase))
                throw new QuantityMeasurementException(
                    $"Cannot {operation} across different categories: " +
                    $"{dto1.MeasurementType} and {dto2.MeasurementType}");
        }
    }
}