using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{
	public class daVideo : dataBaseSuperClass
	{
		public daVideo() : base("neo_video") { }

		public outBaseAnyResponse Create(inVideo _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			if (_params.ExternalId == null) { _params.ExternalId = "0"; }
			if (_params.Live == null) { _params.Live = 0; }
			CleanOrphanSessions(_params);

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				#region Signing based in ID_application and Id_user, for search Profile
				interOpSigningByProfile _objSign = new interOpSigningByProfile();
				_objSign.Id_application = _params.Id_application;
				_objSign.Id_user = _params.Id_user;
				if (_params.Raw_data == null) { _params.Raw_data = ""; }
				_objSign.DataToSign = _params.Raw_data;
				_objSign.KeyDatabase = _DataContext.DefaultDatabase;
				_objSign = new daSignatures().SignString(_objSign, false);
				#endregion

				#region Insert transaction!
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "INSERT INTO dbo.mod_transactions_transactions ";
				cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_application,id_user,id_type_status,privateParty,publicParty,raw_data,mime_type,signature,privateKey,publicKey,integrity,verified_integrity,externalid,id_profile,live) ";
				cmd.CommandText += " VALUES ";
				cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Transacción con datos de control de video") + ",@ID_APPLICATION,@ID_USER,@ID_TYPE_STATUS,@PRIVATEPARTY,@PUBLICPARTY,@RAW_DATA,@MIME_TYPE,@SIGNATURE,@PRIVATEKEY,@PUBLICKEY,1,getdate(),@EXTERNALID,@ID_PROFILE,@LIVE) ";
				cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_TYPE_STATUS", 1);
				cmd.Parameters.AddWithValue("@RAW_DATA", _params.Raw_data);
				cmd.Parameters.AddWithValue("@MIME_TYPE", "text/html");
				cmd.Parameters.AddWithValue("@EXTERNALID", _params.ExternalId);
				cmd.Parameters.AddWithValue("@PRIVATEPARTY", _objSign.PrivateParty);
				cmd.Parameters.AddWithValue("@PUBLICPARTY", _objSign.PublicParty);
				cmd.Parameters.AddWithValue("@SIGNATURE", _objSign.Signature);
				cmd.Parameters.AddWithValue("@PRIVATEKEY", _objSign.PrivateKey);
				cmd.Parameters.AddWithValue("@PUBLICKEY", _objSign.PublicKey);
				cmd.Parameters.AddWithValue("@ID_PROFILE", _objSign.Id_Profile);
				cmd.Parameters.AddWithValue("@LIVE", _params.Live);
				_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }

				cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET raw_data=@RAW_DATA WHERE id=@ID";
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID", _response.Numeric);
				_response.Message = _Token.GenerateTokenJitsi(_response.Numeric);
				cmd.Parameters.AddWithValue("@RAW_DATA", _response.Message);
				cmd.ExecuteScalar();
				#endregion

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse JoinOpenSession(inVideoIn _params)
		{
			string _err = "";
			outBaseAnyResponse _response = new outBaseAnyResponse();
			CleanOrphanSessions(new inVideo { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				if (_params.reOpen.HasValue) {
					if ((bool)_params.reOpen) {
                        cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET id_type_status=1 WHERE id=@ID";
                        cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
                        cmd.ExecuteScalar();
                    }
                }

				cmd.Parameters.Clear();
                cmd.CommandText = "SELECT id,raw_data FROM dbo.mod_transactions_transactions WHERE id=@ID AND id_type_status IN (1,2,3)";
				cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
				_err = (cmd.CommandText + " @ID=" + _params.Id_transaction);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				if (dtResponse.Rows.Count == 0) { throw new Exception("No available record to affect: " + _err, new Exception("901.02")); }
				_response.Numeric = Convert.ToInt32(dtResponse.Rows[0]["id"].ToString());
				_response.Message = dtResponse.Rows[0]["raw_data"].ToString();

				cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET fum=getdate(), id_type_status=2 WHERE id=@ID";
				cmd.ExecuteScalar();
				cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET fum=getdate(), id_type_status=3 WHERE id=@ID";
				cmd.ExecuteScalar();

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse CloseOpenSession(inVideoIn _params)
		{
			string _err = "";
			outBaseAnyResponse _response = new outBaseAnyResponse();
			CleanOrphanSessions(new inVideo { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT id FROM dbo.mod_transactions_transactions WHERE id=@ID AND id_type_status IN (1,2,3)";
				cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
				_err = (cmd.CommandText + " @ID=" + _params.Id_transaction);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				if (dtResponse.Rows.Count == 0) { throw new Exception("No available record to affect: " + _err, new Exception("901.02")); }

				cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET fum=getdate(), id_type_status=4 WHERE id=@ID";
				cmd.ExecuteScalar();
				cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET fum=getdate(), id_type_status=5 WHERE id=@ID";
				cmd.ExecuteScalar();

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public bool CleanOrphanSessions(inVideo _params)
		{
			try
			{
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);
				int _seconds_to_connect = Convert.ToInt32(dtResponse.Rows[0]["seconds_to_connect"]);
				int _seconds_to_drop_after_idle = Convert.ToInt32(dtResponse.Rows[0]["seconds_to_drop_after_idle"]);
				int _seconds_to_drop_after_idle_stream = (_seconds_to_drop_after_idle * 30);

				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;

					string _sqlBase = "UPDATE dbo.mod_transactions_transactions SET fum=getdate(),id_type_status=5";
					_sqlBase += "WHERE (live IS null OR live=@live) ";

					/*Search for open Sessions and never guest connected or bad disconnection from host!*/
					cmd.CommandText = _sqlBase + " AND id_type_status=1 AND DATEDIFF(ss, created, GETDATE())>@ttl";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@live", 0);
					cmd.Parameters.AddWithValue("@ttl", _seconds_to_connect.ToString());
					cmd.ExecuteScalar();

					/*Search for Sessions with video update NULL from host side!*/
					cmd.CommandText = _sqlBase + " AND id_type_status!=5 AND DATEDIFF(ss, created, GETDATE())>@ttl AND update_video_host IS null";
					cmd.Parameters["@ttl"].Value = (_seconds_to_drop_after_idle * 4).ToString();
					cmd.ExecuteScalar();

					/*Search for Sessions with no video update from host side normal and live!*/
					cmd.CommandText = _sqlBase + " AND id_type_status!=5 AND DATEDIFF(ss, update_video_host, GETDATE())>@ttl AND update_video_host IS NOT null";
					cmd.Parameters["@ttl"].Value = _seconds_to_drop_after_idle.ToString();
					cmd.ExecuteScalar();

					/*Search for Sessions with no video update from guest side!*/
					cmd.CommandText = _sqlBase + " AND id_type_status!=5 AND DATEDIFF(ss, update_video_guest, GETDATE())>@ttl AND update_video_guest IS NOT null";
					cmd.Parameters["@ttl"].Value = _seconds_to_drop_after_idle.ToString();
					cmd.ExecuteScalar();

					/*Search for Sessions with no video update from guest side in live stream mode!*/
					cmd.CommandText = _sqlBase + " AND id_type_status!=5 AND DATEDIFF(ss, update_video_guest, GETDATE())>@ttl AND update_video_guest IS NOT null";
					cmd.Parameters["@live"].Value = 1;
					cmd.Parameters["@ttl"].Value = _seconds_to_drop_after_idle_stream.ToString();
					cmd.ExecuteScalar();

					connection.Close();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public outBaseAnyResponse UpdateVideo(inVideoIn _params, string _field)
		{
			string _err = "";
			outBaseAnyResponse _response = new outBaseAnyResponse();

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT id FROM dbo.mod_transactions_transactions WHERE id=@ID AND id_type_status IN (1,2,3)";
				cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
				_err = (cmd.CommandText + " @ID=" + _params.Id_transaction);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				if (dtResponse.Rows.Count == 0) { throw new Exception("No available record to affect: " + _err, new Exception("901.02")); }

				cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET fum=getdate(), " + _field + "=getdate() WHERE id=@ID";
				cmd.ExecuteScalar();

				connection.Close();
			}
			return _response;
		}
		public outBaseAnyResponse UpdateWrtcAnswer(inVideoWrtcAnswer _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "UPDATE dbo.mod_transactions_wrtc SET fum=getdate(), raw_answer=@raw_answer WHERE id_transaction=@ID";
				cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
				cmd.Parameters.AddWithValue("@raw_answer", _params.Raw_answer);
				cmd.ExecuteScalar();

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse SendRelatedData(inVideoData _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			if (_params.Description == null) { _params.Description = ""; }
			if (_params.Raw_data == null) { _params.Raw_data = ""; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "INSERT INTO dbo.mod_transactions_data_transactions ";
				cmd.CommandText += " (code,description,created,verified,offline,fum,id_transaction,id_item,raw_data) ";
				cmd.CommandText += " VALUES ";
				cmd.CommandText += " ('" + Guid.NewGuid().ToString() + "','" + _params.Description + "',getdate(),null,null,getdate(),@id_transaction,@id_item,@RAW_DATA) ";
				cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@id_transaction", _params.Id_transaction);
				cmd.Parameters.AddWithValue("@id_item", _params.Id_user);
				cmd.Parameters.AddWithValue("@RAW_DATA", _params.Raw_data);
				_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse DeleteDataInClient(inVideoIn _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "UPDATE dbo.mod_transactions_data_transactions SET fum=getdate(), offline=getdate() WHERE id=@ID";
				cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
				cmd.ExecuteScalar();

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse ReceiveRelatedData(inVideoReceive _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				if (_params.Remove == 0)
				{
					cmd.CommandText = "UPDATE dbo.mod_transactions_data_transactions SET fum=getdate(), verified=getdate() WHERE id=@ID";
				}
				else
				{
					cmd.CommandText = "DELETE dbo.mod_transactions_data_transactions WHERE id=@ID";
				}
				cmd.Parameters.AddWithValue("@ID", _params.Id_transaction);
				cmd.ExecuteScalar();

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
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
				cmd.CommandText = "SELECT * FROM dbo.mod_backend_profiles WHERE id_application_auth=@ID_APPLICATION AND id_user_auth=@ID_USER";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Records = DataTableToList(dtResponse);
				_response.Logic = (dtResponse.Rows.Count != 0);
				if (!_response.Logic) { throw new Exception("No rows", new Exception("901.01")); }
				_response.Numeric = Convert.ToInt32(_response.Records[0]["id"]);
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
				_response.Message = "La plataforma está operativa y sus credenciales válidas.";
				_response.Trace = "NeoVideo";
				_response.Scope = "V1";
				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
		public outBaseAnyResponse Search(inSearch _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
			if (_params.Page == 0 || _params.Page == null) { _params.Page = 1; }
			if (_params.PageSize == 0 || _params.PageSize == null) { _params.PageSize = 25; }
			_response.Page = (int)_params.Page;
			_response.PageSize = (int)_params.PageSize;

			int _from = ((int)((_params.Page - 1) * _params.PageSize));
			int _to = ((int)(_from + _params.PageSize));

			string _where = " WHERE id_profile=@ID_PROFILE";
			if (_params.Id != 0 && _params.Id.HasValue)
			{
				_where += " AND id=@ID ";
			}
			else
			{
				if (!String.IsNullOrEmpty(_params.Search))
				{
					if (Regex.IsMatch(_params.Search, @"^\d+$"))
					{
						_where += " AND id=@SEARCH ";
					}
					else
					{
						_where += " AND (description LIKE '%@SEARCH%') ";
					}
				}
			}
			if (_params.Date_from.HasValue) { _where += " AND (created >=@DATE_FROM) "; }
			if (_params.Date_to.HasValue) { _where += " AND (created <=@DATE_TO) "; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);

				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_transactions_vw_transactions ";
				if (_where != "") { cmd.CommandText += _where; }
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_PROFILE", Convert.ToInt32(dtResponse.Rows[0]["id"]));

				if (!String.IsNullOrEmpty(_params.Search)) { cmd.Parameters.AddWithValue("@SEARCH", _params.Search); }
				if (_params.Id != 0 && _params.Id.HasValue) { cmd.Parameters.AddWithValue("@ID", _params.Id); }
				if (_params.Date_from.HasValue)
				{
					DateTime _df = Convert.ToDateTime(_params.Date_from.ToString());
					cmd.Parameters.AddWithValue("@DATE_FROM", _df.ToString("yyyy-MM-dd"));
				}
				if (_params.Date_to.HasValue)
				{
					DateTime _dt = Convert.ToDateTime(_params.Date_to.ToString());
					cmd.Parameters.AddWithValue("@DATE_TO", _dt.ToString("yyyy-MM-dd"));
				}
				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.TotalPages = (int)dtResponse.Rows[0]["total"] / _response.PageSize + 1;

				cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS CI_rownum, * ";
				cmd.CommandText += " FROM dbo.mod_transactions_vw_transactions ";
				if (_where != "") { cmd.CommandText += _where; }
				cmd.CommandText += ") AS CI_subquery WHERE ";
				cmd.CommandText += " CI_rownum BETWEEN " + _from.ToString() + " AND " + _to.ToString();

				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Records = DataTableToList(dtResponse);

				connection.Close();
				return _response;
			}
		}
	}
}
