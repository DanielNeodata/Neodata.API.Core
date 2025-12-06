namespace neodataEcosystem.Data
{
	public class daLookups : dataBaseSuperClass
	{
		neoHelper _Helper = new neoHelper();
		public daLookups(string _keyDatabase) : base(_keyDatabase) { }
		public List<Dictionary<string, object>> Select(inLookups _params, string TableView)
		{
			if (_params.Fields == null) { _params.Fields = ""; }
			if (_params.Fields == "") { _params.Fields = "*"; }
			if (_params.Where == null) { _params.Where = ""; }
			if (_params.Order == null) { _params.Order = ""; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT " + _params.Fields + " FROM " + TableView;
				if (_params.Where != "") { cmd.CommandText += " WHERE " + _params.Where; }
				if (_params.Order != "") { cmd.CommandText += " ORDER BY " + _params.Order; }

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				List<Dictionary<string, object>> _records = DataTableToList(dtResponse);

				connection.Close();
				return _records;
			}
		}
	}
}