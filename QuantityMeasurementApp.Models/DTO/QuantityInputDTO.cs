using System.ComponentModel.DataAnnotations;
namespace QuantityMeasurementApp.Models.DTO;

public class QuantityInputDTO
{
    [Required]
    public QuantityDTO First { get; set; } = null!;

    [Required]
    public QuantityDTO Second { get; set; } = null!;
}
// This class is created because API can only take one body
