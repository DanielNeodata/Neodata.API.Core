using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inLogs : inBaseAnyRequest
	{
		[Required] public int Id_type_log { get; set; }
		[Required] public string? Action { get; set; }
		[Required] public string? Trace { get; set; }
		public string? Val_rel { get; set; }
		public string? Field_rel { get; set; }
		public string? Command_rel { get; set; }
	}
}