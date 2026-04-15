using System.ComponentModel.DataAnnotations;
namespace QuantityMeasurementApp.Models.DTO;

public class ConvertRequestDTO
{
    [Required]
    public QuantityDTO Input { get; set; } = null!;

    [Required]
    public string TargetUnit { get; set; } = null!;
}
