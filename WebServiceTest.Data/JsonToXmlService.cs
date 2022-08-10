using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using TCPIP_ServerClient;
using WebServiceTest.Converters;

namespace WebServiceTest.Data
{
    public class JsonToXmlService : MyJsonToXmlConverter
    {
        private string hatamesaj { get; set; }
        private int CommandCode { get; set; }
        XDocument ConvertedXml { get; set; }
        string ConvertedXmlString {  get; set; }

        //SessionRepository _session { get; set; }

        public void JsonToXmlSet(JsonDocument jsonDocument/*,ISessionRepository session*/)
        {
            //_session = (SessionRepository)session;

            //var id = _session._httpContextAccessor.HttpContext.Session.Id;

            //var a = _session._httpContextAccessor.HttpContext.Session.Keys;

            //var b = _session._httpContextAccessor.HttpContext.Session;

            //_session._httpContextAccessor.HttpContext.Session.Set("Key1",Encoding.ASCII.GetBytes("Value1"));

            //a = _session._httpContextAccessor.HttpContext.Session.Keys;

            //dc127874-b9b4-50df-447d-abc5d2af6648

            hatamesaj = "";

            try
            {
                //CommandCode = JsonXmlHelper.FindCommandCodeFromJson(jsonDocument);

                //if (CommandCode == 0)
                //{
                //    new ArgumentException("The Command Could Not Found");
                //}

                ConvertedXml = JsonXmlHelper.ConvertJsonToXml(jsonDocument);

                ConvertedXmlString = JsonXmlHelper.XDocumentToString(ConvertedXml);


            }
            catch
            {
                throw new ArgumentException("Error In Json Content");
            }

        }

        public string ExecuteFunction()
        {
            if(hatamesaj != "")
            {
                return hatamesaj;
            }
            try
            {
                //Real Functioning will be here.

                //TCPIPClientMain main = new TCPIPClientMain();

                //main.ClientMessage = ConvertedXmlString;

                //main.ClientCommand = 'J';

                //main.StartCommunicationThread();

                //ConvertedXmlString = main.ClientMessageReturn;

                //HttpContext context = new HttpContext("ff");

                //if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
                //{
                //    HttpContext.Session.SetString(SessionKeyName, "The Doctor");
                //    HttpContext.Session.SetInt32(SessionKeyAge, 73);
                //}
                //var name = HttpContext.Session.GetString(SessionKeyName);
                //var age = HttpContext.Session.GetInt32(SessionKeyAge).ToString();

                

                //Debug.WriteLine("Session Name: {Name}", name);
                //Debug.WriteLine("Session Age: {Age}", age);

                //main.StopCommunication();




                return ConvertedXmlString;



            }
            catch(Exception e)
            {
                hatamesaj = e.ToString();
            }

            //XML'den Json'a çevrilen string döndürülecek.

            return String.Format("{0}ayayayay"+" gfdgsfs",3);
        }
    }
}
