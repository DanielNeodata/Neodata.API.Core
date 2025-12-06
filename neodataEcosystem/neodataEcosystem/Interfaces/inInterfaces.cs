using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inInterfaces : inBaseAnyRequest
	{
		[Required] public int Id_type_status { get; set; }
		[Required] public string Raw_data { get; set; }
		[Required] public string Mime_type { get; set; }
		public string? ExternalId { get; set; }
	}
	public class inInterfacesSearch : inBaseAnyRequest
	{
		[Required] public int Id { get; set; }
		public int Id_type_status { get; set; }
		public DateTime? CreatedFrom { get; set; }
		public DateTime? CreatedTo { get; set; }
	}
}