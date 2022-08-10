using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebServiceTest.Data
{
    public interface MyJsonToXmlConverter
    {
        public void JsonToXmlSet(JsonDocument jsonDocument);
        public string ExecuteFunction();
    }
}
