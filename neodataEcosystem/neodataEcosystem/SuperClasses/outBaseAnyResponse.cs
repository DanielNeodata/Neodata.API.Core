public class outBaseAnyResponse
{
	public outBaseAnyResponse(string _scope = "")
	{
		DateTime _now = DateTime.Now;
		if (_scope == "") { _scope = MethodBase.GetCurrentMethod().ReflectedType.FullName; }
		Scope = _scope;
		Logic = true;
		Numeric = 0;
		Code = "200";
		Error = "";
		Status = "OK";
		Message = "";
		Trace = "";
		UDT0 = _now.ToUniversalTime();
		UDT1 = _now.ToString("yyyy-MM-dd HH:mm:ss.fff");
		Now = _now.ToString("yyyy-MM-dd HH:mm:ss");
		Today = _now.ToString("yyyy-MM-dd");
		Page = 1;
		PageSize = 25;
		TotalPages = 0;
	}
	public int Page { get; set; }
	public int PageSize { get; set; }
	public int TotalPages { get; set; }
	public string? Scope { get; set; }
	public bool Logic { get; set; }
	public int Numeric { get; set; }
	public string? Code { get; set; }
	public string? Error { get; set; }
	public string? Status { get; set; }
	public string? Message { get; set; }
	public string? Trace { get; set; }
	public DateTime UDT0 { get; set; }
	public string? UDT1 { get; set; }
	public string? Now { get; set; }
	public string? Today { get; set; }
	public string? Base64 { get; set; }
	public string? Mime { get; set; }
	public string? Url { get; set; }
	public List<Dictionary<string, object>>? Records { get; set; }
}
