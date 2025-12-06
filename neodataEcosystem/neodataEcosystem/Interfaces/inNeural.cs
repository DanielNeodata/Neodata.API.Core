using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inNeural : inBaseAnyRequest
	{
		[Required] public int Id_project { get; set; }
		public int? Id_item { get; set; }
		public int? Id_question { get; set; }
		public string? Base64RawData { get; set; }
	}
}