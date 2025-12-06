namespace neodataEcosystem.SuperClasses
{
	public class dataBaseSuperClass
	{
		public neoToken _Token = new neoToken();
		public neoCrypto _Crypto = new neoCrypto();
		public neoHelper _Helper = new neoHelper();
		public neoConfiguration _configServers = new neoConfiguration(neoConfiguration.typeConfig.Servers);
		public dataBaseSuperClass(string _keyDatabase)
		{
			try
			{
				_DataContext = new neoDataContext(_keyDatabase);
			}
			catch (Exception ex)
			{
				throw new Exception("dataBaseSuperClass: " + ex.Message);
			}
		}
		public neoDataContext _DataContext { get; set; }
		public List<Dictionary<string, object>> DataTableToList(DataTable dtResponse)
		{
			List<Dictionary<string, object>> _table = new List<Dictionary<string, object>>();

			foreach (DataRow row in dtResponse.Rows)
			{
				Dictionary<string, object> _record = new Dictionary<string, object>();
				for (int j = 0; j < dtResponse.Columns.Count; j++)
				{
					string _field = dtResponse.Columns[j].ColumnName.ToString();
					object _obj = row[_field];
					if (_obj == DBNull.Value) { _obj = ""; }
					_record.Add(_field, _obj);
				}
				_table.Add(_record);
			}
			return _table;
		}
		public List<Dictionary<string, object>> DoubleListToList(List<double> dtResponse)
		{
			List<Dictionary<string, object>> _table = new List<Dictionary<string, object>>();
			foreach (double row in dtResponse)
			{
				Dictionary<string, object> _record = new Dictionary<string, object>();
				double _obj = Convert.ToDouble(row.ToString());
				_record.Add("Double", _obj);
				_table.Add(_record);
			}
			return _table;
		}
		public DataTable GetProfile(int _id_application, int _id_user, string _keyDatabase)
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.connString[_keyDatabase]))
			{
				DataTable dtResponse = new DataTable();
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT * FROM dbo.mod_backend_profiles WHERE id_application_auth=@ID_APPLICATION AND ";
				cmd.CommandText += "id_user_auth=@ID_USER";
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _id_user);

				dtResponse.Load(cmd.ExecuteReader());
				connection.Close();
				return dtResponse;
			}
		}
		public void WriteLog(int _id_application, int _id_user, string? _action, string? _trace, string? _val_rel, string? _field_rel, string? _command_rel)
		{
			new daLogGeneral().Create(
				new inLogs
				{
					Id_application = _id_application,
					Id_user = _id_user,
					Id_type_log = 1,
					Action = _action,
					Trace = _trace,
					Val_rel = _val_rel,
					Field_rel = _field_rel,
					Command_rel = _command_rel
				});
		}
	}
}
