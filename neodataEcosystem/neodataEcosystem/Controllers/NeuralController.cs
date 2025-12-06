namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neoneural.v1")]
	public class NeuralController : ControllerBase
	{
		ruNeural _ru = new ruNeural();

		[HttpPost("Resolve")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Resolve([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.Resolve(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("SynapsesMatrix")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> SynapsesMatrix([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.SynapsesMatrix(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Trainning")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Trainning([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.Trainning(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Questions")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Questions([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.Questions(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Projects")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Projects([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.Projects(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("ResetProject")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> ResetProject([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.ResetProject(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("AddItemforTranning")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> AddItemforTranning([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.AddItemforTranning(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("AddItemResponseforTranning")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> AddItemResponseforTranning([FromForm] inNeural _params)
		{
			try
			{
				return Ok(_ru.AddItemResponseforTranning(_params));
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
	}
}
