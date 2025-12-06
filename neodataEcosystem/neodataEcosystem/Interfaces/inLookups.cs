namespace neodataEcosystem.Interfaces
{
	public class inLookups : inBaseAnyRequest
	{
		public string? Fields { get; set; }
		public string? Where { get; set; }
		public string? Order { get; set; }
	}
}