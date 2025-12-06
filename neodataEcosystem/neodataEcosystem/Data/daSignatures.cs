using Spire.Barcode;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace neodataEcosystem.Data
{
	public class daSignatures : dataBaseSuperClass
	{
		neoHelper _Helper = new neoHelper();

		public daSignatures() : base("neo_signature") { }

		public outBaseAnyResponse Create(inSignatures _params)
		{
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				outBaseAnyResponse _response = new outBaseAnyResponse();

				if (_params.ExternalId == null) { _params.ExternalId = ""; }
				if (_params.Type_key == null) { _params.Type_key = ""; }
				if (_params.Val_key == null) { _params.Val_key = ""; }
				if (_params.Name_key == null) { _params.Name_key = ""; }
				if (_params.Raw_data == null) { _params.Raw_data = ""; }
				if (_params.Mime_type == null) { _params.Mime_type = ""; }
				if (_params.Raw_data_additional == null) { _params.Raw_data_additional = ""; }
				if (_params.Mime_type_additional == null) { _params.Mime_type_additional = ""; }
				if (_params.ExternalId == null) { _params.ExternalId = ""; }
				if (_params.Modifier == null) { _params.Modifier = ""; }
				if (_params.Referer == null) { _params.Referer = ""; }
				if (_params.Custom_message == null) { _params.Custom_message = ""; }
				if (_params.Id_type_document == null) { _params.Id_type_document = 1; }
				if (_params.Id_type_status == null) { _params.Id_type_status = 1; }

				connection.Open();

				#region Signing based in ID_application and Id_user, for search Profile
				interOpSigningByProfile _objSign = new interOpSigningByProfile();
				_objSign.Id_application = _params.Id_application;
				_objSign.Id_user = _params.Id_user;
				_objSign.DataToSign = _params.Raw_data;
				_objSign.KeyDatabase = _DataContext.DefaultDatabase;
				_objSign = SignString(_objSign,false);

				interOpSigningByProfile _objSignAdditional = new interOpSigningByProfile();
				_objSignAdditional.Id_application = _params.Id_application;
				_objSignAdditional.Id_user = _params.Id_user;
				_objSignAdditional.DataToSign = _params.Raw_data_additional;
				_objSignAdditional.KeyDatabase = _DataContext.DefaultDatabase;
				_objSignAdditional = SignString(_objSignAdditional, false);
				#endregion

				#region Modify PDF if _params.Modifier is ok
				_params.Raw_data = ModifyPDF(_params);
				#endregion

				#region Insert transfer!
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "INSERT INTO dbo.mod_transactions_transfers ";
				cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_application,id_user,id_type_document,id_type_status,";
				cmd.CommandText += " type_key,val_key,name_key,privateParty,publicParty,raw_data,mime_type,signature,privateKey,publicKey,integrity,verified_integrity, ";
				cmd.CommandText += " raw_data_additional,mime_type_additional,signature_additional,privateKey_additional,publicKey_additional,integrity_additional,verified_integrity_additional, ";
				cmd.CommandText += " privatized,destroyed,modifier,lat,lng,altitude,speed,referer,custom_message,externalid,id_profile) ";
				cmd.CommandText += " VALUES ";
				cmd.CommandText += " (" + _DataContext.getCommonFieldsValues("", "Neodata Digital Sign") + ",@ID_APPLICATION,@ID_USER,@ID_TYPE_DOCUMENT,@ID_TYPE_STATUS, ";
				cmd.CommandText += " @TYPE_KEY,@VAL_KEY,@NAME_KEY,@PRIVATEPARTY,@PUBLICPARTY,@RAW_DATA,@MIME_TYPE,@SIGNATURE,@PRIVATEKEY,@PUBLICKEY,1,getdate(), ";
				cmd.CommandText += " @RAW_DATA_ADDITIONAL,@MIME_TYPE_ADDITIONAL,@SIGNATURE_ADDITIONAL,@PRIVATEKEY_ADDITIONAL,@PUBLICKEY_ADDITIONAL,1,GETDATE(), ";
				cmd.CommandText += " null,null,@MODIFIER,@LAT,@LNG,@ALTITUDE,@SPEED,@REFERER,@CUSTOM_MESSAGE,@EXTERNALID,@ID_PROFILE) ";
				cmd.CommandText += " ; SELECT SCOPE_IDENTITY()";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_TYPE_DOCUMENT", _params.Id_type_document);
				cmd.Parameters.AddWithValue("@ID_TYPE_STATUS", _params.Id_type_status);
				cmd.Parameters.AddWithValue("@RAW_DATA", _params.Raw_data);
				cmd.Parameters.AddWithValue("@MIME_TYPE", _params.Mime_type);
				cmd.Parameters.AddWithValue("@EXTERNALID", _params.ExternalId);
				cmd.Parameters.AddWithValue("@TYPE_KEY", _params.Type_key);
				cmd.Parameters.AddWithValue("@VAL_KEY", _params.Val_key);
				cmd.Parameters.AddWithValue("@NAME_KEY", _params.Name_key);
				cmd.Parameters.AddWithValue("@PRIVATEPARTY", _objSign.PrivateParty);
				cmd.Parameters.AddWithValue("@PUBLICPARTY", _objSign.PublicParty);
				cmd.Parameters.AddWithValue("@SIGNATURE", _objSign.Signature);
				cmd.Parameters.AddWithValue("@PRIVATEKEY", _objSign.PrivateKey);
				cmd.Parameters.AddWithValue("@PUBLICKEY", _objSign.PublicKey);
				cmd.Parameters.AddWithValue("@ID_PROFILE", _objSign.Id_Profile);
				cmd.Parameters.AddWithValue("@RAW_DATA_ADDITIONAL", _params.Raw_data_additional);
				cmd.Parameters.AddWithValue("@MIME_TYPE_ADDITIONAL", _params.Mime_type_additional);
				cmd.Parameters.AddWithValue("@SIGNATURE_ADDITIONAL", _objSignAdditional.Signature);
				cmd.Parameters.AddWithValue("@PRIVATEKEY_ADDITIONAL", _objSignAdditional.PrivateKey);
				cmd.Parameters.AddWithValue("@PUBLICKEY_ADDITIONAL", _objSignAdditional.PublicKey);
				cmd.Parameters.AddWithValue("@MODIFIER", _params.Modifier);
				cmd.Parameters.AddWithValue("@LAT", _params.Lat);
				cmd.Parameters.AddWithValue("@LNG", _params.Lng);
				cmd.Parameters.AddWithValue("@ALTITUDE", _params.Altitude);
				cmd.Parameters.AddWithValue("@SPEED", _params.Speed);
				cmd.Parameters.AddWithValue("@REFERER", _params.Referer);
				cmd.Parameters.AddWithValue("@CUSTOM_MESSAGE", _params.Custom_message);

				_response.Numeric = Convert.ToInt32(cmd.ExecuteScalar());
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
				#endregion

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _objSign.Id_Profile.ToString(), "id", cmd.CommandText);
				#endregion

				#region Insert intakes
				cmd.CommandText = "INSERT INTO dbo.mod_transactions_intakes ";
				cmd.CommandText += " (" + _DataContext.getCommonFields() + ",id_application,id_user,id_transfer) ";
				cmd.CommandText += " VALUES ";
				cmd.CommandText += " (" + _DataContext.getCommonFieldsValues(Guid.NewGuid().ToString(), "") + ",@ID_APPLICATION,@ID_USER,@ID_TRANSFER)";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_TRANSFER", _response.Numeric);
				cmd.ExecuteScalar();
				#endregion

				#region Automatic Log
				WriteLog(_params.Id_application, _params.Id_user, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), "0", "id", cmd.CommandText);
				#endregion

				#region Set affected record in Response 
				_response = Search(
					new inSearch
					{
						Id = _response.Numeric,
						Id_application = _params.Id_application,
						Id_user = _params.Id_user
					});
				#endregion

				connection.Close();

				return _response;
			}
		}
		public outBaseAnyResponse Search(inSearch _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
			if (_params.Page == 0 || _params.Page == null) { _params.Page = 1; }
			if (_params.PageSize == 0 || _params.PageSize == null) { _params.PageSize = 25; }
			_response.Page = (int)_params.Page;
			_response.PageSize = (int)_params.PageSize;

			int _from = ((int)((_params.Page - 1) * _params.PageSize));
			int _to = ((int)(_from + _params.PageSize));

			string _where = " WHERE id_application=@ID_APPLICATION AND id_user=@ID_USER";
			if (_params.Id != 0 && _params.Id.HasValue)
			{
				_where += " AND id=@ID ";
			}
			else
			{
				if (!String.IsNullOrEmpty(_params.Search))
				{
					if (Regex.IsMatch(_params.Search, @"^\d+$"))
					{
						_where += " AND id=@SEARCH ";
					}
					else
					{
						_where += " AND (description LIKE '%@SEARCH%') ";
					}
				}
			}
			if (_params.Date_from.HasValue) { _where += " AND (created >=@DATE_FROM) "; }
			if (_params.Date_to.HasValue) { _where += " AND (created <=@DATE_TO) "; }

			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				DataTable dtResponse = new DataTable();

				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;

				cmd.CommandText = "SELECT COUNT(*) AS total FROM dbo.mod_transactions_vw_transfers ";
				if (_where != "") { cmd.CommandText += _where; }
				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);

				if (!String.IsNullOrEmpty(_params.Search)) { cmd.Parameters.AddWithValue("@SEARCH", _params.Search); }
				if (_params.Id != 0 && _params.Id.HasValue) { cmd.Parameters.AddWithValue("@ID", _params.Id); }
				if (_params.Date_from.HasValue)
				{
					DateTime _df = Convert.ToDateTime(_params.Date_from.ToString());
					cmd.Parameters.AddWithValue("@DATE_FROM", _df.ToString("yyyy-MM-dd"));
				}
				if (_params.Date_to.HasValue)
				{
					DateTime _dt = Convert.ToDateTime(_params.Date_to.ToString());
					cmd.Parameters.AddWithValue("@DATE_TO", _dt.ToString("yyyy-MM-dd"));
				}
				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.TotalPages = (int)dtResponse.Rows[0]["total"] / _response.PageSize + 1;

				cmd.CommandText = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY id DESC) AS CI_rownum, * ";
				cmd.CommandText += " FROM dbo.mod_transactions_vw_transfers ";
				if (_where != "") { cmd.CommandText += _where; }
				cmd.CommandText += ") AS CI_subquery WHERE ";
				cmd.CommandText += " CI_rownum BETWEEN " + _from.ToString() + " AND " + _to.ToString();

				dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Records = DataTableToList(dtResponse);

				connection.Close();
				return _response;
			}
		}
		public Image CreateQrCode(inSignatures _params)
		{
			if (_params.Type_key == null) { _params.Type_key = ""; }
			if (_params.Val_key == null) { _params.Val_key = ""; }
			if (_params.Name_key == null) { _params.Name_key = ""; }

			BarcodeSettings settings = new BarcodeSettings();
			settings.Type = BarCodeType.QRCode;
			settings.HasBorder = true;
			settings.AutoResize = true;
			settings.BorderWidth = 0.5f;
			settings.BorderDashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
			settings.BorderColor = Color.DarkGreen;
			settings.TopMargin = 5;
			settings.BottomMargin = 5;
			settings.LeftMargin = 5;
			settings.RightMargin = 5;
			settings.TextMargin = 5;
			settings.ShowTextOnBottom = true;
			settings.ImageWidth = 150;
			settings.ImageHeight = 150;
			settings.BottomTextAligment = StringAlignment.Center;
			settings.QRCodeDataMode = QRCodeDataMode.AlphaNumber;
			settings.X = 1.0f;
			settings.QRCodeECL = QRCodeECL.H;
			string url = _configServers.Servers["NeodataEcosystem"] + "/Certificate?";
			url += "id_application=" + _params.Id_application.ToString();
			url += "&id_user=" + _params.Id_user.ToString();
			url += "&token=" + _Token.GenerateToken(_params.Id_user, "", _params.Id_application, -1, "neodataEcosystem.gruponeodata.com", "neodataEcosystem.gruponeodata.com");
			string data2D = "Verificar QR";
			if (_params.Name_key != "") { data2D += _params.Name_key + Environment.NewLine; }
			if (_params.Type_key != "") { data2D += _params.Type_key + " "; }
			if (_params.Val_key != "") { data2D += _params.Val_key + " "; }
			if (data2D != " ") { data2D = data2D.TrimEnd().TrimStart(); }
			return _Helper.GetImageQR(url, data2D, settings);
		}

		public interOpSigningByProfile SignString(interOpSigningByProfile _params, bool _nosign)
		{
			string _line = "0";
			try
			{
				neoDataContext _DataContext = new neoDataContext(_params.KeyDatabase);
				DataTable dtResponse = GetProfile(_params.Id_application, _params.Id_user, _params.KeyDatabase);
				if (dtResponse.Rows.Count == 0) { throw new Exception("No rows", new Exception("901.01")); }
				_params.Id_Profile = Convert.ToInt32(dtResponse.Rows[0]["id"].ToString());
                if (_params.DataToSign == "") { _params.DataToSign = "no-data"; }
                _params.PublicParty = "no-data";
                _params.PrivateParty = "no-data";
                _params.PublicKey = "no-data";
                _params.PrivateKey = "no-data";
				_params.Signature = "no-data";
                if (!_nosign)
				{
					X509Certificate2 publicCertificate = new X509Certificate2(System.IO.File.ReadAllBytes("./Application/config/certificates/NeodataBase64.p12"), "55AraucariA", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
					X509Certificate2 privateCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(dtResponse.Rows[0]["p12"].ToString()), dtResponse.Rows[0]["certificate_password"].ToString(), X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
					_params.PublicParty = publicCertificate.ExportCertificatePem();
					_params.PrivateParty = privateCertificate.ExportCertificatePem();
					_params.PublicKey = publicCertificate.PublicKey.GetRSAPublicKey().ExportSubjectPublicKeyInfoPem();
					_params.PrivateKey = privateCertificate.PublicKey.GetRSAPublicKey().ExportSubjectPublicKeyInfoPem();
					if (_params.DataToSign == "") { _params.DataToSign = "no-data"; }

					ContentInfo contentInfo = new ContentInfo(new System.Text.UTF8Encoding().GetBytes(_params.DataToSign));
					SignedCms cms = new SignedCms(contentInfo, true);
					CmsSigner signer = new CmsSigner(privateCertificate);
					cms.ComputeSignature(signer, true);
					byte[] encodedSignature = cms.Encode();
					_params.Signature = Convert.ToBase64String(encodedSignature);
				}
				return _params;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Error en GetApplicationProfile: {0}",
					ex.InnerException.ToString() + " " + ex.Message + " - Línea SignString " + _line.ToString()), ex.InnerException
					);
			}
		}
		private string ModifyPDF(inSignatures _params)
		{
			string _newBase64 = _params.Raw_data;
			switch (_params.Modifier)
			{
				case "sign_with_raw_additional":
					if (_params.Mime_type.Split("/")[1].ToLower() != "pdf") { throw new Exception("No PDF Mime_type", new Exception("901.05")); }
					string _ext = _params.Mime_type_additional.Split("/")[1].ToLower();
					if (_ext != "bmp" && _ext != "jpg" && _ext != "gif" && _ext != "jpeg" && _ext != "png")
					{
						throw new Exception("No BMP, GIF, JPEG, JPG or PNG Mime_type_additional", new Exception("901.06"));
					}
					/*Open PDF from base64 stream*/
					PdfDocument pdf = new PdfDocument();
					if (_params.Raw_data.Contains("base64")) { _params.Raw_data = _params.Raw_data.Split(",")[1]; }

					MemoryStream stream_pdf = new MemoryStream(Convert.FromBase64String(_params.Raw_data));
					pdf.LoadFromStream(stream_pdf);

					/*Open IMAGE to insert from base64 stream*/
					if (_params.Raw_data_additional.Contains("base64")) { _params.Raw_data_additional = _params.Raw_data_additional.Split(",")[1]; }

					MemoryStream stream_image = new MemoryStream(Convert.FromBase64String(_params.Raw_data_additional));
					PdfImage image = PdfImage.FromStream(stream_image);

					/*Prepare data for image insertion*/
					/*Select page to alter*/
					PdfPageBase page = pdf.Pages[_params.PageToAlter - 1];
					try
					{
						page.Canvas.DrawImage(image, _params.X, _params.Y, image.Width, image.Height);
					}
					catch (Exception ex) { }

					/*QR generation and printing*/
					PdfImage imageQR = PdfImage.FromImage(CreateQrCode(_params));
					try
					{
						page.Canvas.DrawImage(imageQR, _params.X + image.Width, _params.Y, imageQR.Width / 2, imageQR.Height / 2);
					}
					catch (Exception ex) { }

					/*Open stream for save altered pdf*/
					MemoryStream filestream = new MemoryStream();
					pdf.SaveToStream(filestream);

					/*Transform to byte array for build base64 string*/
					_newBase64 = Convert.ToBase64String(filestream.ToArray());

					/*close streams and pdf object*/
					pdf.Close();
					filestream.Close();
					break;
			}
			return _newBase64;
		}
		public outBaseAnyResponse State(inBaseAnyRequest _params)
		{
			outAuthentication _response = new outAuthentication(new outBaseAnyResponse());
			using (SqlConnection connection = new SqlConnection(_DataContext.getDefaultDatabaseConnectionString()))
			{
				connection.Open();
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = "SELECT * FROM dbo.mod_backend_profiles WHERE id_application_auth=@ID_APPLICATION AND id_user_auth=@ID_USER";

				cmd.Parameters.Clear();
				cmd.Parameters.AddWithValue("@ID_USER", _params.Id_user);
				cmd.Parameters.AddWithValue("@ID_APPLICATION", _params.Id_application);

				DataTable dtResponse = new DataTable();
				dtResponse.Load(cmd.ExecuteReader());
				_response.Records = DataTableToList(dtResponse);
				_response.Logic = (dtResponse.Rows.Count != 0);
				if (!_response.Logic) { throw new Exception("No rows", new Exception("901.01")); }
				_response.Numeric = Convert.ToInt32(_response.Records[0]["id"]);
				if (_response.Numeric == 0) { throw new Exception("No id", new Exception("901.02")); }
				_response.Message = "La plataforma está operativa y sus credenciales válidas.";
				_response.Trace = "NeoSignature";
				_response.Scope = "V1";
				connection.Close();

				#region Automatic Log
				WriteLog(_params.Id_application, _response.Numeric, MethodBase.GetCurrentMethod().Name, JsonConvert.SerializeObject(_params).ToString(), _response.Numeric.ToString(), "id", cmd.CommandText);
				#endregion
			}
			return _response;
		}
	}
}
