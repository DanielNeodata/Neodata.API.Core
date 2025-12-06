using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{
	public class daSchedule : dataBaseSuperClass
	{
		neoHelper _Helper = new neoHelper();

		public daSchedule() : base("neo_schedule") { }
		public outBaseAnyResponse Create(inSchedule _params)
		{
			string _line = "0";
			outBaseAnyResponse _response = new outBaseAnyResponse();

			try
			{
				using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
				{
					connection.Open();

					if (_params.Address == null) { _params.Address = ""; }
					if (_params.ExternalId == null) { _params.ExternalId = ""; }
					if (_params.External_response == null) { _params.External_response = ""; }

					#region Signing based in ID_application and Id_user, for search Profile
					interOpSigningByProfile _objSign = new interOpSigningByProfile();
					_objSign.Id_application = _params.Id_application;
					_objSign.Id_user = _params.Id_user;
					_objSign.DataToSign = _params.Raw_data;
					_objSign.KeyDatabase = _DataContext.DefaultDatabase;
					_objSign = new daSignatures().SignString(_objSign, false);
					#endregion

					#region Insert transaction!
					SqlCommand cmd = new SqlCommand();
					cmd.Connection = connection;
					cmd.CommandType = CommandType.Text;

					cmd.CommandText = "INSERT INTO dbo.mod_schedule_events ";
					cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_application,id_user,id_profile,date_from,date_to,address,raw_data,details,externalid,external_response,id_type_frequency,custom_frequency_1,custom_frequency_2,custom_frequency_3,custom_frequency_4,custom_frequency_5,custom_frequency_6,custom_frequency_7,latitude,longitude,id_assigned_user) ";
					cmd.CommandText += " VALUES ";
					cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Evento con datos personalizados") + ",@ID_APPLICATION,@ID_USER,@ID_PROFILE,@DATE_FROM,@DATE_TO,@ADDRESS,@RAW_DATA,@DETAILS,@EXTERNALID,@EXTERNAL_RESPONSE,@ID_TYPE_FREQUENCY,@CUSTOM_FREQUENCY_1,@CUSTOM_FREQUENCY_2,@CUSTOM_FREQUENCY_3,@CUSTOM_FREQUENCY_4,@CUSTOM_FREQUENCY_5,@CUSTOM_FREQUENCY_6,@CUSTOM_FREQUENCY_7,@LATITUDE,@LONGITUDE,@ID_USER_ASSIGNED) ";
					cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

					cmd.Parameters.Clear();
					_line = "40";
					cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
					cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
					cmd.Parameters.AddWithValue("@ID_PROFILE", _objSign.Id_Profile);
					DateTime _df = Convert.ToDateTime(_params.Date_from.ToString());
					cmd.Parameters.AddWithValue("@DATE_FROM", _df.ToString("yyyy-MM-dd"));
					DateTime _dt = Convert.ToDateTime(_params.Date_to.ToString());
					cmd.Parameters.AddWithValue("@DATE_FROM", _dt.ToString("yyyy-MM-dd"));
					cmd.Parameters.AddWithValue("@ADDRESS", _params.Address);
					cmd.Parameters.AddWithValue("@RAW_DATA", _params.Raw_data);
					cmd.Parameters.AddWithValue("@DETAILS", _params.Details);
					cmd.Parameters.AddWithValue("@EXTERNALID", _params.ExternalId);
					cmd.Parameters.AddWithValue("@EXTERNAL_RESPONSE", _params.External_response);
					cmd.Parameters.AddWithValue("@ID_TYPE_FREQUENCY", _params.Id_type_frequency);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_1", _params.Custom_frequency_1);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_2", _params.Custom_frequency_2);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_3", _params.Custom_frequency_3);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_4", _params.Custom_frequency_4);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_5", _params.Custom_frequency_5);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_6", _params.Custom_frequency_6);
					cmd.Parameters.AddWithValue("@CUSTOM_FREQUENCY_7", _params.Custom_frequency_7);
					cmd.Parameters.AddWithValue("@LATITUDE", _params.Latitude);
					cmd.Parameters.AddWithValue("@LONGITUDE", _params.Longitude);
					cmd.Parameters.AddWithValue("@ID_USER_ASSIGNED", _params.Id_user_assigned);

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
						_where += " AND (description LIKE '%@SEARCH%' OR externalid LIKE '%@SEARCH%') ";
					}
				}
			}
			if (_params.Date_from.HasValue) { _where += " AND (date_from >=@DATE_FROM) "; }
			if (_params.Date_to.HasValue) { _where += " AND (date_to <=@DATE_TO) "; }
			if (_params.Id_Type_Frequency != 0 && _params.Id_Type_Frequency.HasValue) { _where += " AND id_type_frequency=@ID_TYPE_FREQUENCY "; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);

				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_schedule_vw_events ";
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
				if (_params.Id_Type_Status != 0 && _params.Id_Type_Status.HasValue)
				{
					cmd.Parameters.AddWithValue("@ID_TYPE_FREQUENCY", _params.Id_Type_Frequency);
				}
				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.TotalPages = (int)dtResponse.Rows[0]["total"] / _response.PageSize + 1;

				cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS CI_rownum, * ";
				cmd.CommandText += " FROM dbo.mod_schedule_vw_events ";
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
				_response.Trace = "NeoSchedule";
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
