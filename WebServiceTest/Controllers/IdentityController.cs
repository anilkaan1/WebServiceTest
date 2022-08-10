using JwtTokenAuthentication;
using JwtTokenAuthentication.Requests;
using JwtTokenAuthentication.Responses;
using JwtTokenAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TCPIP_ServerClient;
using WebServiceTest.Data;

namespace WebServiceTest.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[Controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private IAuthenticationService _authenticationService;
        public IdentityController(ILogger<MainController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            //_key = myDescriptor.Key
            //_session = session;

            //TCPIPServer yeni = new TCPIPServer();
            //yeni.setupserver();
            //yeni.StartListener();
        }
        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Authorize")]
        [Consumes("application/json")]
        public IActionResult Login([System.Web.Http.FromBody] UserLoginRequestModel reqLogin)
        {
            return _authenticationService.Authenticate(reqLogin);

            //UserLoginRequestModel reqLogin = new UserLoginRequestModel("","");

            //if (myAuthenticationJson.RootElement.GetProperty("UID").ToString() != "" && myAuthenticationJson.RootElement.GetProperty("PASSWORD").ToString() != "")
            //{
            //    reqLogin = myAuthenticationJson.RootElement.Deserialize<UserLoginRequestModel>();
            //}
            //else
            //{
            //    return NotFound("ID/PASSWORD IS EMPTY");
            //}
        }

        [AllowAnonymous]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Refresh")]
        [Consumes("application/json")]
        public IActionResult Refresh([System.Web.Http.FromBody] RefreshTokenRequestModel reqRefresh)
        {
            return _authenticationService.RefreshToken(reqRefresh);

            //UserLoginRequestModel reqLogin = new UserLoginRequestModel("","");

            //if (myAuthenticationJson.RootElement.GetProperty("UID").ToString() != "" && myAuthenticationJson.RootElement.GetProperty("PASSWORD").ToString() != "")
            //{
            //    reqLogin = myAuthenticationJson.RootElement.Deserialize<UserLoginRequestModel>();
            //}
            //else
            //{
            //    return NotFound("ID/PASSWORD IS EMPTY");
            //}
        }
    }
}
