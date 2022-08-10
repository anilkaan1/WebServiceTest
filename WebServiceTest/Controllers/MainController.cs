using JwtTokenAuthentication.Responses;
using JwtTokenAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using TCPIP_ServerClient;
using WebServiceTest.Converters;
using WebServiceTest.Data;

namespace WebServiceTest.Controllers
{
    //[System.Web.Http.Route("[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/[Controller]")]
    public class MainController : ControllerBase
    { 
        private readonly ILogger<MainController> _logger;
        private MyJsonToXmlConverter _JSONconverter;
        private IJwtTokenGenerator _JWTGenerator;
        private MyXmlToJsonConverter _XMLconverter;
        private string _key = "";
        //private ISessionRepository _session;
        public MainController(ILogger<MainController> logger,IJwtTokenGenerator JWTGenerator, MyXmlToJsonConverter mxljson , MyJsonToXmlConverter jsonxml/*, ISessionRepository session*/)
        {
           _logger = logger;
           _JSONconverter = jsonxml;
           _XMLconverter = mxljson;
           _JWTGenerator = JWTGenerator;
           //_key = myDescriptor.Key
            //_session = session;

            //TCPIPServer yeni = new TCPIPServer();

            //yeni.setupserver();
            //yeni.StartListener();
        }
       
        [Authorize]
        [HttpGet]
        [Route("AdminTest")]
        [Consumes("text/plain")]
        public IActionResult denemeAdmin()
        {
            return Ok();
        }


        [Authorize(Roles ="Admin")]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Getxml")]
        [Consumes("application/json")]
        public IActionResult getxml([System.Web.Http.FromBody] JsonDocument myReq, [FromHeader] string Authorization/*way 2*/)
        {

            var token = Request.Headers["Authorization"]; //way 1 

            var handler = new JwtSecurityTokenHandler();

            Authorization = Authorization.Replace("Bearer ", "");

            var jsonToken = handler.ReadToken(Authorization);

            var tokenS = handler.ReadToken(Authorization) as JwtSecurityToken;

            string id = tokenS.Claims.First(claim => claim.Type == "SessionID").Value;

            _JSONconverter.JsonToXmlSet(myReq/*,session*/);

            return Ok(_JSONconverter.ExecuteFunction());
            #region cop
            //const string jsonddd = " [ { \"name\": \"John\" }, [ \"425-000-1212\", 15 ], { \"grades\": [ 90, 80, 100, 75 ] } ]";

            //JsonDocument doc = JsonDocument.Parse(jsonddd);


            //human assaa = new human();
            //assaa.name = "fgsgfsgfs";
            //assaa.description = "fdsgfgfdsg";

            //string deneme = JsonSerializer.Serialize(assaa);

            //JsonDocument newone = JsonSerializer.Deserialize<JsonDocument>(deneme);

            //human aaafd = JsonSerializer.Deserialize<human>(deneme);

            //JsonElement aaa = doc.RootElement;

            //JsonElement info = aaa[0];
            //JsonElement info1 = aaa[1];
            //JsonElement info2 = aaa[2];

            //string json = System.Text.Json.JsonSerializer.Serialize(qqqq.RootElement);


            //using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            //{
            //    string content = await reader.ReadToEndAsync();

            //    string ahg = content.ToString();
            //}
            //myCommonService.hello();
            //return Ok(new JsonResult(qqqq));

            #endregion cop
        }

        [Authorize]
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Getjson")]
        [Consumes("application/xml")]
        public IActionResult getjson([System.Web.Http.FromBody] XElement myReq)
        {
            _XMLconverter.XmlToJsonSet(myReq);
            return Ok(_XMLconverter.ExecuteFunction());
        }


    }
}