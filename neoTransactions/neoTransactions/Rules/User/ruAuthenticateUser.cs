using System;
using System.Net;
using neoTools;
using neoTransactions.Data.User;
using neoTransactions.Interfaces.User;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace neoTransactions.Rules.User
{
    public class ruAuthenticateUser
    {
        public outAuthenticateUser Execute(inAuthenticateUser _params)
        {
            outAuthenticateUser _response = new outAuthenticateUser(new outBaseSuperClass(MethodBase.GetCurrentMethod().Name));
            try
            {
                #region Parsing a validation area
                #endregion

                #region Transformation area 
                #endregion

                #region Calling dataAccess object area
                daAuthenticateUsers _daAuthenticateUsers = new daAuthenticateUsers();
                Task<outAuthenticateUser> _ret = _daAuthenticateUsers.Execute(_params);
                _response = _ret.Result;
                #endregion

                #region Post processing area 
                #endregion
            }
            catch (Exception ex)
            {
                _response.Logic = false;
                _response.Code = "900";
                _response.Error = ex.Message.ToString();
                _response.Status = "ERROR";
                _response.Message = ("Error raised from " + _response.Scope);
                if (ex.InnerException != null) { _response.Trace = ex.InnerException.ToString(); }
            }
            return _response;
        }
    }
}