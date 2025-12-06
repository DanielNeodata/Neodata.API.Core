namespace neodataEcosystem.SuperClasses
{
	public class rulesBaseSuperClass
	{
		public neoToken _Token = new neoToken();
		public neoCrypto _Crypto = new neoCrypto();
		public neoHelper _Helper = new neoHelper();
		public neoConfiguration _configTokens = new neoConfiguration(neoConfiguration.typeConfig.Tokens);
		public outBaseAnyResponse _response = new outBaseAnyResponse();

		public rulesBaseSuperClass(string _keyDatabase, string _tableView = "")
		{
			KeyDatabase = _keyDatabase;
			TableView = _tableView;
		}
		public string KeyDatabase { get; set; }
		public string TableView { get; set; }
		public outBaseAnyResponse VerifyAndUseTokenRule(inToken _params)
		{
			ruAuthentication _ruTk = new ruAuthentication();
			outBaseAnyResponse _response = new outBaseAnyResponse();
			if (!_configTokens.Tokens["default"].ignore)
			{
				outBaseAnyResponse _verify = _ruTk.VerifyTokenSingleUser(_params);
				if (!_verify.Logic)
				{
					_verify = _ruTk.VerifyTokenEthernal(_params);
					if (!_verify.Logic)
					{
						throw new Exception("Token failure", new Exception("900.04"));
					}
				}

				/*--------------------------------------------------------------------------------*/
				/* Single use token destruction! Safety key function for dodge invalids calls.
                    * No more token for further calls! must be regenerated!*/
				/*--------------------------------------------------------------------------------*/
				_ruTk.Destroy(
					new inToken
					{
						Token = _params.Token,
						Id_application = _params.Id_application,
						Id_user = _params.Id_user
					});
				/*--------------------------------------------------------------------------------*/
			}
			return _response;
		}
		public outBaseAnyResponse VerifyEthernalTokenRule(inToken _params)
		{
			ruAuthentication _ruTk = new ruAuthentication();
			outBaseAnyResponse _response = new outBaseAnyResponse();
			if (!_configTokens.Tokens["ethernal"].ignore)
			{
				outBaseAnyResponse _verify = _ruTk.VerifyTokenEthernal(_params);
				if (!_verify.Logic) { throw new Exception("Token failure", new Exception("900.04")); }
			}
			return _response;
		}
		public void SetError(Exception ex)
		{
			_response.Logic = false;
			_response.Code = "900";
			_response.Error = ex.Message.ToString();
			_response.Status = "ERROR";
			_response.Message = "Error raised from " + _response.Scope;
			if (ex.InnerException != null) { _response.Trace = ex.InnerException.ToString(); }
		}
	}
}