namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neoauthentication.v1")]
	public class AuthenticationController : ControllerBase
	{
		ruAuthentication _ru = new ruAuthentication();

		[HttpPost("Authenticate")]
		[ProducesResponseType(typeof(outAuthentication), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outAuthentication> Authenticate([FromForm] inAuthentication _params)
		{
			try
			{
				return Ok(_ru.Authenticate(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("VerifyTokenSingleUser")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> VerifyTokenSingleUser([FromForm] inToken _params)
		{
			try
			{
				return Ok(_ru.VerifyTokenSingleUser(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("VerifyTokenEthernal")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> VerifyTokenEthernal([FromForm] inToken _params)
		{
			try
			{
				return Ok(_ru.VerifyTokenEthernal(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("DestroyToken")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> DestroyToken([FromForm] inToken _params)
		{
			try
			{
				return Ok(_ru.Destroy(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("TypeUsers")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> TypeUsers([FromForm] inLookups _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_backend_type_users").Select(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("State")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> State([FromForm] inBaseAnyRequest _params)
		{
			try
			{
				return Ok(_ru.State(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Monitoring")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Monitoring([FromForm] inSearch _params)
		{
			try
			{
				return Ok(_ru.Monitoring(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
