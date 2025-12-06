using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
    public class inPages
    {
        [Required] public string? Base64 { get; set; }
        [Required] public string? Filename { get; set; }
    }
}