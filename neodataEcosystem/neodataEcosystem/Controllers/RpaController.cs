namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neorpa.v1")]
	public class RpaController : ControllerBase
	{
		ruRpa _ru = new ruRpa();

		[HttpPost("Robots")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Robots([FromForm] inSearch _params)
		{
			try
			{
				return Ok(_ru.Robots(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Start")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<outBaseAnyResponse>> Start([FromForm] inRpaStart _params)
		{
			try
			{
				return Ok(await _ru.Start(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
		[HttpPost("Stop")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Stop([FromForm] inRpa _params)
		{
			try
			{
				return Ok(_ru.Stop(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Status")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Status([FromForm] inRpa _params)
		{
			try
			{
				return Ok(_ru.Status(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Values")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Values([FromForm] inRpa _params)
		{
			try
			{
				return Ok(_ru.Values(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Captured")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Captured([FromForm] inRpa _params)
		{
			try
			{
				return Ok(_ru.Captured(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("TypeEvaluables")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> TypeEvaluables([FromForm] inLookups _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_rpa_type_evaluables").Select(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("TypeSteps")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> TypeSteps([FromForm] inLookups _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_rpa_type_steps").Select(_params));
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
				return Ok(_ru.Robots(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
