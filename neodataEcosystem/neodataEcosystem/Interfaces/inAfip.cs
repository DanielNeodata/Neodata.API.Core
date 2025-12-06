namespace neodataEcosystem.Interfaces
{
	public class inAfip
	{
		public int IsProdEnvironment { get; set; }
		public int Id_application { get; set; }
		public int Id_user { get; set; }
		public string Token { get; set; }
		public long CUIT { get; set; }
		public int Records { get; set; }
		public int VoucherType { get; set; }
		public int SalesPoint { get; set; }
		public string InvoiceCreationDate { get; set; }
		public string InvoicePayDate { get; set; }
		public int Concept { get; set; }
		public int DocumentType { get; set; }
		public long DocumentNumber { get; set; }
		public double AmountNet { get; set; }
		public double AmountIva { get; set; }
		public double AmountTotal { get; set; }
		public DateTime? ServiceDateFrom { get; set; }
		public DateTime? ServiceDateTo { get; set; }
		public int CurrencyChange { get; set; }
		public string CurrencyId { get; set; }

		public int? OriginalVoucherType { get; set; }
		public int? OriginalInvoiceNumber { get; set; }
		public int? OriginalInvoiceSalesPoint { get; set; }
		public string? OriginalDate { get; set; }
		public long? OriginalDocumentNumber { get; set; }

	}
}