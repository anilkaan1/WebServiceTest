using JwtTokenAuthentication.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtTokenAuthentication.Services
{
    public interface IJwtTokenGenerator
    {
        public string GenerateToken(DateTime notBefore, DateTime expire);

        public UserLoginResponseModel GenerateToken(string[] claimRoles, DateTime notBefore, DateTime expire, string UID);


    }
}
