namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neoafip.v1")]
	public class AfipController : ControllerBase
	{
		ruAfip _ru = new ruAfip();

		[HttpGet("Status")]
		public outBaseAnyResponse Status()
		{
			return _ru.Status();
		}
		[HttpPost("Invoice")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<outBaseAnyResponse>> Invoice([FromForm] inAfip _params)
		{
			try
			{
				return Ok(await _ru.Invoice(_params, Request.Headers.Host.ToString()));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
		[HttpPost("CreditNote")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<outBaseAnyResponse>> CreditNote([FromForm] inAfip _params)
		{
			try
			{
				return Ok(await _ru.CreditNote(_params, Request.Headers.Host.ToString()));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
	}
}
