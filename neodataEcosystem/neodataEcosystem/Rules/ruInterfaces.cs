namespace neodataEcosystem.Rules
{
	public class ruInterfaces : rulesBaseSuperClass
	{
		daInterfaces _da = new daInterfaces();
		public ruInterfaces() : base("neo_interfaces") { }
		public outBaseAnyResponse Search(inSearch _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Search(_params);
				_response.Trace = "NeoInterfaces";
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