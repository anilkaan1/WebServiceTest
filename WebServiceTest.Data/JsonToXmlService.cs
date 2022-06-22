using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
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


                return ConvertedXmlString;



            }
            catch(Exception e)
            {
                hatamesaj = e.ToString();
            }

            //XML'den Json'a çevrilen string döndürülecek.

            return String.Format("{0}ayayayay"+" gfdgsfs",3);
        }


        //public Dictionary<string ,object> Conversion(Dictionary<string,object> notConvertDict)
        //{
        //    Dictionary<string, object> result = notConvertDict;



        //    foreach (KeyValuePair<string, object> entry in notConvertDict)
        //    {
        //        string valueType = ((JsonElement)notConvertDict[entry.Key]).ValueKind.ToString();


        //        if(valueType == "String")
        //        {
        //            result[entry.Key] = ((JsonElement)notConvertDict[entry.Key]).ToString();
        //        }

        //        else if (valueType == "Integer")
        //        {
        //            result[entry.Key] = Convert.ToInt32((JsonElement)notConvertDict[entry.Key]);
        //        }

        //        else if (valueType == "Array")
        //        {
                    
        //            foreach(var a in ((JsonElement)notConvertDict[entry.Key]).EnumerateArray()){


        //                if (a.ValueKind.ToString() == "String")
        //                {
        //                    result[entry.Key] = ((JsonElement)notConvertDict[entry.Key]).ToString();
        //                }





        //            }

        //            result[entry.Key] = notConvertDict[entry.Key].ToString();

        //        }
                

        //        else if (valueType == "Object")
        //        {
        //            Dictionary<string, object> ToBeConverted = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>((JsonElement)notConvertDict[entry.Key]);

        //            result[entry.Key] = Conversion(ToBeConverted);
        //        }
                
        //    }
        //    return result;
        //}
       
    }
}
