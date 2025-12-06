namespace neodataEcosystem.Rules
{
	public class ruNeural : rulesBaseSuperClass
	{
		public ruNeural() : base("neo_neural") { }
		daNeural _da = new daNeural();

		public outBaseAnyResponse Resolve(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Resolve(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse SynapsesMatrix(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.SynapsesMatrix(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse Trainning(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Trainning(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}

		public outBaseAnyResponse Questions(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Questions(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}

		public outBaseAnyResponse Projects(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Projects(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}

		public outBaseAnyResponse ResetProject(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.ResetProject(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse AddItemforTranning(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });

				/*Default for consolidate error managment */
				if (_params.Id_project == null) { _params.Id_project = 0; }
				if (_params.Base64RawData == null) { _params.Base64RawData = ""; }
				if (_params.Id_project == 0) { throw new Exception("No se ha especificado proyecto en [Id_project]"); }
				if (_params.Base64RawData == "") { throw new Exception("No se han enviado datos a insertar en [Base64RawData]"); }

				_response = _da.AddItemforTranning(_params);
				_response.Trace = "NeoNeural";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse AddItemResponseforTranning(inNeural _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });

				/*Default for consolidate error managment */
				if (_params.Id_project == null) { _params.Id_project = 0; }
				if (_params.Id_item == null) { _params.Id_item = 0; }
				if (_params.Id_question == null) { _params.Id_question = 0; }
				if (_params.Base64RawData == null) { _params.Base64RawData = ""; }
				if (_params.Id_project == 0) { throw new Exception("No se ha especificado proyecto en [Id_project]"); }
				if (_params.Id_item == 0) { throw new Exception("No se ha especificado proyecto en [Id_item]"); }
				if (_params.Id_question == 0) { throw new Exception("No se ha especificado proyecto en [Id_question]"); }
				if (_params.Base64RawData == "") { throw new Exception("No se han enviado datos a insertar en [Base64RawData]"); }

				_response = _da.AddItemResponseforTranning(_params);
				_response.Trace = "NeoNeural";
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