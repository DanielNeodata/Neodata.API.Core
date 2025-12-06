namespace neodataEcosystem.Rules
{
	public class ruLogGeneral : rulesBaseSuperClass
	{
		public ruLogGeneral() : base("neo_logs") { }
		daLogGeneral _da = new daLogGeneral();

		public outBaseAnyResponse Create(inLogs _params)
		{
			try
			{
				outBaseAnyResponse _validate = new outBaseAnyResponse();
				_validate = new daAuthenticate().VerifyTokenEthernal(
					new inToken
					{
						Id_application = _params.Id_application,
						Id_user = _params.Id_user,
						Token = _params.Token
					}
				);
				_response = new daLogGeneral().Create(_params);
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
				_response.Trace = "NeoLogs";
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