using System;
using System.IO;
using System.Net;
using System.Xml;

namespace sw.descargamasiva
{
    internal abstract class SoapRequestBase
    {
        protected string xml;


        protected HttpWebRequest webRequest;

        /// <summary>
        /// Genera el constructor con los datos esenciales para enviar enviar el XML, mas no genera el XML
        /// </summary>
        /// <param name="NameToFindURL">Nombre del endpoint del archivo de configuración</param>
        /// <param name="SOAPAction">URL con información del metodo que ejecutara</param>
        /// <param name="Digest">Función enviada para ejecutar la operación de digestión</param>
        /// <param name="Sign">Función enviada para ejecutar la operación de timbrado</param>
        protected SoapRequestBase(string url, string SOAPAction)
        {
            this.xml = null;
            webRequest = WebRequest(url, SOAPAction);
        }

        /// <summary>
        /// Genera el constructor con los datos esenciales para enviar enviar el XML, mas no genera el XML
        /// </summary>
        /// <param name="NameToFindURL">Nombre del endpoint del archivo de configuración</param>
        /// <param name="SOAPAction">URL con información del metodo que ejecutara</param>
        /// <param name="Digest">Función enviada para ejecutar la operación de digestión</param>
        /// <param name="Sign">Función enviada para ejecutar la operación de timbrado</param>
        protected SoapRequestBase(Uri URL, string SOAPAction)
        {
            this.xml = null;
            webRequest = WebRequest(URL.ToString(), SOAPAction);
        }

        /// <summary>
        /// Envia el xml formado mediante una subclase
        /// </summary>
        /// Lanzada cuando no se proporcionan valores a la propiedad XML o cuando ocurre un problema de comunicación
        /// </exception>
        /// <returns>XML optenido del Web Response</returns>
        public string Send(string autorization = null)
        {
            try
            {
                if (xml == null)
                    throw new Exception("No se ha proporcionado ningún valor a la propiedad \"xml\"");

                HttpWebRequest request = webRequest;
                if (!string.IsNullOrEmpty(autorization))
                {
                    request.Headers.Add(HttpRequestHeader.Authorization, autorization);
                }
                using (Stream stream = request.GetRequestStream())
                {
                    using (StreamWriter stmw = new StreamWriter(stream))
                    {
                        stmw.Write(xml);
                    }
                }

                WebResponse response = (WebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string result = sr.ReadToEnd();
                sr.Close();

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                return GetResult(xmlDoc);
               
            }
            catch (WebException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public abstract string GetResult(XmlDocument xmlDoc);
 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="SOAPAction"></param>
        /// <returns></returns>
        private static HttpWebRequest WebRequest(string URL, string SOAPAction, int maxTimeMilliseconds = 120000)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            webRequest.Timeout = maxTimeMilliseconds;//Milisecons
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Headers.Add("SOAPAction: " + SOAPAction);
            return webRequest;
        }
    }
}
