using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{
	public class daInterfaces : dataBaseSuperClass
	{
		public daInterfaces() : base("neo_interfaces") { }

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

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_interfaces_forms ";
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
				_response.TotalPages = (int)dtResponse.Rows[0]["total"] / _response.PageSize;

				cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS CI_rownum, * ";
				cmd.CommandText += " FROM dbo.mod_interfaces_forms ";
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
				_response.Trace = "NeoInterfaces";
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
