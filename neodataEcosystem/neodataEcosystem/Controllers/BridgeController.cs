namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neobridge.v1")]
	public class BridgeController : ControllerBase
	{
		ruBridge _ru = new ruBridge();

		[HttpGet("Status")]
		public ActionResult<outBaseAnyResponse> Status()
		{
			try
			{
				return Ok(_ru.Status());
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
		[HttpGet("Loyal")]
		public async Task<outBaseAnyResponse> Loyal([FromQuery] inBridge _params)
		{
			return await _ru.LoyalAsync(_params);
		}
	}
}
