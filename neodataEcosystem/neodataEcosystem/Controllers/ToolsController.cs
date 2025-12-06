namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neotools.v1")]
	public class ToolsController : ControllerBase
	{
		ruTools _ru = new ruTools();

        [HttpPost("CountPages")]
        [ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> CountPages([FromForm] inPages _params)
        {
            try
            {
                return Ok(_ru.CountPages(_params));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
		[HttpPost("CreateQR")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> CreateQR([FromForm] inQR _params)
		{
			try
			{
				return Ok(_ru.CreateQR(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("CreateBarCode")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> CreateBarCode([FromForm] inBarCode _params)
		{
			try
			{
				return Ok(_ru.CreateBarCode(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
		[HttpGet("QR")]
		public IActionResult QR([FromQuery] string b64)
		{
			return _ru.QR(b64);
		}
		[HttpGet("BarCode")]
		public IActionResult BarCode([FromQuery] string b64)
		{
			return _ru.BarCode(b64);
		}
	}
}
