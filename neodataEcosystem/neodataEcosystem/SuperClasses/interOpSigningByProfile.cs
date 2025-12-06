namespace neodataEcosystem.SuperClasses
{
	public class interOpSigningByProfile
	{
		public int Id_application { get; set; }
		public int Id_user { get; set; }
		public int Id_Profile { get; set; }
		public string? KeyDatabase { get; set; }
		public string? DataToSign { get; set; }
		public string? PublicParty { get; set; }
		public string? PrivateParty { get; set; }
		public string? PublicKey { get; set; }
		public string? PrivateKey { get; set; }
		public string? Signature { get; set; }
	}
}