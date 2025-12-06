using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using neoTransactions.Interfaces.User;
using neoTransactions.Rules.User;

namespace neoTransactions.Controllers
{
    [ApiController]
    [Route("api.v1/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpPost("Authenticate")]
        [ProducesResponseType(typeof(outAuthenticateUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<outAuthenticateUser> Authenticate(inAuthenticateUser _params)
        {
            try
            {
                return Ok(new ruAuthenticateUser().Execute(_params));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
