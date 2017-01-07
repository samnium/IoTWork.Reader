using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IoTWork.Infrastructure.Helpers
{
    public static class SerializerHelper
    {
        public static string ToString(XmlDocument doc)
        {
            throw new NotImplementedException();

            //using (StringWriter sw = new StringWriter())
            //{
            //    using (XmlTextWriter tx = new XmlTextWriter(sw))
            //    {
            //        doc.WriteTo(tx);
            //        string strXmlText = sw.ToString();
            //        return strXmlText;
            //    }
            //}
        }

        public static string IndentXml(string xml)
        {
            var stringBuilder = new StringBuilder();

            var element = XElement.Parse(xml);

            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineOnAttributes = true;

            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }

            return stringBuilder.ToString();
        }

        #region XML Serializer
        public static string ToXml(object o)
        {
            throw new NotImplementedException();

            //StringWriter sw = new StringWriter();
            //XmlTextWriter tw = null;
            //try
            //{
            //    XmlSerializer serializer = new XmlSerializer(o.GetType());
            //    tw = new XmlTextWriter(sw);
            //    serializer.Serialize(tw, o);
            //}
            //catch (Exception ex)
            //{
            //    //Handle Exception Code
            //}
            //finally
            //{
            //    sw.Close();
            //    if (tw != null)
            //    {
            //        tw.Close();
            //    }
            //}
            //return sw.ToString();
        }

        public static Object FromXml(string xml, Type objectType)
        {
            throw new NotImplementedException();

            //StringReader strReader = null;
            //XmlSerializer serializer = null;
            //XmlTextReader xmlReader = null;
            //Object obj = null;
            //try
            //{
            //    strReader = new StringReader(xml);
            //    serializer = new XmlSerializer(objectType);
            //    xmlReader = new XmlTextReader(strReader);
            //    obj = serializer.Deserialize(xmlReader);
            //}
            //catch (Exception exp)
            //{
            //    //Handle Exception Code
            //}
            //finally
            //{
            //    if (xmlReader != null)
            //    {
            //        xmlReader.Close();
            //    }
            //    if (strReader != null)
            //    {
            //        strReader.Close();
            //    }
            //}
            //return obj;
        }
        #endregion

        #region Data Contract Serializer
        public static string XmlSerialize(object obj)
        {
            try
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (StreamReader reader = new StreamReader(memoryStream))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                        serializer.WriteObject(memoryStream, obj);
                        memoryStream.Position = 0;
                        return reader.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            {
                return String.Empty;
            }
        }

        public static object XmlDeserialize(string xml, Type toType)
        {
            try
            {
                using (Stream stream = new MemoryStream())
                {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                    stream.Write(data, 0, data.Length);
                    stream.Position = 0;
                    DataContractSerializer deserializer = new DataContractSerializer(toType);
                    return deserializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region CSV Simple Serializer
        public static string ToCsvHeader(object obj, string separator = ",")
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            PropertyInfo[] properties = obj.GetType().GetProperties();

            string csv;

            csv = String.Join(separator, fields.Select(f => f.Name).Concat(properties.Select(p => p.Name)).ToArray());
            csv = csv + Environment.NewLine;

            return csv;
        }

        public static string ToCsv(object obj, string separator = ",")
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            PropertyInfo[] properties = obj.GetType().GetProperties();
            string csv;

            csv = string.Join(separator, fields.Select(f => (Regex.Replace(Convert.ToString(f.GetValue(obj)), @"\t|\n|\r", "") ?? "").Trim())
               .Concat(properties.Select(p => (Regex.Replace(Convert.ToString(p.GetValue(obj, null)), @"\t|\n|\r", "") ?? "").Trim())).ToArray());

            return csv;
        }
        #endregion

        #region Binary
        public static string BinarySerialize(object item)
        {
            throw new NotImplementedException();

            //if (item != null)
            //{
            //    using (MemoryStream memoryStream = new MemoryStream())
            //    {
            //        BinaryFormatter binaryFormatter = new BinaryFormatter();
            //        binaryFormatter.Serialize(memoryStream, item);
            //        return Convert.ToBase64String(memoryStream.ToArray());
            //    }
            //}
            //return string.Empty;
        }

        public static object BinaryDeserialize(string item)
        {
            throw new NotImplementedException();

            //if (!string.IsNullOrEmpty(item))
            //{
            //    using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(item)))
            //    {
            //        BinaryFormatter binaryFormatter = new BinaryFormatter();
            //        return binaryFormatter.Deserialize(memoryStream);
            //    }
            //}
            //return null;
        }

        internal static string JSonSerialize(Object Obj)
        {
            return JsonConvert.SerializeObject(Obj);
        }
        #endregion
    }
}
