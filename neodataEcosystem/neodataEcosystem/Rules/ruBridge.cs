namespace neodataEcosystem.Rules
{
	public class ruBridge : rulesBaseSuperClass
	{
		public ruBridge() : base("neo_bridge") { }
		daBridge _da = new daBridge();

		public outBaseAnyResponse Status()
		{
			return new daBridge().Status();
		}

		public async Task<outBaseAnyResponse> LoyalAsync(inBridge _params)
		{
			try
			{
				_response = await new daBridge().LoyalAsync(_params);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
	}
}