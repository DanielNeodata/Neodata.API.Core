namespace neodataEcosystem.Data
{
	public class daAuthenticate : dataBaseSuperClass
	{
		public daAuthenticate() : base("neo_authentication") { }

		public outAuthentication Authenticate(inAuthentication _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT ";
				cmd.CommandText += " id as id_user,id_type_user,id_application,type_user,application,username,name,surname,email,phone,image ";
				cmd.CommandText += " FROM dbo.mod_backend_vw_users WHERE username=@USERNAME AND ";
				cmd.CommandText += " password=@PASSWORD AND ";
				cmd.CommandText += " offline IS null AND ";
				cmd.CommandText += " id IN (SELECT r.id_user FROM mod_backend_rel_users_applications as r WHERE r.id_application=@ID_APPLICATION) AND ";
				cmd.CommandText += " id_application=@ID_APPLICATION";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@USERNAME", _params.Username);
				cmd.Parameters.AddWithValue("@PASSWORD", _params.Password);
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Records = DataTableToList(dtResponse);
				_response.Logic = (dtResponse.Rows.Count != 0);
				if (!_response.Logic) { throw new Exception("No rows", new Exception("901.01")); }
				_response.Numeric = Convert.ToInt32(_response.Records[0]["id_user"]);
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }

				cmd.CommandText = "SELECT ";
				cmd.CommandText += " * FROM mod_backend_applications WHERE ";
				cmd.CommandText += "   id IN ";
				cmd.CommandText += "    (SELECT id_application FROM mod_backend_rel_users_applications WHERE id_user=@ID_USER)";
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_USER", _response.Numeric);
				DataTable dtAvailable = new DataTable();
				dtAvailable.Load(cmd.ExecuteReader());
				_response.AvailableApplications = DataTableToList(dtAvailable);

				connection.Close();

				#region Automatic Log
				WriteLog(Convert.ToInt32(_response.Records[0]["id_application"]), _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse Persist(inTokenPersist _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "INSERT INTO dbo.mod_backend_tokens ";
				cmd.CommandText += " (" + _DataContext.getCommonFields() + ",token,token_created,token_expire,token_type,id_user,id_application) ";
				cmd.CommandText += " VALUES ";
				cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "") + ",@TOKEN,getdate(),dateadd(ss,@SECONDS,getdate()),@TOKEN_TYPE,@ID_USER,@ID_APPLICATION) ";
				cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@TOKEN", _params.Token);
				cmd.Parameters.AddWithValue("@SECONDS", _params.Seconds);
				cmd.Parameters.AddWithValue("@TOKEN_TYPE", _params.Token_type);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);

				try
				{
					_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
					if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }

					connection.Close();

					#region Automatic Log
					WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion
				}
				catch (Exception err) { throw new Exception("No rows", err); }

				if (!_response.Logic) { throw new Exception("No rows", new Exception("901.01")); }
			}
			return _response;
		}
		public outBaseAnyResponse VerifyTokenSingleUser(inToken _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			if (!_Token.VerifyToken(_params.Token, _params.Id_application, _params.Id_user)) { throw new Exception("Expired token", new Exception("901.03")); }
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT * FROM dbo.mod_backend_tokens WHERE token=@TOKEN AND ";
				cmd.CommandText += "id_application=@ID_APPLICATION AND ";
				cmd.CommandText += "id_user=@ID_USER";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@TOKEN", _params.Token);
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());

				_response.Logic = dtResponse.Rows.Count != 0;
				if (!_response.Logic) { throw new Exception("No rows", new Exception("901.01")); }
				_response.Numeric = Convert.ToInt32(dtResponse.Rows[0]["id"].ToString());
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion

			}
			return _response;
		}
		public outBaseAnyResponse VerifyTokenEthernal(inToken _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			if (!_Token.VerifyToken(_params.Token, _params.Id_application, _params.Id_user)) { throw new Exception("Expired token", new Exception("901.03")); }
			return _response;
		}
		public outBaseAnyResponse Destroy(inToken _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "DELETE dbo.mod_backend_tokens WHERE token=@TOKEN";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@TOKEN", _params.Token);

				try
				{
					_response.Numeric = Convert.ToInt32(cmd.ExecuteNonQuery());
					if (_response.Numeric == 0) { _response.Message = "No record affected"; }

					connection.Close();
				}
				catch (Exception err) { throw new Exception("Database error", err); }

				#region Automatic Log
				WriteLog(0, 0, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}

			return _response;
		}
		public outBaseAnyResponse State(inBaseAnyRequest _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT * FROM dbo.mod_backend_applications WHERE id=@ID_APPLICATION";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Records = DataTableToList(dtResponse);
				_response.Logic = (dtResponse.Rows.Count != 0);
				if (!_response.Logic) { throw new Exception("No rows", new Exception("901.01")); }
				_response.Numeric = Convert.ToInt32(_response.Records[0]["id"]);
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
				_response.Message = "La plataforma está operativa y sus credenciales válidas.";
				_response.Trace = "NeoAuthentication";
				_response.Scope = "V1";
				connection.Close();

				#region Automatic Log
				WriteLog(Convert.ToInt32(_response.Records[0]["id"]), _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
	}
}
