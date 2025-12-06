using neoTransactions.Interfaces.User;
using neoTransactions.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Routing;
using System;
using System.Data;
using System.Data.SqlClient;
using neoTransactions.Interfaces;
using neoTools;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;

namespace neoTransactions.Data.User
{
    public class daAuthenticateUsers
    {
        neoConfiguration _configServers = new neoConfiguration(neoConfiguration.typeConfig.Servers);
        neoDataContext _DataContext = new neoDataContext("neo_transactions");
        public async Task<outAuthenticateUser> Execute(inAuthenticateUser _params)
        {
            outAuthenticateUser _response = new outAuthenticateUser(new outBaseSuperClass(MethodBase.GetCurrentMethod().Name));
            try
            {
                var clientR = new RestClient((_configServers.Servers["neoAuthentication"].url + "/api.v1/Users/Authenticate"));
                var requestR = new RestRequest();
                requestR.RequestFormat = DataFormat.Json;
                requestR.Method = Method.Post;
                requestR.AddHeader("Content-Type", "application/json");
                requestR.AddJsonBody(JsonConvert.SerializeObject(_params));
                var responseR = await clientR.ExecutePostAsync(requestR);
                JsonSerializerSettings jsonSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                _response = JsonConvert.DeserializeObject<outAuthenticateUser>(responseR.Content, jsonSetting);

            }
            catch (Exception ex)
            {
                _response.Logic = false;
                _response.Code = "901";
                _response.Error = ex.Message.ToString();
                _response.Status = "ERROR";
                _response.Message = ("Error raised from " + _response.Scope);
                if (ex.InnerException != null) { _response.Trace = ex.InnerException.ToString(); }
                //throw new Exception(string.Format("Error en Scoring: {0}", ex.Message), ex.InnerException);
            }
            return _response;
        }
    }
}
