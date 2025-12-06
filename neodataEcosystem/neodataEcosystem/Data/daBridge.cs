using Newtonsoft.Json.Linq;
using RestSharp;

namespace neodataEcosystem.Data
{

	public class daBridge : dataBaseSuperClass
	{
		public daBridge() : base("neo_transactions") { }
		public outBaseAnyResponse Status()
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
			}
			outBaseAnyResponse _response = new outBaseAnyResponse();
			_response.Scope = "Status";
			_response.Message = "El servidor está operativo";
			_response.Trace = "Status";
			return _response;
		}
		public async Task<outBaseAnyResponse> LoyalAsync(inBridge _params)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			if (_params.Id_user == null) { _params.Id_user = 0; }
			if (_params.Id_profile == null) { _params.Id_profile = 0; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT * FROM dbo.mod_transactions_transactions ";
				cmd.CommandText += " WHERE id_type_status=@TYPE_STATUS AND id_user=@ID_USER AND id_profile=@ID_PROFILE";
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@TYPE_STATUS", 1);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_PROFILE", _params.Id_profile);
				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Scope = "LoyalAsync";

				foreach (DataRow row in dtResponse.Rows)
				{
					_response.Numeric = Convert.ToInt32(row["id"]);
					JObject raw_data = JObject.Parse(row["raw_data"].ToString());
					Dictionary<string, string> dictObj = raw_data.ToObject<Dictionary<string, string>>();

					//string _descripcion = raw_data["descripcion"];

					RestClient client = new RestClient(_configServers.Servers["Loyal6"].url);
					RestRequest request = new RestRequest("/loyal6/rest/api/createForm", Method.Post);

					string _raw_body = "{\"userName\":\"" + raw_data["userName"].ToString() + "\"";
					_raw_body += ",\"password\":\"" + raw_data["password"].ToString() + "\"";
					_raw_body += ",\"abstractFormId\":\"" + raw_data["abstractFormId"].ToString() + "\"";
					_raw_body += ",\"formTypeId\":\"" + raw_data["formTypeId"].ToString() + "\"";
					_raw_body += ",\"code\":\"" + row["code"].ToString() + "\"";
					_raw_body += ",\"authorId\":\"" + raw_data["authorId"].ToString() + "\"";
					_raw_body += ",\"companyId\":\"" + raw_data["companyId"].ToString() + "\"";
					_raw_body += ",\"title\":\"" + raw_data["titulo"].ToString() + "\"";
					_raw_body += ",\"scopesCodes\":[\"" + raw_data["scopesCodes"].ToString() + "\"]";
					_raw_body += ",\"references\":[";
					_raw_body += "{\"referenceId\":508,\"values\":[\"" + raw_data["latitude"].ToString() + "\",\"" + raw_data["longitude"].ToString() + "\"]}";
					_raw_body += "]";
					_raw_body += ",\"lifeCycleOption\": { \"type\":\"none\"}";
					_raw_body += "}";
					request.AddHeader("Content-Type", "application/json");
					request.AddStringBody(_raw_body, DataFormat.Json);
					RestResponse response = await client.ExecuteAsync(request);
					string _external_response = response.Content;
					JObject json = JObject.Parse(_external_response);
					_response.Message = json.ToString();
					string _insertedFormId = json["formId"].ToString();

					foreach (var (key, value) in dictObj)
					{
						if (key.ToString().Substring(0, 5) == "file-")
						{
							request = new RestRequest("/loyal6/rest/api/uploadAttach", Method.Put);
							request.AlwaysMultipartFormData = true;

							string[] _fileData = value.Split(",");
							string _ext = _fileData[0].Split("/")[1].Split(";")[0];
							byte[] bytes = Convert.FromBase64String(_fileData[1]);
							string _filename = (key + "." + _ext);
							System.IO.File.WriteAllBytes(_filename, bytes);

							request.AddFile("file", _filename);
							request.AddParameter("userName", raw_data["userName"].ToString());
							request.AddParameter("password", raw_data["password"].ToString());
							request.AddParameter("formId", _insertedFormId);
							request.AddParameter("companyId", raw_data["companyId"].ToString());
							request.AddParameter("replaceFile", "false");
							response = await client.ExecuteAsync(request);
							System.IO.File.Delete(_filename);
						}
					}
					cmd.CommandText = "UPDATE dbo.mod_transactions_transactions SET ";
					cmd.CommandText += " id_type_status=@ID_TYPE_STATUS, ";
					cmd.CommandText += " externalid=@EXTERNALID, ";
					cmd.CommandText += " external_response=@EXTERNAL_RESPONSE ";
					cmd.CommandText += " WHERE id=@ID";
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@ID", _response.Numeric.ToString());
					cmd.Parameters.AddWithValue("@ID_TYPE_STATUS", 4);// processed!
					cmd.Parameters.AddWithValue("@EXTERNALID", _insertedFormId);// INSERTED VALUE IN REMOTE PLATFORM!
					cmd.Parameters.AddWithValue("@EXTERNAL_RESPONSE", _external_response);// REMOTE PLATFORM RAW RESPONSE!
					cmd.ExecuteNonQuery();
				}
				connection.Close();
			}
			return _response;
		}
	}
}
