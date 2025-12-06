namespace neoTools
{
	/* sample call ! just a note!
        //Neodata implementacion daConfiguration!
        daConfiguration _config = new daConfiguration(daConfiguration.typeConfig.Servers);
        //_config.Servers["Common"].url // como se acceden a los valores cargados en la configuracion 
    */
	public class neoConfiguration
	{
		/*Define all accesible interfaces via class instancing*/
		/*must be modified or extended with each implementation*/
		public Dictionary<string, tConfigTokens> Tokens { get; set; }
		public Dictionary<string, tConfigDatabases> Databases { get; set; }
		public Dictionary<string, tConfigServer> Servers { get; set; }
		public Dictionary<string, tConfigEmailing> Emailing { get; set; }
		public List<string> TokensProperties { get; set; }
		public List<string> ServersProperties { get; set; }
		public List<string> EmailingProperties { get; set; }
		public List<string> DatabasesProperties { get; set; }

		/*enum for controlling parametrized calls*/
		/*must be modified or extended with each implementation*/
		public enum typeConfig
		{
			All = 0,
			Servers = 1,
			Emailing = 2,
			Databases = 3,
			Tokens = 4
		}

		/* Class Contructor, the idea is resolve ALL in the class instancing */
		/* We look the minimum path for external configuration */
		public neoConfiguration(typeConfig _typeconfig)
		{
			try
			{
				switch (_typeconfig)
				{
					case typeConfig.All:
						LoadAll();
						break;
					case typeConfig.Servers:
						LoadServers();
						break;
					case typeConfig.Emailing:
						LoadEmailing();
						break;
					case typeConfig.Databases:
						LoadDatabases();
						break;
					case typeConfig.Tokens:
						LoadTokens();
						break;
				}
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		/*Private call for retrieve properties of a given class instance*/
		/*DON'T TOUCH!*/
		private List<string> ReflexProperties(Object _obj)
		{
			List<string> ret = new List<string>();
			foreach (PropertyInfo p in _obj.GetType().GetProperties()) { ret.Add(p.Name); }
			return ret;
		}

		/*Define all calls for loading each config external file*/
		/*must be modified or extended with each implementation*/
		private void LoadAll()
		{
			try
			{
				LoadServers();
				LoadEmailing();
				LoadDatabases();
				LoadTokens();
			}
			catch (Exception err)
			{
				throw err;
			}
		}
		private void LoadDatabases()
		{
			DatabasesProperties = ReflexProperties(new tConfigDatabases());
			tConfigDatabases[]? tDatabases;
			//string path = "./Resources/configDatabases.json";
			string path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\configDatabases.json");
			if (!File.Exists(path)) { throw new Exception("Path not found: " + path); }
			var json = File.ReadAllText(path);
			tDatabases = JsonConvert.DeserializeObject<tConfigDatabases[]>(json);
			Databases = new Dictionary<string, tConfigDatabases>();
			foreach (tConfigDatabases obj in tDatabases) { Databases.Add(obj.key, obj); }
		}
		private void LoadServers()
		{
			ServersProperties = ReflexProperties(new tConfigServer());
			tConfigServer[]? tServers;
			//string path = "./Resources/configServers.json";
			string path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\configServers.json");
			if (!File.Exists(path)) { throw new Exception("Path not found: " + path); }
			var json = File.ReadAllText(path);
			tServers = JsonConvert.DeserializeObject<tConfigServer[]>(json);
			Servers = new Dictionary<string, tConfigServer>();
			foreach (tConfigServer obj in tServers) { Servers.Add(obj.key, obj); }
		}
		private void LoadEmailing()
		{
			EmailingProperties = ReflexProperties(new tConfigEmailing());
			tConfigEmailing[]? tEmailing;
			//string path = "./Resources/configEmailing.json";
			string path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\configEmailing.json");
			if (!File.Exists(path)) { throw new Exception("Path not found: " + path); }
			var json = File.ReadAllText(path);
			tEmailing = JsonConvert.DeserializeObject<tConfigEmailing[]>(json);
			Emailing = new Dictionary<string, tConfigEmailing>();
			foreach (tConfigEmailing obj in tEmailing) { Emailing.Add(obj.key, obj); }
		}
		private void LoadTokens()
		{
			TokensProperties = ReflexProperties(new tConfigTokens());
			tConfigTokens[]? tTokens;
			//string path = "./Resources/configTokens.json";
			string path = (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\configTokens.json");

			if (!File.Exists(path)) { throw new Exception("Path not found: " + path); }
			var json = File.ReadAllText(path);
			tTokens = JsonConvert.DeserializeObject<tConfigTokens[]>(json);
			Tokens = new Dictionary<string, tConfigTokens>();
			foreach (tConfigTokens obj in tTokens) { Tokens.Add(obj.key, obj); }
		}

	}

	/*Define all config files structures*/
	/*must be modified or extended with each implementation*/
	public class tConfigDatabases
	{
		public string? key { get; set; }
		public string? server { get; set; }
		public string? username { get; set; }
		public string? password { get; set; }
		public string? database { get; set; }
	}
	public class tConfigServer
	{
		public string? key { get; set; }
		public string? url { get; set; }
		public string? username { get; set; }
		public string? password { get; set; }
	}
	public class tConfigEmailing
	{
		public string? key { get; set; }
		public string? server { get; set; }
		public string? username { get; set; }
		public string? password { get; set; }
	}
	public class tConfigTokens
	{
		public string? key { get; set; }
		public int seconds { get; set; }
		public string? type { get; set; }
		public bool ignore { get; set; }
	}
}
