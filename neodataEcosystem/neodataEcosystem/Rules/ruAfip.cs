namespace neodataEcosystem.Rules
{
	public class ruAfip : rulesBaseSuperClass
	{
		public ruAfip() : base("neo_afip") { }
		daAfip _da = new daAfip();

		public outBaseAnyResponse Status()
		{
			try
			{
				_response = _da.Status();
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}

		public async Task<outBaseAnyResponse> Invoice(inAfip _params, string _host)
		{
			try
			{
				_response = await _da.Invoice(_params, _host);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public async Task<outBaseAnyResponse> CreditNote(inAfip _params, string _host)
		{
			try
			{
				_response = await _da.CreditNote(_params, _host);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
	}
}

