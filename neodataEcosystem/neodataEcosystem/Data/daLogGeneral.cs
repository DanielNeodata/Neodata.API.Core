using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{

	public class daLogGeneral : dataBaseSuperClass
	{
		public daLogGeneral() : base("neo_logs") { }
		public outBaseAnyResponse Create(inLogs _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			if (_params.Action == null) { _params.Action = ""; }
			if (_params.Trace == null) { _params.Trace = ""; }
			if (_params.Val_rel == null) { _params.Val_rel = ""; }
			if (_params.Field_rel == null) { _params.Field_rel = ""; }
			if (_params.Command_rel == null) { _params.Command_rel = ""; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_backend_type_active_logs ";
				cmd.CommandText += " WHERE code=@ACTION";
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ACTION", _params.Action);
				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				if ((int)dtResponse.Rows[0]["total"] > 0)
				{
					#region Insert logGeneral!
					cmd.CommandText = "INSERT INTO dbo.mod_backend_log_general ";
					cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_application,id_user,id_type_log,";
					cmd.CommandText += " action,trace,val_rel,field_rel,command_rel) ";
					cmd.CommandText += " VALUES ";
					cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Log Neodata Ecosystem") + ",@ID_APPLICATION,@ID_USER,@ID_TYPE_LOG, ";
					cmd.CommandText += " @ACTION,@TRACE,@VAL_REL,@FIELD_REL,@COMMAND_REL) ";
					cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
					cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
					cmd.Parameters.AddWithValue("@ID_TYPE_LOG", _params.Id_type_log);
					cmd.Parameters.AddWithValue("@ACTION", _params.Action);
					cmd.Parameters.AddWithValue("@TRACE", _params.Trace);
					cmd.Parameters.AddWithValue("@VAL_REL", _params.Val_rel);
					cmd.Parameters.AddWithValue("@FIELD_REL", _params.Field_rel);
					cmd.Parameters.AddWithValue("@COMMAND_REL", _params.Command_rel);

					_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
					if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
					#endregion
				}
				connection.Close();
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
			string _where = "";
			if (_params.Id != 0 && _params.Id.HasValue)
			{
				if (_where == "") { _where = " WHERE "; } else { _where += " AND "; }
				_where += " id=@ID ";
			}
			else
			{
				if (!String.IsNullOrEmpty(_params.Search))
				{
					if (_where == "") { _where = " WHERE "; } else { _where += " AND "; }
					if (Regex.IsMatch(_params.Search, @"^\d+$"))
					{
						_where += " val_rel=@SEARCH ";
					}
					else
					{
						_where += " (action LIKE '%@SEARCH%' OR trace LIKE '%@SEARCH%' OR table_rel LIKE '%@SEARCH%') ";
					}
				}
			}
			if (_params.Date_from.HasValue)
			{
				if (_where == "") { _where = " WHERE "; } else { _where += " AND "; }
				_where += " (created >=@DATE_FROM) ";
			}
			if (_params.Date_to.HasValue)
			{
				if (_where == "") { _where = " WHERE "; } else { _where += " AND "; }
				_where += " (created <=@DATE_TO) ";
			}

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_backend_log_general ";
				if (_where != "") { cmd.CommandText += _where; }
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				if (_params.Id != 0 && _params.Id.HasValue) { cmd.Parameters.AddWithValue("@ID", _params.Id); }
				if (!String.IsNullOrEmpty(_params.Search)) { cmd.Parameters.AddWithValue("@SEARCH", _params.Search); }
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

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.TotalPages = (int)dtResponse.Rows[0]["total"] / _response.PageSize + 1;

				cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS CI_rownum, * ";
				cmd.CommandText += " FROM dbo.mod_backend_log_general ";
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
				DataTable dtResponse = new DataTable();
				_response.Records = DataTableToList(dtResponse);
				_response.Logic = true;
				_response.Numeric = 0;
				_response.Message = "La plataforma está operativa y sus credenciales válidas.";
				_response.Trace = "NeoLogs";
				_response.Scope = "V1";
			}
			return _response;
		}
	}
}
