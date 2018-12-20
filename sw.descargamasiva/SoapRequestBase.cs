using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace sw.descargamasiva
{
    internal abstract class SoapRequestBase
    {
        protected string xml;

        protected HttpWebRequest webRequest;

        protected SoapRequestBase(string url, string SOAPAction)
        {
            this.xml = null;
            webRequest = WebRequest(url, SOAPAction);
        }

        protected SoapRequestBase(Uri URL, string SOAPAction)
        {
            this.xml = null;
            webRequest = WebRequest(URL.ToString(), SOAPAction);
        }

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
 
        private static HttpWebRequest WebRequest(string URL, string SOAPAction, int maxTimeMilliseconds = 120000)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            webRequest.Timeout = maxTimeMilliseconds;//Milisecons
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml; charset=utf-8";
            webRequest.Headers.Add("SOAPAction: " + SOAPAction);
            return webRequest;
        }

        public string CreateDigest(string sourceData)
        {
            byte[] data = GetBytes(sourceData);
            return System.Convert.ToBase64String(HashAlgorithm.Create("SHA1").ComputeHash(data));
        }
        public string Sign(string sourceData, X509Certificate2 certificate)
        {
            byte[] data = GetBytes(sourceData);
            byte[] signature = null;

            using (RSA rsaCryptoServiceProvider = certificate.GetRSAPrivateKey())
            {
                signature = rsaCryptoServiceProvider.SignData(data, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
            return System.Convert.ToBase64String(signature);
        }
        private byte[] GetBytes(string sourceData)
        {
            return Encoding.Default.GetBytes(sourceData);
        }
    }
}
