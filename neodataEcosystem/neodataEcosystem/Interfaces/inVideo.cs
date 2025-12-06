using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace neodataEcosystem.Interfaces
{
	public class inVideo : inBaseAnyRequest
	{
		public string? Raw_data { get; set; }
		public string? Tech { get; set; }
		public int Live { get; set; }
		public string? ExternalId { get; set; }
	}

	public class inVideoIn : inBaseAnyRequest
	{
		[Required] public int Id_transaction { get; set; }
		public bool? reOpen { get; set; }
	}
	public class inVideoWrtcAnswer : inBaseAnyRequest
	{
		[Required] public int Id_transaction { get; set; }
		[Required] public string Raw_answer { get; set; }
	}
	public class inVideoData : inBaseAnyRequest
	{
		[Required] public int Id_transaction { get; set; }
		public int Id_item { get; set; }
		public string? Description { get; set; }
		public string? Raw_data { get; set; }
	}
	public class inVideoReceive : inBaseAnyRequest
	{
		[Required] public int Id_transaction { get; set; }
		public int Remove { get; set; }
	}
}