using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GapCondition.Util
{
    public class XmlUtils
    {
        public void DeSerialize<T>(string link, out T data)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            using (TextReader reader = new StreamReader(link))
            {
                object obj = deserializer.Deserialize(reader);
                T XmlData = (T)obj;
                data = XmlData;
                reader.Close();
            }
        }
        public void Serialize<T>(string link, T data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (XmlTextWriter writer = new XmlTextWriter(link, Encoding.GetEncoding("UTF-8")))
            {
                serializer.Serialize(writer, data);
            }
        }
    }
}
