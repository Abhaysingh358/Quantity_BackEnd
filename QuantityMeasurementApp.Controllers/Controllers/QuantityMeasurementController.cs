// this controller is in used with application project meanwhile in ASP.Net another controller is working

using QuantityMeasurementApp.Models.DTO;
using QuantityMeasurementApp.Business.Interfaces;

namespace QuantityMeasurementApp.Controllers.Controllers
{
    public class QuantityMeasurementController
    {
        private readonly IQuantityMeasurementService _service;

        public QuantityMeasurementController(IQuantityMeasurementService service)
        {
            _service = service;
        }

        public bool Compare(QuantityDTO first, QuantityDTO second)
        {
            if (first == null || second == null)
                throw new ArgumentNullException("Input quantities cannot be null.");

            return _service.Compare(first, second);
        }

        public QuantityDTO Convert(QuantityDTO input, string targetUnit)
        {
            return _service.Convert(input, targetUnit);
        }

        public QuantityDTO Add(QuantityDTO first, QuantityDTO second)
        {
            return _service.Add(first, second);
        }

        public QuantityDTO Subtract(QuantityDTO first, QuantityDTO second)
        {
            return _service.Subtract(first, second);
        }

        public double Divide(QuantityDTO first, QuantityDTO second)
        {
            return _service.Divide(first, second);
        }
    }
}