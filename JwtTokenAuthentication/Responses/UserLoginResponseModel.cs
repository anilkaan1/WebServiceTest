using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenAuthentication.Responses
{
    public class UserLoginResponseModel
    {
        public string Token { get; set; }
        public string RefleshToken { get; set; }


        public UserLoginResponseModel(string token, string refleshToken)
        {
            Token = token;
            RefleshToken = refleshToken;
        }
    }
}
