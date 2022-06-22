using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Web.Http;
using System.Xml.Linq;
using WebServiceTest.Converters;
using WebServiceTest.Data;
namespace WebServiceTest.Controllers
{



    //[System.Web.Http.Route("[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api")]
    public class MainController : ControllerBase
    { 
        private readonly ILogger<MainController> _logger;
        private MyConverter _converter;

        public MainController(ILogger<MainController> logger)
        {
           _logger = logger;
        }


        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("requests")]
        [Consumes("text/plain")]
        public async Task<IActionResult> Get()
        {
            return Ok("Hello");
        }


        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Getxml")]
        [Consumes("application/json")]
        public IActionResult getxml([System.Web.Http.FromBody] JsonDocument myReq)
        {
            _converter = new JsonToXmlService(myReq);
            return Ok(_converter.ExecuteFunction());
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


        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("Getjson")]
        [Consumes("application/xml")]
        public IActionResult getjson([System.Web.Http.FromBody] XElement myReq)
        {
            _converter = new XmlToJsonService(myReq);
            return Ok(_converter.ExecuteFunction());
        }


    }
}