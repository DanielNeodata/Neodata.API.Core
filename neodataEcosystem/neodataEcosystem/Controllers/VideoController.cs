namespace neodataEcosystem.Controllers
{
	[ApiController]
	[Route("neovideo.v1")]
	public class VideoController : ControllerBase
	{
		ruVideo _ru = new ruVideo();

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

		[HttpPost("CreateNewVideoRoom")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> CreateNewVideoRoom([FromForm] inVideo _params)
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

		[HttpPost("ListAvailableVideoRooms")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> ListAvailableVideoRooms([FromForm] inBaseAnyRequest _params)
		{
			try
			{
				return Ok(
					new ruLookups(_ru.KeyDatabase, "mod_transactions_vw_transactions").Select(
					new inLookups
					{
						Where = "id_type_status IN (1,2,3) AND ISNULL(live,0)!=1 AND id_application=" + _params.Id_application.ToString() + " AND id_user=" + _params.Id_user.ToString(),
						Order = "created DESC"
					}));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("ListAvailableLiveStreaming")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> ListAvailableLiveStreaming([FromForm] inBaseAnyRequest _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_transactions_vw_transactions").Select(
					new inLookups
					{
						Where = "id_type_status IN (1,2,3) AND live=1",
						Order = "created DESC"
					}));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("JoinOpenSession")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> JoinOpenSession([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(_ru.JoinOpenSession(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("CloseOpenSession")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> CloseOpenSession([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(_ru.CloseOpenSession(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("CleanOrphanSessions")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> CleanOrphanSessions([FromForm] inVideo _params)
		{
			try
			{
				return Ok(_ru.CleanOrphanSessions(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("UpdateVideoHost")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> UpdateVideoHost([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(_ru.UpdateVideoHost(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("UpdateVideoGuest")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> UpdateVideoGuest([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(_ru.UpdateVideoHost(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("GetWrtcOfferAnswer")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> GetWrtcOfferAnswer([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_transactions_wrtc").Select(new inLookups { Fields = "id, raw_answer", Where = ("id_transaction=" + _params.Id_transaction.ToString()) }));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("UpdateWrtcAnswer")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> UpdateWrtcAnswer([FromForm] inVideoWrtcAnswer _params)
		{
			try
			{
				return Ok(_ru.UpdateWrtcAnswer(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("SendRelatedData")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> SendRelatedData([FromForm] inVideoData _params)
		{
			try
			{
				return Ok(_ru.SendRelatedData(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("DeleteDataInClient")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> DeleteDataInClient([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(_ru.DeleteDataInClient(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("ReceiveRelatedData")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> ReceiveRelatedData([FromForm] inVideoReceive _params)
		{
			try
			{
				return Ok(_ru.ReceiveRelatedData(_params));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPost("GetRelatedData")]
		[ProducesResponseType(typeof(outBaseAnyTable), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<outBaseAnyTable> GetRelatedData([FromForm] inVideoIn _params)
		{
			try
			{
				return Ok(new ruLookups(_ru.KeyDatabase, "mod_transactions_data_transactions").Select(
					new inLookups
					{
						Where = "id_transaction=" + _params.Id_transaction,
						Order = "created DESC"
					}));
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
