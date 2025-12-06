using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inRpa : inBaseAnyRequest
	{
		[Required] public int Id_robot { get; set; }
		[Required] public int Id_thread { get; set; }
		public int Id_step { get; set; }
		public string? Parameter { get; set; }
	}
	public class inRpaSearch : inBaseAnyRequest
	{
		[Required] public int Id { get; set; }
		public int? Page { get; set; }
		public int? PageSize { get; set; }
		public string? Search { get; set; }
		public DateTime? CreatedFrom { get; set; }
		public DateTime? CreatedTo { get; set; }
	}
	public class inRpaStart : inBaseAnyRequest
	{
		[Required] public int Id_profile { get; set; }
		[Required] public int Id_robot { get; set; }
		[Required] public bool HideNavigator { get; set; }
		public inRpaStartData[]? Data { get; set; }
	}
	public class inRpaStartData
	{
		[Required] public int Id_step { get; set; }
		[Required] public string? Parameter { get; set; }
		[Required] public string? Description { get; set; }
	}
	public class inRpaStartResponse
	{
		public string status { get; set; }
		public string key { get; set; }
		public string message { get; set; }
	}
	public class inRpaStartPuppeter
	{
		[Required] public int Id_profile { get; set; }
		[Required] public int Id { get; set; }
		[Required] public bool HideNavigator { get; set; }
		public inRpaStartData[]? Data { get; set; }
	}

}