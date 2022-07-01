using System.Text.Json;
using System.Xml.Linq;
using TCPIP_ServerClient;
using WebServiceTest.Converters;

namespace WebServiceTest.Data
{
    public class JsonToXmlService : MyConverter
    {
        private string hatamesaj { get; set; }
        private int CommandCode { get; set; }
        XDocument ConvertedXml { get; set; }
        string ConvertedXmlString {  get; set; }

        public JsonToXmlService(JsonDocument jsonDocument)
        {
            hatamesaj = "";

            try
            {
                CommandCode = JsonXmlHelper.FindCommandCodeFromJson(jsonDocument);

                if (CommandCode == 0)
                {
                    new ArgumentException("The Command Could Not Found");
                }

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

                TCPIPClientMain main = new TCPIPClientMain("192.168.1.1");
                main.StartCommunicationThread();



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
