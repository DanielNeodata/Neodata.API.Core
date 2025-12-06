using Spire.Barcode;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace neodataEcosystem.Rules
{
	public class ruTools : rulesBaseSuperClass
	{
		neoHelper _Helper = new neoHelper();
		public ruTools() : base("neo_tools") { }

		public int CountPages(inPages _params)
		{
            int _pages = 0;
            try
            {
                _params.Base64 = _params.Base64.Replace("\\", string.Empty);
                byte[] bytes = Convert.FromBase64String(_params.Base64);
                MemoryStream ms = new MemoryStream(bytes);
                switch (_params.Filename.Split(".").Last())
                {
                    case "tiff":
                    case "tif":
                        Image img = Image.FromStream(ms);
                        Guid ID = img.FrameDimensionsList[0];
                        FrameDimension fd = new FrameDimension(ID);
                        _pages = img.GetFrameCount(fd);
                        img.Dispose();
                        break;
                    case "pdf":
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            Regex regex = new Regex(@"/Type\s*/Page[^s]");
                            MatchCollection matches = regex.Matches(sr.ReadToEnd());
                            _pages = matches.Count;
                        }
                        break;
                }
                return _pages;
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _pages;
		}
        public outBaseAnyResponse CreateQR(inQR _params)
        {
            try
            {
                BarcodeSettings settings = new BarcodeSettings();
                settings.Type = BarCodeType.QRCode;
                settings.QRCodeDataMode = QRCodeDataMode.AlphaNumber;
                settings.HasBorder = false;
                settings.AutoResize = true;
                settings.ShowText = false;
                settings.TopMargin = 5;
                settings.BottomMargin = 5;
                settings.LeftMargin = 5;
                settings.RightMargin = 5;
                settings.ImageWidth = 200;
                settings.ImageHeight = 200;
                settings.X = 1.0f;
                settings.QRCodeECL = QRCodeECL.H;
                Image _image = _Helper.GetImageQR(_params.Url, " ", settings);

                ImageConverter imgCon = new ImageConverter();
                _response.Base64 = Convert.ToBase64String((byte[])imgCon.ConvertTo(_image, typeof(byte[])));
                _response.Mime = "image/png";
                _response.Url = ("QR?b64=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(_params.Url)));

                return _response;
            }
            catch (Exception ex)
            {
                SetError(ex);
            }
            return _response;
        }
		public outBaseAnyResponse CreateBarCode(inBarCode _params)
		{
			try
			{
				BarcodeSettings settings = new BarcodeSettings();
				settings.Type = BarCodeType.Code128;
                settings.BottomText = _params.Text;
                settings.ShowBottomText = true;
                settings.ShowText = false;
				Image _image = _Helper.GetImageBarCode(_params.Text, _params.Text, settings);

				ImageConverter imgCon = new ImageConverter();
				_response.Base64 = Convert.ToBase64String((byte[])imgCon.ConvertTo(_image, typeof(byte[])));
				_response.Mime = "image/png";
				_response.Url = ("BarCode?b64=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(_params.Text)));

				return _response;
			}
			catch (Exception ex)
			{
				SetError(ex);
			}
			return _response;
		}
		public FileStreamResult QR(string _params)
		{
			_response = CreateQR(new inQR { Url = Encoding.UTF8.GetString(Convert.FromBase64String(_params)) });
			byte[] pdfBytes = Convert.FromBase64String(_response.Base64);
			MemoryStream ms = new MemoryStream(pdfBytes);
			return new FileStreamResult(ms, "image/png");
		}
		public FileStreamResult BarCode(string _params)
		{
			_response = CreateBarCode(new inBarCode { Text = Encoding.UTF8.GetString(Convert.FromBase64String(_params)) });
			byte[] pdfBytes = Convert.FromBase64String(_response.Base64);
			MemoryStream ms = new MemoryStream(pdfBytes);
			return new FileStreamResult(ms, "image/png");
		}
	}
}