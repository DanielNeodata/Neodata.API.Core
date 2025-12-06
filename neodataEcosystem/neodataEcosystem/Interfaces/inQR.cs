using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inQR
	{
		[Required] public string? Url { get; set; }
	}
	public class inBarCode
	{
		[Required] public string? Text { get; set; }
	}
}