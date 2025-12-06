using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.SuperClasses
{
	public class inBaseAnyRequest
	{
		[Required] public int Id_application { get; set; }
		[Required] public int Id_user { get; set; }
		[Required] public string Token { get; set; }
	}
}