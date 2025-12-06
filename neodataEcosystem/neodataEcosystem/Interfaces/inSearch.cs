namespace neodataEcosystem.Interfaces
{
	public class inSearch : inBaseAnyRequest
	{
		public int? Id { get; set; }
		public int? Page { get; set; }
		public int? PageSize { get; set; }
		public string? Search { get; set; }
		public DateTime? Date_from { get; set; }
		public DateTime? Date_to { get; set; }
		public int? Id_Type_Status { get; set; }
		public int? Id_Type_Transaction { get; set; }
		public int? Id_Type_Frequency { get; set; }
        public string? ExternalId { get; set; }
    }
}