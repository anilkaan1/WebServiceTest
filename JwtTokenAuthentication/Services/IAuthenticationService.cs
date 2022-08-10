using JwtTokenAuthentication.Requests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebServiceTest.Data;

namespace JwtTokenAuthentication.Services
{
    
    public interface IAuthenticationService
    {
        public ObjectResult Authenticate(UserLoginRequestModel reqLogin);
        public ObjectResult RefreshToken(RefreshTokenRequestModel reqRefresh);
    }
}
