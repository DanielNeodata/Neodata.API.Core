using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inSignatures : inBaseAnyRequest
	{
		public int? Id_type_document { get; set; }
		public int? Id_type_status { get; set; }
		[Required] public string Raw_data { get; set; }
		[Required] public string Mime_type { get; set; }
		public string? Raw_data_additional { get; set; }
		public string? Mime_type_additional { get; set; }
		public float Lat { get; set; }
		public float Lng { get; set; }
		public float Altitude { get; set; }
		public float Speed { get; set; }
		public string? Referer { get; set; }
		public string? Custom_message { get; set; }
		public string? Modifier { get; set; }
		public string? ExternalId { get; set; }
		public string? Type_key { get; set; }
		public string? Val_key { get; set; }
		public string? Name_key { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int PageToAlter { get; set; }
	}
	public class inSignaturesSearch : inBaseAnyRequest
	{
		[Required] public int Id { get; set; }
		public string? Type_key { get; set; }
		public string? Val_key { get; set; }
		public string? Name_key { get; set; }
		public int Id_type_document { get; set; }
		public int Id_type_status { get; set; }
		public DateTime? CreatedFrom { get; set; }
		public DateTime? CreatedTo { get; set; }
	}
}