using Spire.Barcode;
using System.Drawing;
using System.Text.Json;

namespace neoTools
{
	public class neoHelper
	{
		public string detectMimeTypeFromBase64(string b64)
		{
			string _ret = "";
			Dictionary<string, string> signatures = new Dictionary<string, string>();
			signatures.Add("JVBERi0", "application/pdf");
			signatures.Add("R0lGODdh", "image/gif");
			signatures.Add("R0lGODlh", "image/gif");
			signatures.Add("iVBORw0KGgo", "image/png");
			signatures.Add("/9j/", "image/jpg");
			foreach (var item in signatures)
			{
				if (b64.IndexOf(item.Value) == 0)
				{
					_ret = signatures[item.Key];
					break;
				}
			}
			return _ret;
		}
		public DateTime? SecureDateTime(object? str)
		{
			if (string.IsNullOrEmpty(str.ToString()))
			{
				return null;
			}
			else
			{
				return Convert.ToDateTime(str);
			}
		}
		public string? GetHtmlPageWrapper(string title, string message, string? backtrace)
		{
			string _html = "";
			_html += "<html>";
			_html += "<head>";
			_html += "<meta charset='utf-8'>";
			_html += "<title>Neodata Ecosystem</title>";
			_html += "<meta http-equiv='Content-Language' content='EN'/>";
			_html += "<meta http-equiv='X-UA-Compatible' content='IE=edge'/>";
			_html += "<meta name='viewport' content='width=device-width, initial-scale=1'/>";
			_html += "<meta name='apple-mobile-web-app-capable' content='yes'/>";
			_html += "<meta name='mobile-web-app-capable' content='yes'/>";
			_html += "<meta name='apple-mobile-web-app-status-bar-style' content='black-translucent'/>";
			_html += "</head>";

			_html += "<body>";
			_html += "<div style='border:1px solid #990000;padding:10px;margin:10px;'>";
			if (title != "") { _html += "<h3>" + title + "</h3>"; }
			if (message != "")
			{
				_html += "   <p style='border:1px solid red;padding:20px;margin:20px:'>" + message + "</p>";
			}
			if (backtrace != "") { _html += "<p>Backtrace: <i>" + backtrace + "</i></p>"; }
			_html += "<p>Timestamp: <i>" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "</i></p>";

			_html += "</div>";
			_html += "</body>";
			_html += "</html>";
			return _html;
		}
		public Image GetImageQR(string url, string data2D, BarcodeSettings settings)
		{
			settings.Data = url;
			settings.Data2D = data2D;
			BarCodeGenerator generator = new BarCodeGenerator(settings);
			return generator.GenerateImage();
		}
		public Image GetImageBarCode(string text, string data2D, BarcodeSettings settings)
		{
			settings.Data = text;
			settings.Data2D = data2D;
			BarCodeGenerator generator = new BarCodeGenerator(settings);
			return generator.GenerateImage();
		}
		public bool IsJsonValid(string txt)
		{
			try { return JsonDocument.Parse(txt) != null; } catch { }

			return false;
		}
	}
}
