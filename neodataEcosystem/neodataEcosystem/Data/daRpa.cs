using RestSharp;
using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{
	public class daRpa : dataBaseSuperClass
	{
		public daRpa() : base("neo_rpa") { }
		public outBaseAnyResponse Robots(inSearch _params)
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

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_rpa_vw_robots_status ";
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
				cmd.CommandText += " FROM dbo.mod_rpa_vw_robots_status ";
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
		public async Task<outBaseAnyResponse> StartAsync(inRpaStart _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);

			inRpaStartPuppeter _puppeteer = new inRpaStartPuppeter();
			_puppeteer.Id = _params.Id_robot;
			_puppeteer.Id_profile = Convert.ToInt32(dtResponse.Rows[0]["id"]);
			_puppeteer.HideNavigator = _params.HideNavigator;
			_puppeteer.Data = _params.Data;

			var clientR = new RestClient((_configServers.Servers["Puppeteer"].url));
			var requestR = new RestRequest("StartProcess");
			requestR.Method = Method.Post;
			requestR.AddHeader("Content-Type", "application/json");
			requestR.AddBody(JsonConvert.SerializeObject(_puppeteer));

			var responseR = await clientR.ExecutePostAsync(requestR);
			JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
			inRpaStartResponse _responseR = JsonConvert.DeserializeObject<inRpaStartResponse>(responseR.Content, jsonSetting);

			if (_responseR.status != "OK") { throw new Exception(_responseR.message); }

			#region Automatic Log
			WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", "");
			#endregion
			return _response;
		}
		public outBaseAnyResponse Stop(inRpa _params)
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				outBaseAnyResponse _response = new outBaseAnyResponse();
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "UPDATE dbo.mod_rpa_robots SET ";
				cmd.CommandText += " end_last_thread=getdate(),last_error='Operación cancelada por el usuario' ";
				cmd.CommandText += " WHERE ";
				cmd.CommandText += " id_last_thread=@ID_LAST_THREAD ";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_LAST_THREAD", _params.Id_thread);
				_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());

				cmd.CommandText = "UPDATE dbo.mod_rpa_threads SET ";
				cmd.CommandText += " offline=getdate(),fum=getdate(),[end]=getdate(),";
				cmd.CommandText += " error='Operación cancelada por el usuario',";
				cmd.CommandText += " fum_error=getdate(),screenshot=null,fum_screenshot=getdate()";
				cmd.CommandText += " WHERE ";
				cmd.CommandText += " id=@ID";
				cmd.CommandType = CommandType.Text;

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID", _params.Id_thread);
				_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion

				return _response;
			}
		}
		public List<Dictionary<string, object>> Status(inRpa _params)
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);

				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT * FROM dbo.mod_rpa_vw_steps WHERE ";
				cmd.CommandText += " offline IS null AND require_input=1 AND ";
				cmd.CommandText += " id_robot IN ";
				cmd.CommandText += "  (";
				cmd.CommandText += "     SELECT id FROM mod_rpa_vw_robots WHERE ";
				cmd.CommandText += "        offline IS null AND ";
				cmd.CommandText += "        id_profile=@ID_PROFILE AND ";
				cmd.CommandText += "        thread_id_profile IS null AND thread_id_profile=@ID_PROFILE";
				if (_params.Id_robot != 0) { cmd.CommandText += " AND id=@ID_ROBOT"; }
				cmd.CommandText += "  )";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_PROFILE", Convert.ToInt32(dtResponse.Rows[0]["id"]));
				if (_params.Id_robot != 0) { cmd.Parameters.AddWithValue("@ID_ROBOT", _params.Id_robot); }

				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				List<Dictionary<string, object>> _records = DataTableToList(dtResponse);

				connection.Close();
				return _records;
			}
		}
		public outBaseAnyResponse Values(inRpa _params)
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				outBaseAnyResponse _response = new outBaseAnyResponse();
				connection.Open();

				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "UPDATE INTO dbo.mod_rpa_robots SET ";
				cmd.CommandText += " end_last_thread=getdate(),last_error='Operación cancelada por el usuario' ";
				cmd.CommandText += " WHERE ";
				cmd.CommandText += " id_last_thread=@ID_LAST_THREAD) ";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_LAST_THREAD", _params.Id_thread);
				_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());

				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion

				return _response;
			}
		}
		public List<Dictionary<string, object>> Captured(inRpa _params)
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _DataContext.DefaultDatabase);

				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT * FROM dbo.mod_rpa_vw_captured_values WHERE ";
				cmd.CommandText += " id_robot=@ID_ROBOT AND ";
				cmd.CommandText += " datediff(week,created,getdate())<=1 ";
				cmd.CommandText += " ORDER BY created DESC, priority_step ASC";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_ROBOT", _params.Id_robot);

				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				List<Dictionary<string, object>> _records = DataTableToList(dtResponse);

				connection.Close();
				return _records;
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
				_response.Trace = "NeoRPA";
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
