using QuantityMeasurementApp.Business.Interfaces;
using QuantityMeasurementApp.Models.DTO;
using QuantityMeasurementApp.Models.Exceptions;

namespace QuantityMeasurementApp.Application.Services
{
    internal class Menu
    {
        private readonly IQuantityMeasurementService _service;

        public Menu(IQuantityMeasurementService service)
        {
            _service = service;
        }

        internal void Start()
        {
            Console.WriteLine("====================================");
            Console.WriteLine(" Quantity Measurement Application  ");
            Console.WriteLine("====================================");

            while (true)
            {
                ShowOptions();

                string choice = Console.ReadLine()?.Trim() ?? string.Empty;
                Console.WriteLine();

                switch (choice)
                {
                    case "1": RunCompare(); break;
                    case "2": RunConvert(); break;
                    case "3": RunAdd(); break;
                    case "4": RunSubtract(); break;
                    case "5": RunDivide(); break;
                    case "6": Console.WriteLine("Thank You"); return;
                    default: Console.WriteLine("  Invalid option. Please enter 1 to 6."); break;
                }
            }
        }

        private void ShowOptions()
        {
            Console.WriteLine("\n----------Menu--------------------------");
            Console.WriteLine("  1. Compare two quantities");
            Console.WriteLine("  2. Convert a quantity");
            Console.WriteLine("  3. Add two quantities");
            Console.WriteLine("  4. Subtract two quantities");
            Console.WriteLine("  5. Divide two quantities");
            Console.WriteLine("  6. Exit");
            Console.WriteLine("---------------------------------------------");
            Console.Write("  Choose an option: ");
        }

        private void RunCompare()
        {
            try
            {
                Console.WriteLine("  --- Compare Two Quantities ---");
                QuantityDTO dto1 = ReadQuantity("  First quantity");
                QuantityDTO dto2 = ReadQuantity("  Second quantity");

                bool result = _service.Compare(dto1, dto2);
                if(result)
                {
                 Console.WriteLine("\nResult: " + dto1 + " == " + dto2);
                }
                else
                {
                    Console.WriteLine("\nResult: " + dto1 + " != " + dto2);
                }
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"\n  [ERROR] {ex.Message}");
            }
        }

        private void RunConvert()
        {
            try
            {
                Console.WriteLine("  --- Convert a Quantity ---");
                QuantityDTO dto = ReadQuantity("  Quantity to convert");

                Console.Write("  Target unit (e.g. Inch, Gram, Fahrenheit): ");
                string targetUnit = Console.ReadLine()?.Trim() ?? string.Empty;

                QuantityDTO result = _service.Convert(dto, targetUnit);
                Console.WriteLine($"\n  Result: {dto} → {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"\n  [ERROR] {ex.Message}");
            }
        }

        private void RunAdd()
        {
            try
            {
                Console.WriteLine("  --- Add Two Quantities ---");
                QuantityDTO dto1 = ReadQuantity("  First quantity");
                QuantityDTO dto2 = ReadQuantity("  Second quantity");

                QuantityDTO result = _service.Add(dto1, dto2);
                Console.WriteLine($"\n  Result: {dto1} + {dto2} = {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"\n  [ERROR] {ex.Message}");
            }
        }

        private void RunSubtract()
        {
            try
            {
                Console.WriteLine("  --- Subtract Two Quantities ---");
                QuantityDTO dto1 = ReadQuantity("  First quantity");
                QuantityDTO dto2 = ReadQuantity("  Second quantity");

                QuantityDTO result = _service.Subtract(dto1, dto2);
                Console.WriteLine($"\n  Result: {dto1} - {dto2} = {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"\n  [ERROR] {ex.Message}");
            }
        }

        private void RunDivide()
        {
            try
            {
                Console.WriteLine("  --- Divide Two Quantities ---");
                QuantityDTO dto1 = ReadQuantity("  First quantity");
                QuantityDTO dto2 = ReadQuantity("  Second quantity");

                double result = _service.Divide(dto1, dto2);
                Console.WriteLine($"\n  Result: {dto1} / {dto2} = {result}");
            }
            catch (QuantityMeasurementException ex)
            {
                Console.WriteLine($"\n  [ERROR] {ex.Message}");
            }
        }

        private QuantityDTO ReadQuantity(string operation)
        {
            Console.WriteLine($"\n{operation}:");

            Console.WriteLine("  Measurement type (Length / Weight / Volume / Temperature):");
            Console.Write(" --> ");
            string measurementType = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.WriteLine("  Unit ( Feet, Inch, Kilogram, Gram, Litre, Celsius ...):");
            Console.Write("  -> ");
            string unit = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.WriteLine("  Value:");
            Console.Write("  -> ");
            string valueInput = Console.ReadLine()?.Trim() ?? "0";
            double value = double.TryParse(valueInput, out double result) ? result : 0;

            return new QuantityDTO(value, unit, measurementType);
        }
    }
}
