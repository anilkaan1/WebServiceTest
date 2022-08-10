using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WebServiceTest.Data
{
    public interface MyXmlToJsonConverter
    {
        public void XmlToJsonSet(XElement MyXElement);
        public string ExecuteFunction();
    }
}
