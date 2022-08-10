using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WebServiceTest.Converters
{
    [DataContract]
    public class SerializeTest
    {
        //[DataMember]
        //public Dictionary<string, object> Map_list { set; get; }
        //public string CommandName { set; get; }
        //public SerializeTest(string commandName)
        //{
        //    Map_list = new Dictionary<string, object>();
        //    CommandName = commandName;
        //}
        //private static DataContractSerializer serializer;
        //public static string Serialize(SerializeTest sTest)
        //{
        //    using (StringWriter sw = new StringWriter())
        //    {
        //        using (XmlWriter writer = XmlWriter.Create(sw))
        //        {
        //            writer.WriteStartElement(sTest.CommandName);   
        //            foreach(KeyValuePair<string,object> entry in sTest.Map_list)
        //            {
        //                writer.WriteElementString(entry.Key, (string)entry.Value);
        //            }
        //            //serializer.WriteObject(writer, sTest);
        //            XDocument bbb = new XDocument();
        //            XmlDocument fd = new XmlDocument();
        //            XmlElement aaaaa = fd.CreateElement("fds","fdsa");
        //        }
        //        return sw.ToString();
        //    }
        //}
        //public static SerializeTest DeSerialize(string filePath)
        //{
        //    serializer = new DataContractSerializer(typeof(SerializeTest));
        //    SerializeTest serializeTest;
        //    using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //    {
        //        serializeTest = serializer.ReadObject(reader) as SerializeTest;
        //    }
        //    return serializeTest;
        //}
    }
    public static class JsonXmlHelper
    {
        public static int FindCommandCodeFromJson(JsonDocument myJsonDocument)  //Commandı Buluyoruz.
        {
            IEnumerable<JsonProperty> myObjectEnumarator = myJsonDocument.RootElement.EnumerateObject();

            Func<JsonProperty, bool> predicate = x => x.Name.Equals("Command");

            JsonProperty myprotest = myObjectEnumarator.First(predicate);

            string CommandText = myprotest.Value.ToString();

            FunctionsEnum a;

            Enum.TryParse<FunctionsEnum>(CommandText, true, out a);

            int CommandCode = (int)(a);

            return CommandCode;

        }
        public static XDocument ConvertJsonToXml(JsonDocument jsonDocument)
        {
            JsonElement myRootElement = jsonDocument.RootElement;

            //3 FONKSİYON ÜZERİNDEN RECURSİVE GİDİLECEK,1. ROOT İLE ÇALIŞIR+ ,2. OBJECT İLE ÇALIŞIR, 3. ARRAY İLE ÇALIŞIR.

            try
            {
                XDocument myDoc = new XDocument();

                if (jsonDocument.RootElement.ValueKind.ToString() == "Object") //Ana json her zmana obje olur.
                {
                    CreateXMLRootStructure(myRootElement, myDoc);
                }
                else
                {
                    throw new ArgumentException("The Json Is Empty or Not Object"); 
                }
                           

                return myDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        //object xelement bu xdocument kabul eder
        //her seferinde rootta mıyız değil miyiz kontrol etmemiz gerekecek
        //root'u json element şeklinde dışarıdan oluşturup eklememiz gerekecek bu da gereksiz bağlılık yaratır.

        public static void CreateXMLRootStructure(JsonElement jsonElement, XDocument xDocument)
        {
            XDeclaration declaration = new XDeclaration("1.0", "utf-8", "yes");

            XElement rootElement = new XElement("root");

            xDocument.Declaration = declaration;

            xDocument.AddFirst(rootElement);

            IEnumerator<JsonProperty> myObjectEnumarator22 = jsonElement.EnumerateObject();

            IEnumerable<JsonProperty> myObjectEnumarator = jsonElement.EnumerateObject();

            string test1 = xDocument.ToString();  //Root üretildi mi ?.

            if (xDocument.Root != null)
            {
                while (myObjectEnumarator22.MoveNext())
                {
                    if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "String")
                    {
                        XElement newElement = new XElement(myObjectEnumarator22.Current.Name, myObjectEnumarator22.Current.Value.ToString());

                        bool abcd = newElement.HasElements;

                        rootElement.Add(newElement);

                        //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                    }
                    else if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "Integer")
                    {
                        XElement newElement = new XElement(myObjectEnumarator22.Current.Name, myObjectEnumarator22.Current.Value.ToString());

                        rootElement.Add(newElement);

                        //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                    }
                    else if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "Object")
                    {
                        XElement newElement = new XElement(myObjectEnumarator22.Current.Name);

                        rootElement.Add(newElement);

                        JsonObjectToXElement(myObjectEnumarator22.Current.Value, newElement);


                        //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                    }
                    else if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "Array")
                    {
                        XElement newElement = new XElement(myObjectEnumarator22.Current.Name);

                        rootElement.Add(newElement);

                        JsonArrayToXElement(myObjectEnumarator22.Current.Value, newElement);

                        //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                    }

                }
            }

            
        }
        public static void JsonObjectToXElement(JsonElement jsonElement, XElement xelement)
        {
            bool abcd = xelement.HasElements;


            IEnumerator<JsonProperty> myObjectEnumarator22 = jsonElement.EnumerateObject();

            IEnumerable<JsonProperty> myObjectEnumarator = jsonElement.EnumerateObject();

            while (myObjectEnumarator22.MoveNext())
            {
                if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "String")
                {
                    XElement newElement = new XElement(myObjectEnumarator22.Current.Name, myObjectEnumarator22.Current.Value.ToString());

                    xelement.Add(newElement);

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
                else if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "Integer")
                {
                    XElement newElement = new XElement(myObjectEnumarator22.Current.Name, myObjectEnumarator22.Current.Value.ToString());

                    xelement.Add(newElement);

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
                else if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "Object")
                {
                    XElement newElement = new XElement(myObjectEnumarator22.Current.Name);

                    xelement.Add(newElement);

                    JsonObjectToXElement(myObjectEnumarator22.Current.Value,newElement);

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
                else if (myObjectEnumarator22.Current.Value.ValueKind.ToString() == "Array")
                {
                    XElement newElement = new XElement(myObjectEnumarator22.Current.Name);

                    xelement.Add(newElement);

                    JsonArrayToXElement(myObjectEnumarator22.Current.Value, newElement);

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
            }

            myObjectEnumarator.GetEnumerator().Reset();
        }
        public static void JsonArrayToXElement(JsonElement jsonElement, XElement xelement)
        {
            IEnumerator<JsonElement> myObjectEnumarator22 = jsonElement.EnumerateArray();

            IEnumerable<JsonElement> myObjectEnumarator = jsonElement.EnumerateArray();

            int index = 0;

            while (myObjectEnumarator22.MoveNext())
            {
                if (myObjectEnumarator22.Current.ValueKind.ToString() == "String")
                {
                    XElement newElement = new XElement(string.Format("Value"+index.ToString()), myObjectEnumarator22.Current.ToString());

                    xelement.Add(newElement);

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();

                    index += 1;
                }
                else if (myObjectEnumarator22.Current.ValueKind.ToString() == "Integer")
                {
                    XElement newElement = new XElement(string.Format("Value" + index.ToString()), myObjectEnumarator22.Current.ToString());

                    xelement.Add(newElement);

                    index += 1;

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
                else if (myObjectEnumarator22.Current.ValueKind.ToString() == "Object")
                {
                    string name = string.Format("Value" + index.ToString());

                    JsonElement value = myObjectEnumarator22.Current;

                    XElement newElement = new XElement(name);

                    xelement.Add(newElement);

                    JsonObjectToXElement(value, newElement);

                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
                else if (myObjectEnumarator22.Current.ValueKind.ToString() == "Array")
                {
                    XElement newElement = new XElement(string.Format("Value" + index.ToString()));

                    xelement.Add(newElement);

                    index += 1;

                    JsonArrayToXElement(myObjectEnumarator22.Current, newElement);


                    //IEnumerable<JsonElement> myArrayEnumarator = myObjectEnumarator22.Current.Value.EnumerateArray();
                }
            }

        }
        public static string XDocumentToString(XDocument myDocument)
        {
            string newstr = "";

            MemoryStream ms = new MemoryStream();

            using (StringWriter writer = new Utf8StringWriter())
            {
                myDocument.Save(writer,SaveOptions.None);
                newstr = writer.ToString();
            }

            return newstr;
        }
        





        public static XDocument PrepareSetRefreshTokenData(string UID, string GROUPID, string SESSIONID,string token,string refreshtoken)
        {
            XDocument reqDoc = new XDocument();

            XElement elementa = new XElement("ITQDOC");

            reqDoc.Add(elementa);

            XElement elementUID = new XElement("USERID",UID);

            XElement elementGROUID = new XElement("GROUPID",GROUPID);

            XElement elementSESSIONID= new XElement("SESSIONID",SESSIONID);

            XElement elementREFRESHTOKEN = new XElement("REFRESHTOKEN", refreshtoken);

            XElement elementTOKEN = new XElement("TOKEN", token);

            XElement elementCOMMAND = new XElement("COMMAND", "SETTOKENINFO");


            elementa.Add(elementUID);
            elementa.Add(elementGROUID);
            elementa.Add(elementSESSIONID);
            elementa.Add(elementCOMMAND);
            elementa.Add(elementREFRESHTOKEN);
            elementa.Add(elementTOKEN);


            return reqDoc;


        }













    }
    }