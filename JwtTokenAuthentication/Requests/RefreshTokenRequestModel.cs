using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenAuthentication.Requests
{
    public class RefreshTokenRequestModel
    {
        public string Token { get; set; }
        public string RefleshToken { get; set; }


        public RefreshTokenRequestModel(string token, string refleshToken)
        {
            Token = token;
            RefleshToken = refleshToken;
        }

    }
}
