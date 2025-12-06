using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inAuthentication
	{
		[Required][RegularExpression("^[a-zA-Z][\\w]*$", ErrorMessage = "Invalid value: only alphanumeric and underscore characters")] public string Username { get; set; }
		[Required] public string Password { get; set; }
		[Required] public int Id { get; set; }

	}
	public class outAuthentication : outBaseAnyResponse
	{
		public outAuthentication(outBaseAnyResponse response) : base(response.Scope) { }

		public string? TokenSingleUse { get; set; }
		public string? TokenEthernal { get; set; }
		public List<Dictionary<string, object>>? AvailableApplications { get; set; }

	}
}