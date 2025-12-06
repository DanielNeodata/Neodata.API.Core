using AfipServiceReference;
using AfipWsfeClient;

namespace neodataEcosystem.Data
{
	public class daAfip : dataBaseSuperClass
	{
		public daAfip() : base("neo_afip") { }
		public outBaseAnyResponse Status()
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			_response.Scope = "Status";
			_response.Message = "El servidor está operativo";
			_response.Trace = "Status";
			return _response;
		}

		private async Task<WsfeClient> getWsfeClient(inAfip _params, string _host)
		{
			LoginCmsClient loginClient = new LoginCmsClient { IsProdEnvironment = (_params.IsProdEnvironment == 1) };
			string _pathP12 = "./Application/config/certificates/afip/afip.p12";
			if (_params.IsProdEnvironment == 1)
			{
				switch (_params.CUIT)
				{
					case 20214784069:
						_pathP12 = "./application/config/certificates/afip/20214784069/sergio.p12";
						break;
					case 20176342502:
						_pathP12 = "./application/config/certificates/afip/20176342502/ruben.p12";
						break;
					case 30712162712:
						_pathP12 = "./application/config/certificates/afip/30712162712/sanisidro.p12";
						break;
					case 20207347966:
						_pathP12 = "./application/config/certificates/afip/20207347966/daniel.p12";
						break;
				}
			}
			WsaaTicket ticket = await loginClient.LoginCmsAsync("wsfe", _pathP12, "55AraucariA", true);
			return new WsfeClient { IsProdEnvironment = (_params.IsProdEnvironment == 1), Cuit = _params.CUIT, Sign = ticket.Sign, Token = ticket.Token };
		}
		public async Task<outBaseAnyResponse> Invoice(inAfip _params, string _host)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			try
			{
				/*Inicializar cliente*/
				WsfeClient wsfeClient = await getWsfeClient(_params, _host);

				/*Traer último comprobante y asignar nuevo número*/
				FECompUltimoAutorizadoResponse _ret = await wsfeClient.FECompUltimoAutorizadoAsync(_params.SalesPoint, _params.VoucherType);
				int compNumber = (_ret.Body.FECompUltimoAutorizadoResult.CbteNro + 1);

				/*Armado del detalle del item para enviar en el request*/
				FECAEDetRequest _FeDetReqItem = new FECAEDetRequest
				{
					CbteDesde = compNumber,
					CbteHasta = compNumber,
					CbteFch = _params.InvoiceCreationDate,
					Concepto = _params.Concept,
					DocNro = _params.DocumentNumber,
					DocTipo = _params.DocumentType,
					FchVtoPago = _params.InvoicePayDate,
					ImpNeto = _params.AmountNet,
					ImpTotal = _params.AmountTotal,
					FchServDesde = _params.InvoiceCreationDate,
					FchServHasta = _params.InvoiceCreationDate,
					MonCotiz = _params.CurrencyChange,
					MonId = _params.CurrencyId,
					ImpIVA = _params.AmountIva
				};

				/*Armado de items de iva si corresponde*/
				if (_params.AmountIva != 0)
				{
					List<AlicIva> _Iva = new List<AlicIva>();
					_Iva.Add(new AlicIva { BaseImp = _params.AmountNet, Id = 5, Importe = _params.AmountIva });
					_FeDetReqItem.Iva = _Iva;
				}

				/*Agregar cabecera del request*/
				FECAECabRequest _FeCabReq = new FECAECabRequest
				{
					CantReg = _params.Records,
					CbteTipo = _params.VoucherType,
					PtoVta = _params.SalesPoint
				};

				/*Agregar detalle del request*/
				List<FECAEDetRequest> _FeDetReq = new List<FECAEDetRequest>();
				_FeDetReq.Add(_FeDetReqItem);

				/*Armar la llamada final para procesar*/
				FECAERequest feCaeReq = new FECAERequest
				{
					FeCabReq = _FeCabReq,
					FeDetReq = _FeDetReq
				};

				/*Ejectuar y luego evaluar la respuesta*/
				FECAESolicitarResponse compResult = await wsfeClient.FECAESolicitarAsync(feCaeReq);
				List<Dictionary<string, object>> _records = new List<Dictionary<string, object>>();
				Dictionary<string, object> _item = new Dictionary<string, object>();
				_item.Add("CAE", compResult.Body.FECAESolicitarResult.FeDetResp[0].CAE);
				_item.Add("CAEFchVto", compResult.Body.FECAESolicitarResult.FeDetResp[0].CAEFchVto);
				_item.Add("Concepto", compResult.Body.FECAESolicitarResult.FeDetResp[0].Concepto);
				_item.Add("Resultado", compResult.Body.FECAESolicitarResult.FeDetResp[0].Resultado);
				_item.Add("CbteDesde", compResult.Body.FECAESolicitarResult.FeDetResp[0].CbteDesde);
				_item.Add("CbteHasta", compResult.Body.FECAESolicitarResult.FeDetResp[0].CbteHasta);
				_item.Add("CbteFch", compResult.Body.FECAESolicitarResult.FeDetResp[0].CbteFch);
				_item.Add("DocNro", compResult.Body.FECAESolicitarResult.FeDetResp[0].DocNro);
				_item.Add("DocTipo", compResult.Body.FECAESolicitarResult.FeDetResp[0].DocTipo);
				_item.Add("Observaciones", compResult.Body.FECAESolicitarResult.FeDetResp[0].Observaciones);
				_records.Add(_item);
				_response.Records = _records;
				_response.Logic = true;
			}
			catch (Exception err)
			{
				_response.Logic = false;
				_response.Message = err.Message;
			}
			return _response;
		}
		public async Task<outBaseAnyResponse> CreditNote(inAfip _params, string _host)
		{
			outBaseAnyResponse _response = new outBaseAnyResponse();
			try
			{
				/*Inicializar cliente*/
				WsfeClient wsfeClient = await getWsfeClient(_params, _host);

				/*Traer último comprobante y asignar nuevo número*/
				FECompUltimoAutorizadoResponse _ret = await wsfeClient.FECompUltimoAutorizadoAsync(_params.SalesPoint, _params.VoucherType);
				int compNumber = (_ret.Body.FECompUltimoAutorizadoResult.CbteNro + 1);

				/*Armado del detalle del item para enviar en el request*/
				FECAEDetRequest _FeDetReqItem = new FECAEDetRequest
				{
					CbteDesde = compNumber,
					CbteHasta = compNumber,
					CbteFch = _params.InvoiceCreationDate,
					Concepto = _params.Concept,
					DocNro = _params.DocumentNumber,
					DocTipo = _params.DocumentType,
					FchVtoPago = _params.InvoicePayDate,
					ImpNeto = _params.AmountNet,
					ImpTotal = _params.AmountTotal,
					FchServDesde = _params.InvoiceCreationDate,
					FchServHasta = _params.InvoiceCreationDate,
					MonCotiz = _params.CurrencyChange,
					MonId = _params.CurrencyId,
					ImpIVA = _params.AmountIva
				};

				/*Armado de items de iva si corresponde*/
				if (_params.AmountIva != 0)
				{
					List<AlicIva> _Iva = new List<AlicIva>();
					_Iva.Add(new AlicIva { BaseImp = _params.AmountNet, Id = 5, Importe = _params.AmountIva });
					_FeDetReqItem.Iva = _Iva;
				}

				/*Agregar cabecera del request*/
				FECAECabRequest _FeCabReq = new FECAECabRequest
				{
					CantReg = _params.Records,
					CbteTipo = _params.VoucherType,
					PtoVta = _params.SalesPoint
				};

				/*Agregar detalle del request*/
				List<FECAEDetRequest> _FeDetReq = new List<FECAEDetRequest>();
				_FeDetReq.Add(_FeDetReqItem);

				/*Armar la llamada final para procesar*/
				FECAERequest feCaeReq = new FECAERequest
				{
					FeCabReq = _FeCabReq,
					FeDetReq = _FeDetReq
				};

				/*Agregar comprobante original asociado a la nc*/
				CbteAsoc _comprobante = new CbteAsoc();
				_comprobante.CbteFch = _params.OriginalDate;
				_comprobante.PtoVta = (Int32)_params.OriginalInvoiceSalesPoint;
				_comprobante.Nro = (Int32)_params.OriginalInvoiceNumber;
				_comprobante.Tipo = (Int32)_params.OriginalVoucherType;
				_comprobante.Cuit = _params.OriginalDocumentNumber.ToString();

				List<CbteAsoc> _FeCbteAsoc = new List<CbteAsoc>();
				_FeCbteAsoc.Add(_comprobante);
				_FeDetReqItem.CbtesAsoc = _FeCbteAsoc;

				/*Ejectuar y luego evaluar la respuesta*/
				FECAESolicitarResponse compResult = await wsfeClient.FECAESolicitarAsync(feCaeReq);
				List<Dictionary<string, object>> _records = new List<Dictionary<string, object>>();
				Dictionary<string, object> _item = new Dictionary<string, object>();
				_item.Add("CAE", compResult.Body.FECAESolicitarResult.FeDetResp[0].CAE);
				_item.Add("CAEFchVto", compResult.Body.FECAESolicitarResult.FeDetResp[0].CAEFchVto);
				_item.Add("Concepto", compResult.Body.FECAESolicitarResult.FeDetResp[0].Concepto);
				_item.Add("Resultado", compResult.Body.FECAESolicitarResult.FeDetResp[0].Resultado);
				_item.Add("CbteDesde", compResult.Body.FECAESolicitarResult.FeDetResp[0].CbteDesde);
				_item.Add("CbteHasta", compResult.Body.FECAESolicitarResult.FeDetResp[0].CbteHasta);
				_item.Add("CbteFch", compResult.Body.FECAESolicitarResult.FeDetResp[0].CbteFch);
				_item.Add("DocNro", compResult.Body.FECAESolicitarResult.FeDetResp[0].DocNro);
				_item.Add("DocTipo", compResult.Body.FECAESolicitarResult.FeDetResp[0].DocTipo);
				_item.Add("Observaciones", compResult.Body.FECAESolicitarResult.FeDetResp[0].Observaciones);
				_records.Add(_item);
				_response.Records = _records;
				_response.Logic = true;
			}
			catch (Exception err)
			{
				_response.Logic = false;
				_response.Message = err.Message;
			}
			return _response;
		}
	}
}
