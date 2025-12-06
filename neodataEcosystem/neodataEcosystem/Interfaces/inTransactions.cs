using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inTransactions : inBaseAnyRequest
	{
		public int Id_type_status { get; set; }
		public int? Id_type_transaction { get; set; }
		[Required] public string Raw_data { get; set; }
		[Required] public string Mime_type { get; set; }
		public string? ExternalId { get; set; }
	}
}