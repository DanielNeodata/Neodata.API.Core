namespace neoTools
{
	public class neoCrypto
	{

		public bool IsMD5(string? input)
		{
			if (String.IsNullOrEmpty(input))
			{
				return false;
			}
			return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
		}

		public string MD5(string? input)
		{
			MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
			byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
			bs = x.ComputeHash(bs);
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach (byte b in bs) { s.Append(b.ToString("x2").ToLower()); }
			return s.ToString();
		}

		public string SHA256(byte[] input)
		{
			SHA256CryptoServiceProvider x = new SHA256CryptoServiceProvider();
			byte[] bs = x.ComputeHash(input);
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach (byte b in bs) { s.Append(b.ToString("x2").ToLower()); }
			return s.ToString();
		}

	}
}
