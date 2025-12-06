namespace neodataEcosystem.Rules
{
	public class ruTransactions : rulesBaseSuperClass
	{
		daTransactions _da = new daTransactions();
		public ruTransactions() : base("neo_transactions") { }
        public outBaseAnyResponse Create(inTransactions _params, bool _nosign)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Create(_params, _nosign);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse Search(inSearch _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Search(_params);
				_response.Trace = "NeoTransactions";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse State(inBaseAnyRequest _params)
		{
			try
			{
				_response = _da.State(_params);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
	}
}