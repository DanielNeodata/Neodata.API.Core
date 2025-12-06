namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neologs.v1")]
	public class LogController : ControllerBase
	{
		ruLogGeneral _ru = new ruLogGeneral();

		[HttpPost("Create")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Create([FromForm] inLogs _params)
		{
			try
			{
				return Ok(_ru.Create(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("TypeLogs")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> TypeLogs([FromForm] inLookups _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_backend_type_logs").Select(_params));
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
				return Ok(_ru.Search(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

	}
}
