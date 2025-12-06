using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{
	public class daTransactions : dataBaseSuperClass
	{
		neoHelper _Helper = new neoHelper();

		public daTransactions() : base("neo_transactions") { }
        public outBaseAnyResponse Create(inTransactions _params, bool _nosign)
		{
			string _line = "0";
			outBaseAnyResponse _response = new outBaseAnyResponse();

			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();
					#region Signing based in ID_application and Id_user, for search Profile
					interOpSigningByProfile _objSign = new interOpSigningByProfile();
					_objSign.Id_application = _params.Id_application;
					_objSign.Id_user = _params.Id_user;
					_objSign.DataToSign = _params.Raw_data;
					_objSign.KeyDatabase = _DataContext.DefaultDatabase;
					_objSign = new daSignatures().SignString(_objSign, _nosign);
					#endregion

					#region Insert transaction!
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;

					if (_params.Id_type_transaction != null)
					{
						cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_transactions_type_transactions WHERE id=@ID_TYPE_TRANSACTION AND id_profile=@ID_PROFILE";
						cmd.Parameters.Clear();
						cmd.Parameters.AddWithValue("@ID_TYPE_TRANSACTION", _params.Id_type_transaction);
						cmd.Parameters.AddWithValue("@ID_PROFILE", _objSign.Id_Profile);
						DataTable dtResponse = new DataTable();
						dtResponse.Load(cmd.ExecuteReader());
						if (Convert.ToInt32(dtResponse.Rows[0]["total"].ToString()) == 0)
						{
							_line = "";
							throw new Exception("El tipo de transacción no está relacionado al perfil del usuario", new Exception("901.04"));
						}
					}
					_line = "48";

					cmd.CommandText = "INSERT INTO dbo.mod_transactions_transactions ";
					cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_application,id_user,id_type_status,privateParty,publicParty,raw_data,mime_type,signature,privateKey,publicKey,integrity,verified_integrity,externalid,id_profile";
					if (_params.Id_type_transaction != null) { cmd.CommandText += ",id_type_transaction"; }
					cmd.CommandText += ") ";
					cmd.CommandText += " VALUES ";
					cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Transacción con datos personalizados") + ",@ID_APPLICATION,@ID_USER,@ID_TYPE_STATUS,@PRIVATEPARTY,@PUBLICPARTY,@RAW_DATA,@MIME_TYPE,@SIGNATURE,@PRIVATEKEY,@PUBLICKEY,1,getdate(),@EXTERNALID,@ID_PROFILE";
					if (_params.Id_type_transaction != null) { cmd.CommandText += ",@ID_TYPE_TRANSACTION"; }
					cmd.CommandText += ") ";
					cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

					cmd.Parameters.Clear();
					_line = "40";
					cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
					cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
					cmd.Parameters.AddWithValue("@ID_TYPE_STATUS", _params.Id_type_status);
					cmd.Parameters.AddWithValue("@RAW_DATA", _params.Raw_data);
					cmd.Parameters.AddWithValue("@MIME_TYPE", _params.Mime_type);
					cmd.Parameters.AddWithValue("@EXTERNALID", _params.ExternalId);
					cmd.Parameters.AddWithValue("@PRIVATEPARTY", _objSign.PrivateParty);
					cmd.Parameters.AddWithValue("@PUBLICPARTY", _objSign.PublicParty);
					cmd.Parameters.AddWithValue("@SIGNATURE", _objSign.Signature);
					cmd.Parameters.AddWithValue("@PRIVATEKEY", _objSign.PrivateKey);
					cmd.Parameters.AddWithValue("@PUBLICKEY", _objSign.PublicKey);
					cmd.Parameters.AddWithValue("@ID_PROFILE", _objSign.Id_Profile);
					if (_params.Id_type_transaction != null) { cmd.Parameters.AddWithValue("@ID_TYPE_TRANSACTION", _params.Id_type_transaction); }
					_line = "50";
					_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
					if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
					#endregion

					connection.Close();

					#region Automatic Log
					_line = "60";
					WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
					#endregion

				}
				return _response;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				if (_line != "") { msg += " - Error en línea " + _line.ToString(); }
				throw new Exception(msg);
			}
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
			string _fields = " id,code,description,created,verified,offline,fum,id_type_transaction,id_type_status,mime_type,raw_data,integrity,verified_integrity,externalid ";
			string _where = " WHERE id_profile=@ID_PROFILE";
			if (_params.Id != 0 && _params.Id.HasValue)
			{
                _fields = " * ";
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
			if (_params.ExternalId != null && _params.ExternalId != "") { _where += " AND externalid=@EXTERNALID "; }
			if (_params.Date_from.HasValue) { _where += " AND (created >=@DATE_FROM) "; }
			if (_params.Date_to.HasValue) { _where += " AND (created <=@DATE_TO) "; }
			if (_params.Id_Type_Status != 0 && _params.Id_Type_Status.HasValue) { _where += " AND id_type_status=@ID_TYPE_STATUS "; }
			if (_params.Id_Type_Transaction != 0 && _params.Id_Type_Transaction.HasValue) { _where += " AND id_type_transaction=@ID_TYPE_TRANSACTION "; }

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
				if (_params.ExternalId != null && _params.ExternalId != "") { cmd.Parameters.AddWithValue("@EXTERNALID", _params.ExternalId); }
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
				if (_params.Id_Type_Status != 0 && _params.Id_Type_Status.HasValue) { cmd.Parameters.AddWithValue("@ID_TYPE_STATUS", _params.Id_Type_Status); }
				if (_params.Id_Type_Transaction != 0 && _params.Id_Type_Transaction.HasValue) { cmd.Parameters.AddWithValue("@ID_TYPE_TRANSACTION", _params.Id_Type_Transaction); }
				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.TotalPages = (int)dtResponse.Rows[0]["total"] / _response.PageSize + 1;

				cmd.CommandText = "SELECT "+ _fields + " FROM (SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS CI_rownum, * ";
				cmd.CommandText += " FROM dbo.mod_transactions_vw_transactions ";
				if (_where != "") { cmd.CommandText += _where; }
				cmd.CommandText += ") AS CI_subquery WHERE ";
                cmd.CommandText += " CI_rownum BETWEEN " + _from.ToString() + " AND " + _to.ToString();
                cmd.CommandText += " ORDER BY created ASC";

                dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());

				dtResponse.Columns.Add("latitude");
				dtResponse.Columns.Add("longitude");
				foreach (DataRow row in dtResponse.Rows)
				{
					row["latitude"] = null;
					row["longitude"] = null;
					if (_Helper.IsJsonValid(row["raw_data"].ToString()))
					{
						JObject raw_data = JObject.Parse(row["raw_data"].ToString());
						if (raw_data.ContainsKey("latitud")) { row["latitude"] = raw_data["latitud"]; }
						if (raw_data.ContainsKey("longitud")) { row["longitude"] = raw_data["longitud"]; }
						if (raw_data.ContainsKey("latitude")) { row["latitude"] = raw_data["latitude"]; }
						if (raw_data.ContainsKey("longitude")) { row["longitude"] = raw_data["longitude"]; }
						if (raw_data.ContainsKey("lat")) { row["latitude"] = raw_data["lat"]; }
						if (raw_data.ContainsKey("long")) { row["longitude"] = raw_data["long"]; }
						if (raw_data.ContainsKey("lng")) { row["latitude"] = raw_data["lng"]; }
					}
				}
				_response.Records = DataTableToList(dtResponse);

				connection.Close();
				return _response;
			}
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
				_response.Trace = "NeoTransactions";
				_response.Scope = "V1";
				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
	}
}
