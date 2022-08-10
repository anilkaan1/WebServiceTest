using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebServiceTest.Converters;

namespace WebServiceTest.Data
{
    public class XmlToJsonService:MyXmlToJsonConverter
    {
        JsonDocument myDocument { get; set; }

        string JsonDocumentStr { get; set; }

        JsonElement elementroot { get; set; }

        public void XmlToJsonSet(XElement MyXElement)
        {

            XDocument myXDocument = new XDocument();

            XDeclaration xDeclaration = new XDeclaration("1.0", "utf-8", "yes");

            myXDocument.Declaration = xDeclaration;

            myXDocument.AddFirst(MyXElement);

            string rerer = JsonXmlHelper.XDocumentToString(myXDocument);

            myXDocument = XDocument.Parse(rerer);

            JsonDocumentStr = JsonConvert.SerializeXNode(myXDocument.Root);   //serialize işleminde mecburen newtonsoft kulandık

            myDocument = System.Text.Json.JsonSerializer.Deserialize<JsonDocument>(JsonDocumentStr);

            elementroot = myDocument.RootElement;

            IEnumerator<JsonProperty> myObjectEnumarator22 = elementroot.EnumerateObject();

            myObjectEnumarator22.MoveNext();

            elementroot = myObjectEnumarator22.Current.Value;
        }

        public string ExecuteFunction()
        {
            string jsondoc;

            jsondoc = System.Text.Json.JsonSerializer.Serialize(elementroot, new JsonSerializerOptions { WriteIndented = true });

            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream);
                elementroot.WriteTo(writer);
                writer.Flush();
                string json = Encoding.UTF8.GetString(stream.ToArray());
            }

            var abcd = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(elementroot, elementroot.GetType(), new JsonSerializerOptions { WriteIndented = true });

            string result = System.Text.Encoding.UTF8.GetString(abcd);

            return result;

            

            //3 tane  stringe çevirme tipi implement edildi.  (jsondoc, json, result)

        }

    }
}
