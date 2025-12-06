namespace neodataEcosystem.SuperClasses
{
	public class outBaseAnyTable
	{
		public int Id { get; set; }
		public string? Code { get; set; }
		public string? Description { get; set; }
		public DateTime? Created { get; set; }
		public DateTime? Verified { get; set; }
		public DateTime? Offline { get; set; }
		public DateTime? Fum { get; set; }
		public int Id_application { get; set; }
		public int Id_user { get; set; }
	}
}