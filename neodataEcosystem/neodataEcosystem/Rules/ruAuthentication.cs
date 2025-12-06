namespace neodataEcosystem.Rules
{
	public class ruAuthentication : rulesBaseSuperClass
	{
		daAuthenticate _da = new daAuthenticate();
		public ruAuthentication() : base("neo_authentication") { }

		public outAuthentication Authenticate(inAuthentication _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());

			try
			{
				if (_params.Username == "") { throw new Exception("No Username", new Exception("900.01")); }
				if (_params.Password == "") { throw new Exception("No Password", new Exception("900.02")); }
				if (_params.Id == 0) { throw new Exception("No Id", new Exception("900.03")); }
				if (!_Crypto.IsMD5(_params.Password)) { _params.Password = _Crypto.MD5(_params.Password); }

				_response = _da.Authenticate(_params);

				if (_response.Logic)
				{
					int seconds = Convert.ToInt32(_configTokens.Tokens["default"].seconds);
					int secondsEthernal = Convert.ToInt32(_configTokens.Tokens["ethernal"].seconds);
					int _id_application = Convert.ToInt32(_response.Records[0]["id_application"]);
					/*Token generation */
					_response.TokenSingleUse = _Token.GenerateToken(_response.Numeric, _response.Records[0]["username"].ToString(), _id_application, seconds, "neodataEcosystem.gruponeodata.com", "neodataEcosystem.gruponeodata.com");
					_response.TokenEthernal = _Token.GenerateToken(_response.Numeric, _response.Records[0]["username"].ToString(), _id_application, secondsEthernal, "neodataEcosystem.gruponeodata.com", "neodataEcosystem.gruponeodata.com");
					try
					{
						/*Token persist in DBMS */
						ruAuthentication _ruTk = new ruAuthentication();
						outBaseAnyResponse _outPersist = _ruTk.Persist(
							new inTokenPersist
							{
								Token = _response.TokenSingleUse,
								Token_type = _configTokens.Tokens["default"].type,
								Seconds = seconds,
								Id_user = _response.Numeric,
								Id_application = _id_application
							});
					}
					catch (Exception ex)
					{
						throw new Exception("Token persistence error", ex);
					}
				}
			}
			catch (Exception ex)
			{
				_response.Logic = false;
				_response.Code = "900";
				_response.Error = ex.Message.ToString();
				_response.Status = "ERROR";
				_response.Message = "Error raised from " + _response.Scope;
				if (ex.InnerException != null) { _response.Trace = ex.InnerException.ToString(); }
			}
			return _response;
		}
		public outBaseAnyResponse VerifyTokenSingleUser(inToken _params)
		{
			try
			{
				_response = _da.VerifyTokenSingleUser(_params);
				if (!_response.Logic)
				{
					outBaseAnyResponse _outDestroy = new ruAuthentication().Destroy(
						new inToken
						{
							Token = _params.Token,
							Id_application = _params.Id_application,
							Id_user = _params.Id_user
						});
				}
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse VerifyTokenEthernal(inToken _params)
		{
			try
			{
				_response = _da.VerifyTokenEthernal(_params);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse Persist(inTokenPersist _params)
		{
			try
			{
				_response = _da.Persist(_params);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse Destroy(inToken _params)
		{
			try
			{
				_response = _da.Destroy(_params);
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
		public outBaseAnyResponse Monitoring(inSearch _params)
		{
			try
			{
				_response = new daLogGeneral().Search(_params);
				_response.Trace = "NeoAuthentication";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
	}
}