namespace neoTools
{
	public class neoToken
	{
		public string GenerateTokenJitsi(int id)
		{
			string key = "5dd2b2d8c528e0ab5a2083ffddd3e41e671a36be018f74dc2f7c32c962c73a41";
			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			JwtSecurityToken token = new JwtSecurityToken();
			JwtPayload payload = new JwtPayload();
			Claim[] someClaims = new Claim[] { new Claim("aud", "NeoVideo"), new Claim("iss", "NeoVideo"), new Claim("sub", "meet.jitsi"), new Claim("room", "NEOVIDEO" + id.ToString()) };
			token = new JwtSecurityToken(claims: someClaims, signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));
			string _token = new JwtSecurityTokenHandler().WriteToken(token);
			return _token;
		}

		public string GenerateToken(int id, string? username, int id_application, int seconds, string? issuer, string? audience)
		{
			Claim[] someClaims = new Claim[]{
				new Claim(JwtRegisteredClaimNames.UniqueName,username),
				new Claim(JwtRegisteredClaimNames.Nonce,id_application.ToString()),
				new Claim(JwtRegisteredClaimNames.NameId,id.ToString())
			};

			SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("com.gruponeodata.neoauthentication"));
			JwtSecurityToken token = new JwtSecurityToken();
			if (seconds > 0)
			{
				token = new JwtSecurityToken(
					issuer: issuer,
					audience: audience,
					claims: someClaims,
					expires: DateTime.Now.AddSeconds(seconds),
					signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
				);
			}
			else
			{
				token = new JwtSecurityToken(
					issuer: issuer,
					audience: audience,
					claims: someClaims,
					signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
				);
			}

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public bool VerifyToken(string? token, int _id_application, int _id_user)
		{
			bool _ret = true;
			bool evaluateExp = true;
			var obj = new JwtSecurityToken(jwtEncodedString: token);
			string x = "0";
			try
			{
				x = obj.Claims.First(c => c.Type == "exp").Value.ToString();
			}
			catch (Exception)
			{
				evaluateExp = false;
			};
			long exp = 0;
			long.TryParse(x, out exp);

			string? username = obj.Claims.First(c => c.Type == "unique_name").Value;
			int id_user = Convert.ToInt32(obj.Claims.First(c => c.Type == "nameid").Value);
			int id_application = Convert.ToInt32(obj.Claims.First(c => c.Type == "nonce").Value);
			var tokenDate = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
			DateTime now = DateTime.Now.ToUniversalTime();
			if (evaluateExp)
			{
				_ret = ((tokenDate >= now) && (id_application == _id_application) && (id_user == _id_user));
			}
			else
			{
				_ret = ((id_application == _id_application) && (id_user == _id_user));
			}
			return _ret;
		}
	}
}
