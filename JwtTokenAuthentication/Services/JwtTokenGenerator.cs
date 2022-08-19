using JwtTokenAuthentication.Responses;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtTokenAuthentication.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        readonly JwtDescriptor _jwtKey;
        public JwtTokenGenerator(IOptions<JwtDescriptor> myDescriptor)
        {
            _jwtKey = myDescriptor.Value;
        }
        public string GenerateToken(DateTime notBefore, DateTime expire)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey.Key));

            SigningCredentials credentialsPrev = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(_jwtKey.RsaPrivateKey.ToByteArray(), out _);

            SigningCredentials credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            #region start
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            #endregion 



            JwtSecurityToken Token = new JwtSecurityToken(
                claims: null,
                issuer: "http://localhost",
                audience: "http://localhost",
                notBefore: notBefore,
                expires: expire,
                signingCredentials: credentials);

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(Token);
        }
        
        public UserLoginResponseModel GenerateToken(string[] claimRoles, DateTime notBefore, DateTime expire)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AnilKaanBaskaya1."));

            SigningCredentials credentialsPrev = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(_jwtKey.RsaPrivateKey.ToByteArray(), out _);

            SigningCredentials credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            #region start
            List<Claim> claims = new List<Claim>();

            //ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            //{
            //    new Claim("UserName", "joey"),
            //    new Claim("Email","xxx@test.com")
            //});

            //foreach (string role in claimRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));

            //}
            #endregion


            claims.Add(new Claim("USERID", claimRoles[0]));

            claims.Add(new Claim("SESSIONID", claimRoles[1]));
            
            claims.Add(new Claim("GROUPID", claimRoles[2]));


            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, "432432432"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Email, "432432432"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Name, "432432432"));



            JwtSecurityToken Token = new JwtSecurityToken(
                claims: claims,
                issuer: "http://localhost",
                audience: "http://localhost",
                notBefore: notBefore,
                expires: expire,
                signingCredentials: credentials
                );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            string token = handler.WriteToken(Token);

            string guid = Guid.NewGuid().ToString();

            RefleshToken yeni = new RefleshToken {
                Token = guid,
            };
         
            string reftoken = yeni.Token;

            return new UserLoginResponseModel(token,reftoken);

        }


    }
}