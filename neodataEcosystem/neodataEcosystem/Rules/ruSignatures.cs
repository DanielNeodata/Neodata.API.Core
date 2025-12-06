using Microsoft.IdentityModel.Tokens;
using SelectPdf;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;

namespace neodataEcosystem.Rules
{
	public class ruSignatures : rulesBaseSuperClass
	{
		daSignatures _da = new daSignatures();
		public ruSignatures() : base("neo_signature") { }
		public outBaseAnyResponse Create(inSignatures _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Create(_params);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public outBaseAnyResponse Search(inSearch _params)
		{
			try
			{
				outBaseAnyResponse _verify = VerifyAndUseTokenRule(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				_response = _da.Search(_params);
				_response.Trace = "NeoSignature";
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public FileStreamResult RawData(inSignaturesSearch _params)
		{
			string _data = "";
			string _mime = "";
			try
			{
				#region Parsing and validation area
				outBaseAnyResponse _validate = new outBaseAnyResponse();
				_validate = new daAuthenticate().VerifyTokenEthernal(
					new inToken
					{
						Id_application = _params.Id_application,
						Id_user = _params.Id_user,
						Token = _params.Token
					}
				);
				if (_validate.Logic)
				{
					_response = _da.Search(
						new inSearch
						{
							Id = _params.Id,
							Id_application = _params.Id_application,
							Id_user = _params.Id_user
						});
					if (_response.Records.Count() == 0) { throw new Exception(""); }
					_data = _response.Records[0]["raw_data"].ToString();
					_mime = _response.Records[0]["mime_type"].ToString();
					#region Automatic Log
					new daLogGeneral().WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", "");
					#endregion
				}
				else
				{
					throw new Exception("");
				}
			}
			catch (Exception)
			{
				string _html = _Helper.GetHtmlPageWrapper("An unexpected exception was encountered", "Invalid Token value", MethodBase.GetCurrentMethod().ReflectedType.FullName);
				_data = Convert.ToBase64String(Encoding.UTF8.GetBytes(_html));
				_mime = "text/html";
			}

			byte[] pdfBytes = Convert.FromBase64String(_data);
			MemoryStream ms = new MemoryStream(pdfBytes);
			return new FileStreamResult(ms, _mime);
		}
		public FileStreamResult RawDataAdditional(inSignaturesSearch _params)
		{
			string _data = "";
			string _mime = "";
			try
			{
				outBaseAnyResponse _validate = new outBaseAnyResponse();
				_validate = new daAuthenticate().VerifyTokenEthernal(
					new inToken
					{
						Id_application = _params.Id_application,
						Id_user = _params.Id_user,
						Token = _params.Token
					}
				);
				if (_validate.Logic)
				{
					_response = _da.Search(
						new inSearch
						{
							Id = _params.Id,
							Id_application = _params.Id_application,
							Id_user = _params.Id_user
						});
					if (_response.Records.Count() == 0) { throw new Exception(""); }

					#region Automatic Log
					new daLogGeneral().WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", "");
					#endregion
				}
				else
				{
					throw new Exception("");
				}
				#endregion
			}
			catch (Exception)
			{
				string _html = _Helper.GetHtmlPageWrapper("An unexpected exception was encountered", "Invalid Token value", MethodBase.GetCurrentMethod().ReflectedType.FullName);
				_data = Convert.ToBase64String(Encoding.UTF8.GetBytes(_html));
				_mime = "text/html";
			}
			byte[] pdfBytes = Convert.FromBase64String(_data);
			MemoryStream ms = new MemoryStream(pdfBytes);
			return new FileStreamResult(ms, _mime);
		}
		public FileStreamResult Certificate(inSignaturesSearch _params)
		{
			Dictionary<string, string> _lang = LoadStrings_es();
			string _data = "";
			string _mime = "";
			MemoryStream ms = new MemoryStream();
			try
			{
				outBaseAnyResponse _validate = new outBaseAnyResponse();
				_validate = new daAuthenticate().VerifyTokenEthernal(new inToken { Id_application = _params.Id_application, Id_user = _params.Id_user, Token = _params.Token });
				if (_validate.Logic)
				{
					_response = _da.Search(new inSearch { Id = _params.Id, Id_application = _params.Id_application, Id_user = _params.Id_user });
					/*Build PDF Certificate*/
					if (_response.Records.Count() == 0) { throw new Exception(""); }

					System.Drawing.Image imageQrCode = _da.CreateQrCode(new inSignatures { Id_application = _params.Id_application, Id_user = _params.Id_user });

					ms = new MemoryStream();
					imageQrCode.Save(ms, ImageFormat.Png);
					string imgB64QrCode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
					ms.Close();

					_data = "<div style='width:100%;font-family:Tahoma;'>";
					_data += "<table style='width:100%;'>";
					_data += "   <tr>";
					_data += "      <td valign='top'>";
					_data += "         <h1><span style='color:rgb(0,155,219);font-size:1.25em;'>N</span>eoSignature</h1>";
					_data += "         <h2>" + _lang["msg_10000"] + "</h2>";
					_data += "      </td>";
					_data += "      <td valign='top' align='right'>";
					_data += "         <img src='" + imgB64QrCode + "' style='width:200px;'/>";
					_data += "      </td>";
					_data += "   </tr>";
					_data += "</table>";
					_data += "<hr/>";

					_data += "<h3 style='padding:2px;margin:0px;background-color:rgb(0,155,219);color:white;width:100%;'>" + _lang["msg_10001"] + "</h3>";
					_data += "<p style='font-size:12px;padding-left:10px;text-align:justify;'>";
					_data += string.Format(_lang["msg_10002"], Convert.ToDateTime(_response.Records[0]["fum"]).ToString("dd/MM/yyyy"), _response.Records[0]["code"].ToString());
					_data += "</p>";
					string _ext = _response.Records[0]["mime_type"].ToString().Split(new char[] { '/' })[1];

					_data += "<div style='width:100%;padding:5px;font-size:10px;'>";
					_data += "   <ul style='padding-left:15px;margin:0px;'>";
					_data += "      <li>";
					_data += "         <b>" + _lang["msg_10003"] + " </b>" + _response.Records[0]["description"] + " - " + _response.Records[0]["description"] + " + " + _ext + "</td>";
					_data += "      </li>";
					_data += "      <li>";
					_data += "         <b>" + _lang["msg_10004"] + " </b>" + Convert.ToDateTime(_response.Records[0]["verified"]).ToString("dd/MM/yyyy HH:mm:ss") + "</td>";
					_data += "      </li>";
					_data += "      <li>";
					_data += "         <b>" + _lang["msg_10005"] + " </b>" + _response.Records[0]["code"].ToString() + " </td>";
					_data += "      </li>";
					_data += "   </ul>";
					if ((int)_response.Records[0]["integrity"] == 1)
					{
						_data += "<p style='color:green;font-size:14px;'>" + _lang["msg_10006"] + "</p>";
					}
					else
					{
						_data += "<p style='color:red;font-size:14px;'>" + _lang["msg_10007"] + "</p>";
					}
					_data += "</div>";

					_data += "<h3 style='padding:2px;margin:0px;background-color:rgb(0,155,219);color:white;width:100%;'>" + _lang["msg_10009"] + "</h3>";
					_data += "<table style='width:100%;margin:0px;padding:0px;font-size:14px;'>";
					_data += "   <tr style='background-color:silver;'>";
					_data += "      <td width='40%' style='padding:5px;'><b>" + _lang["msg_10010"] + "</b></td>";
					_data += "      <td width='30%' style='padding:5px;'><b>" + _lang["msg_10011"] + "</b></td>";
					_data += "      <td width='30%' style='padding:5px;'><b>" + _lang["msg_10012"] + "</b></td>";
					_data += "   </tr>";
					_data += "   <tr>";
					_data += "      <td width='40%' valign='top' style='padding:0px;font-size:16px;' align='center'>";
					string _raw_additional = _response.Records[0]["raw_data_additional"].ToString();
					if (_raw_additional==null || _raw_additional == "") { _raw_additional = ""; }
					if (_raw_additional != "")
					{
						if (_raw_additional.IndexOf(",") != -1) { _raw_additional = _raw_additional.Split(new char[] { '/' })[1]; }
						_raw_additional = "data:" + _Helper.detectMimeTypeFromBase64(_raw_additional) + ";base64," + _raw_additional;
						_data += "<img src='" + _raw_additional + "' style='width:100%;margin:0px;padding:0px;'/>";
					}
					_data += "<p>" + _response.Records[0]["name_key"].ToString() + " " + _response.Records[0]["type_key"].ToString() + " " + _response.Records[0]["val_key"].ToString() + " </p>";
					if ((int)_response.Records[0]["integrity_additional"] == 1)
					{
						_data += "<p style='color:green;font-size:14px;'>" + _lang["msg_10014"] + "</p>";
					}
					else
					{
						_data += "<p style='color:red;font-size:14px;'>" + _lang["msg_10007"] + "</p>";
					}
					_data += "      </td>";

					string _privateParty = _response.Records[0]["privateParty"].ToString();
					_privateParty = _privateParty.Replace("-----BEGIN CERTIFICATE-----", null).Replace("-----END CERTIFICATE-----", null);
					X509Certificate2 certPrivate = new X509Certificate2(Convert.FromBase64String(_privateParty));

					string _publicParty = _response.Records[0]["publicParty"].ToString();
					_publicParty = _publicParty.Replace("-----BEGIN CERTIFICATE-----", null).Replace("-----END CERTIFICATE-----", null);
					X509Certificate2 certPublic = new X509Certificate2(Convert.FromBase64String(_publicParty));

					_data += "      <td width='30%' valign='top' style='padding:5px;'>";
					_data += certPrivate.SubjectName.Name + "<br/>";
					_data += certPrivate.IssuerName.Name + "<br/>";
					_data += certPrivate.NotAfter + " - " + certPrivate.NotBefore + "<br/>";
					_data += certPrivate.Thumbprint + " " + certPrivate.SignatureAlgorithm.FriendlyName + "<br/>";
					_data += certPrivate.SerialNumber + "<br/>";
					_data += "      </td>";
					_data += "      <td width='30%' valign='top' style='padding:5px;'>";
					_data += certPublic.SubjectName.Name + "<br/>";
					_data += certPublic.IssuerName.Name + "<br/>";
					_data += certPublic.NotAfter + " - " + certPublic.NotBefore + "<br/>";
					_data += certPublic.Thumbprint + " " + certPublic.SignatureAlgorithm.FriendlyName + "<br/>";
					_data += certPublic.SerialNumber + "<br/>";
					_data += "      </td>";

					_data += "   </tr>";
					_data += "</table>";

					/*
                    interOpSigningByProfile _selfSign = new interOpSigningByProfile();
                    _selfSign.Id_application = _params.Id_application;
                    _selfSign.Id_user = _params.Id_user;
                    _selfSign.DataToSign = _data;
                    _selfSign.KeyDatabase = KeyDatabase;
                    _selfSign = _da.SignString(_selfSign);
                    */
					_data += "   <h3 style='padding:2px;margin:0px;margin-top:5px;background-color:#b3cccc;color:black;width:100%;'>" + _lang["msg_10016"] + "</h3>";
					_data += "   <ul>";
					_data += "      <li><b>" + _lang["msg_10017"] + " </b>" + DateTime.Now.ToString("dd/MM/yyyy") + "</li>";
					_data += "      <li><b>" + _lang["msg_10018"] + " </b>" + Convert.ToDateTime(_response.Records[0]["verified_integrity"]).ToString("dd/MM/yyyy") + "</li>";
					_data += "      <li><b>" + _lang["msg_2005"] + " </b>" + _response.Records[0]["lat"].ToString() + " </li>";
					_data += "      <li><b>" + _lang["msg_2006"] + " </b>" + _response.Records[0]["lng"].ToString() + " </li>";
					_data += "      <li><b>" + _lang["msg_2007"] + " </b>" + _response.Records[0]["altitude"].ToString() + " </li>";
					_data += "      <li><b>" + _lang["msg_2008"] + " </b>" + _response.Records[0]["speed"].ToString() + " </li>";
					_data += "      <li><b>" + _lang["msg_2009"] + " </b>" + _response.Records[0]["referer"].ToString() + " </li>";
					_data += "      <li><b>" + _lang["msg_2010"] + " </b>" + _response.Records[0]["custom_message"].ToString() + " </li>";
					//_data += "      <li><b>" + _lang["msg_10019"] + " </b>" + _selfSign.Signature+ "</li>";
					_data += "   </ul>";
					_data += "   <h3 style='padding:2px;margin-top:10px;background-color:silver;color:black;width:100%;'>" + _lang["msg_10020"] + "</h3>";
					_data += "   <table style='width:100%;font-size:9px;'>";
					_data += "      <tr>";
					_data += "         <td valign='top'><img src='" + imgB64QrCode + "' style='width:100px;'/></td>";
					_data += "         <td valign='top'>" + _lang["msg_10021"] + "</td>";
					_data += "      </tr>";
					_data += "   </table>";
					_data += "</div>";

					_data += "<div style='font-family:Tahoma;margin:10px;padding:10px;page-break-before: always;'>";
					_data += "   <h3 style='padding:2px;margin:0px;background-color:#b3cccc;color:black;width:100%;'>" + _lang["msg_10026"] + "</h3>";
					_data += "   <p style='font-size:12px;'><b>" + _lang["msg_10008"] + "</b></p>";
					_data += "   <div style='font-size:8px;word-break: break-all;'>" + _response.Records[0]["signature"].ToString() + "[parcial] </div>";
					_data += "   <p style='font-size:12px;'><b>" + _lang["msg_10023"] + "</b></p>";
					_data += "   <div style='font-size:8px;word-break: break-all;'>" + _response.Records[0]["publicKey"].ToString() + " </div>";

					_data += "   <p style='font-size:12px;'><b>" + _lang["msg_10022"] + "</b></p>";
					_data += "   <div style='font-size:8px;word-break: break-all;'>" + _response.Records[0]["signature_additional"].ToString() + " </div>";
					_data += "   <p style='font-size:12px;'><b>" + _lang["msg_10024"] + "</b></p>";
					_data += "   <div style='font-size:8px;word-break: break-all;'>" + _response.Records[0]["publicKey_additional"].ToString() + " </div>";
					_data += "</div>";

					HtmlToPdf converter = new HtmlToPdf();
					converter.Options.PdfPageSize = PdfPageSize.A4;
					converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
					PdfDocument doc = converter.ConvertHtmlString(_data);
					//doc.Save("nuevo.pdf");
					byte[] pdfBytes = doc.Save();
					doc.Close();
					ms = new MemoryStream(pdfBytes);
					_mime = "application/pdf";

					new daLogGeneral().WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", "");
				}
				else
				{
					throw new Exception("");
				}
			}
			catch (Exception)
			{
				string _html = _Helper.GetHtmlPageWrapper("An unexpected exception was encountered", "Invalid Token value", MethodBase.GetCurrentMethod().ReflectedType.FullName);
				_data = Convert.ToBase64String(Encoding.UTF8.GetBytes(_html));
				_mime = "text/html";
				byte[] pdfBytes = Convert.FromBase64String(_data);
				ms = new MemoryStream(pdfBytes);
			}
			return new FileStreamResult(ms, _mime);
		}
		private Dictionary<string, string> LoadStrings_es()
		{
			Dictionary<string, string> _lang = new Dictionary<string, string>();
			_lang.Add("msg_log_general", "Log general");
			_lang.Add("msg_nodata", "No se han obtenido datos");
			_lang.Add("msg_uninitialized", "El componente requerido, no ha sido definido");
			_lang.Add("msg_ok", "Operación ejecutada con normalidad");
			_lang.Add("msg_2003", "La plataforma está operativa y sus credenciales válidas.");
			_lang.Add("msg_2004", "Verificar con QR-Code");
			_lang.Add("msg_2005", "Latitud:");
			_lang.Add("msg_2006", "Longitud:");
			_lang.Add("msg_2007", "Altitud:");
			_lang.Add("msg_2008", "Velocidad");
			_lang.Add("msg_2009", "Referer:");
			_lang.Add("msg_2010", "Mensaje interno:");
			_lang.Add("msg_10000", "Información certificada<br/>Proceso de firma digital");
			_lang.Add("msg_10001", "Información General");
			_lang.Add("msg_10002", "NeoSignature plataforma de firma remota, actuando como Tercero de Confianza según lo dispuesto en la normativa sobre firma electrónica y comercio electrónico, CERTIFICA EN NOMBRE PROPIO que los datos recogidos a lo largo del presente documento corresponden al proceso de firma formalizado {0}, bajo el Código de Transacción {1}");
			_lang.Add("msg_10003", "Documento:");
			_lang.Add("msg_10004", "Fecha del proceso:");
			_lang.Add("msg_10005", "Código transacción:");
			_lang.Add("msg_10006", "El documento concuerda con la firma digital");
			_lang.Add("msg_10007", "La integridad de los datos está comprometida");
			_lang.Add("msg_10008", "Hash permanente:");
			_lang.Add("msg_10009", "Información de los intervinientes");
			_lang.Add("msg_10010", "Firmante");
			_lang.Add("msg_10011", "Privado");
			_lang.Add("msg_10012", "Público");
			_lang.Add("msg_10013", "Información relacionada al firmante externo:");
			_lang.Add("msg_10014", "El registro hológrafo concuerda con la firma digital");
			_lang.Add("msg_10015", "Hash del firmante:");
			_lang.Add("msg_10016", "Referencia del contenido");
			_lang.Add("msg_10017", "Certificado generado el:");
			_lang.Add("msg_10018", "Última verificación de firma:");
			_lang.Add("msg_10019", "Hash volátil de este informe (para verificación interna):");
			_lang.Add("msg_10020", "Detalles técnicos");
			_lang.Add("msg_10021",
			"<p>Modelo de encriptación asimétrica utilizando OpenSSL, implementando algoritmo SHA-512 con uso de clave pública criptográficamente segura generada con OpenSSL y clave privada relacionada al certificado provisto por <b>{0}</b></p>"
			+ "<p>Se proveen servicios para la verificación de cada una de las firmas y/o hashes indicados en este informe</p>"
			+ "<p>Se proveen servicios para la verificación de la integridad de los datos en la propia plataforma de firma</p>"
			+ "<p>Se provee trazabilidad completa de los eventos relacionados con las firmas y/o hashes indicados en este informe</p>"
			+ "<p>Se garantiza la guarda de los algoritmos de cálculo de claves, PEM y certificados involucrados en las firmas y/o hashes indicados en este informe, siguiedo las buenas prácticas y guías que indican normas internacionales</p>");
			_lang.Add("msg_10022", "Hash permanente adicional:");
			_lang.Add("msg_10023", "Clave pública");
			_lang.Add("msg_10024", "Clave pública adicional");
			_lang.Add("msg_10025", "Validación");
			_lang.Add("msg_10026", "Firmas digitales");
			return _lang;
		}
		public outBaseAnyResponse State(inBaseAnyRequest _params)
		{
			try
			{
				_response = _da.State(_params);
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
	}
}