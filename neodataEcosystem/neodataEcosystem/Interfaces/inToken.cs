namespace neodataEcosystem.Interfaces
{
	public class inToken : inBaseAnyRequest
	{
	}
	public class inTokenPersist : inToken
	{
		public int Seconds { get; set; }
		public string? Token_type { get; set; }
	}
}