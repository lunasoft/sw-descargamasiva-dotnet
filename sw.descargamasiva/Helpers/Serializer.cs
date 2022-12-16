using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace sw.descargamasiva
{
    public static class Serializer
    {
        public static T Deserialize<T>(string xml)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                stream = new StringReader(xml);
                reader = new XmlTextReader(stream);
                return (T)serializer.Deserialize(reader);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Got exception on desealize: {ex}. At: Serializer.Deserialize");
                return default(T);
            }
            finally
            {
                if (stream != null) stream.Close();
                if (reader != null) reader.Close();
            }
        }
        
        public static string SerializeDocumentToXml<T>(T obj)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(T));
                FullXmlTextWriter xmlTextWriter = new FullXmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, obj);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                return RemoveInvalidCharacters(new UTF8Encoding().GetString(memoryStream.ToArray()));
            }
            catch
            {
                return string.Empty;
            }
        }
        
        public static string RemoveInvalidCharacters(string xmlInvoice)
        {
            xmlInvoice = xmlInvoice.Replace("\r\n", "");
            xmlInvoice = xmlInvoice.Replace("\r", "");
            xmlInvoice = xmlInvoice.Replace("\n", "");
            xmlInvoice = xmlInvoice.Replace(@"<?xml version=""1.0"" encoding=""utf-16""?>", @"<?xml version=""1.0"" encoding=""utf-8""?>").Trim();
            xmlInvoice = xmlInvoice.Replace("﻿", "");
            xmlInvoice = xmlInvoice.Replace(@"
", "");
            return xmlInvoice;
        }
    }
}