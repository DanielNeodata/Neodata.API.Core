namespace neodataEcosystem.Rules
{
	public class ruLookups : rulesBaseSuperClass
	{
		public ruLookups(string _keyDatabase, string _tableView) : base(_keyDatabase, _tableView) { }
		public outBaseAnyResponse Select(inLookups _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response.Records = new daLookups(KeyDatabase).Select(_params, TableView);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
	}
}