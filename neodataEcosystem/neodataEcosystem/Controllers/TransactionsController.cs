namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neotransactions.v1")]
	public class TransactionsController : ControllerBase
	{
		ruTransactions _ru = new ruTransactions();

        [HttpPost("CreateNoSign")]
        [ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<outBaseAnyResponse> CreateNoSign([FromForm] inTransactions _params)
        {
            try
            {
                return Ok(_ru.Create(_params,true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("Create")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Create([FromForm] inTransactions _params)
		{
			try
			{
				return Ok(_ru.Create(_params,false));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("Search")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> Search([FromForm] inSearch _params)
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

		[HttpPost("TypeStatus")]
		[ProducesResponseType(typeof(outBaseAnyResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyResponse> TypeStatus([FromForm] inLookups _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_transactions_type_status").Select(_params));
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
