using System;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace sw.descargamasiva
{
    internal class Autenticacion : SoapRequestBase
    {
        public Autenticacion(string url, string action)
           : base(url, action)
        {
        }

        #region Crea el XML para enviar
        public string Generate(X509Certificate2 certificate)
        {
            string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
            DateTime date = DateTime.UtcNow;
            string created = date.ToString(format);
            string expires = date.AddSeconds(300).ToString(format);
            string uuid = "uuid-" + Guid.NewGuid().ToString() + "-1";

            string canonicalTimestamp = @"<u:Timestamp xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" u:Id=""_0"">" +
                                            "<u:Created>" + created + "</u:Created>" +
                                            "<u:Expires>" + expires + "</u:Expires>" +
                                         "</u:Timestamp>";
            string digest = CreateDigest(canonicalTimestamp);
            string canonicalSignedInfo = @"<SignedInfo xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                            @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></CanonicalizationMethod>" +
                                            @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""></SignatureMethod>" +
                                            @"<Reference URI=""#_0"">" +
                                               "<Transforms>" +
                                                  @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""></Transform>" +
                                               "</Transforms>" +
                                               @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""></DigestMethod>" +
                                               "<DigestValue>" + digest + "</DigestValue>" +
                                            "</Reference>" +
                                         "</SignedInfo>";
            string signature = Sign(canonicalSignedInfo, certificate);
            string soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">" +
                                    @"<s:Header>" +
                                     @"<o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">" +
                                      @"<u:Timestamp u:Id=""_0"">" +
                                       @"<u:Created>" + created + "</u:Created>" +
                                       @"<u:Expires>" + expires + "</u:Expires>" +
                                      @"</u:Timestamp>" +
                                      @"<o:BinarySecurityToken u:Id=""" + uuid + @""" ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" EncodingType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"">" +
                                       Convert.ToBase64String(certificate.RawData) +
                                      @"</o:BinarySecurityToken>" +
                                      @"<Signature xmlns=""http://www.w3.org/2000/09/xmldsig#"">" +
                                       @"<SignedInfo>" +
                                        @"<CanonicalizationMethod Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                        @"<SignatureMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#rsa-sha1""/>" +
                                        @"<Reference URI=""#_0"">" +
                                         @"<Transforms>" +
                                          @"<Transform Algorithm=""http://www.w3.org/2001/10/xml-exc-c14n#""/>" +
                                         @"</Transforms>" +
                                         @"<DigestMethod Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>" +
                                         @"<DigestValue>" + digest + @"</DigestValue>" +
                                        @"</Reference>" +
                                       @"</SignedInfo>" +
                                       @"<SignatureValue>" + signature + "</SignatureValue>" +
                                       @"<KeyInfo>" +
                                        @"<o:SecurityTokenReference>" +
                                         @"<o:Reference ValueType=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-x509-token-profile-1.0#X509v3"" URI=""#" + uuid + @"""/>" +
                                        @"</o:SecurityTokenReference>" +
                                       @"</KeyInfo>" +
                                      @"</Signature>" +
                                     @"</o:Security>" +
                                    @"</s:Header>" +
                                    @"<s:Body>" +
                                     @"<Autentica xmlns=""http://DescargaMasivaTerceros.gob.mx""/>" +
                                    @"</s:Body>" +
                                   @"</s:Envelope>";
            xml = soap_request;
            return soap_request;
        }
        #endregion

        public override string GetResult(XmlDocument xmlDoc)
        {
            string s = xmlDoc.GetElementsByTagName("AutenticaResult")[0].InnerXml;
            return s;
        }
    }
}
