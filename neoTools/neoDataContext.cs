namespace neoTools
{
	public class neoDataContext
	{
		public Dictionary<string, string> connString = new Dictionary<string, string>();
		neoConfiguration _configDatabases = new neoConfiguration(neoConfiguration.typeConfig.Databases);
		public string? DefaultDatabase { get; set; }

		public neoDataContext(string? _DefaultDatabase)
		{
			try
			{
				DefaultDatabase = _DefaultDatabase;
				/*Se definen los diversos servers con las bases de datos a acceder*/
				/*Se definen las diversas connection strings con las bases de datos a acceder*/
				connString.Clear();
				for (int index = 0; index < _configDatabases.Databases.Count; index++)
				{
					var item = _configDatabases.Databases.ElementAt(index);
					tConfigDatabases itemValue = item.Value;
					string str = ("encrypt=false;database=" + itemValue.database + ";server=" + itemValue.server + ";user=" + itemValue.username + ";password=" + itemValue.password + ";MultipleActiveResultSets=True");
					connString.Add(item.Key, str);
				}
			}
			catch (Exception ex)
			{
				throw new Exception("neoDataContext: " + ex.Message);

			}
		}

		public string getDefaultDatabaseConnectionString()
		{
			return connString[DefaultDatabase];
		}
		public string getCommonFields()
		{
			string fields = "code,description,created,verified,offline,fum";
			return fields;
		}
		public string getCommonFieldsValues(string? code, string? description)
		{
			if (code == "" || code == null) { code = Guid.NewGuid().ToString(); }
			if (description == null) { description = ""; }
			string values = ("'" + code + "','" + description + "',getdate(),getdate(),null,getdate()");
			return values;
		}
	}
}
