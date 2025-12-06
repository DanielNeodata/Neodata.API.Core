namespace neodataEcosystem.Interfaces
{
	public class inSchedule : inBaseAnyRequest
	{
		public DateTime? Date_from { get; set; }
		public DateTime? Date_to { get; set; }
		public string? Address { get; set; }
		public string Raw_data { get; set; }
		public string Details { get; set; }
		public string? ExternalId { get; set; }
		public string? External_response { get; set; }
		public int Id_type_frequency { get; set; }
		public int? Custom_frequency_1 { get; set; }
		public int? Custom_frequency_2 { get; set; }
		public int? Custom_frequency_3 { get; set; }
		public int? Custom_frequency_4 { get; set; }
		public int? Custom_frequency_5 { get; set; }
		public int? Custom_frequency_6 { get; set; }
		public int? Custom_frequency_7 { get; set; }
		public double? Latitude { get; set; }
		public double? Longitude { get; set; }
		public int? Id_user_assigned { get; set; }
	}
}