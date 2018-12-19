using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sw.descargamasiva
{
    internal class VerificaSolicitud : SoapRequestBase
    {
        public VerificaSolicitud(string url, string action)
           : base(url, action)
        {
        }

        #region Crea el XML para enviar
        public string Generate(X509Certificate2 certificate, string rfcSolicitante, string idSolicitud)
        {
            string canonicalTimestamp = "<des:VerificaSolicitudDescarga xmlns:des=\"http://DescargaMasivaTerceros.sat.gob.mx\">"
                + "<des:solicitud IdSolicitud=\"" + idSolicitud + "\" RfcSolicitante=\"" + rfcSolicitante + ">"
                + "</des:solicitud>"
                + "</des:VerificaSolicitudDescarga>";

            string digest = System.Convert.ToBase64String(Digest(System.Text.Encoding.Default.GetBytes(canonicalTimestamp)));

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
            string signature = System.Convert.ToBase64String(Sign(System.Text.Encoding.Default.GetBytes(canonicalSignedInfo), certificate));
            string soap_request = @"<s:Envelope xmlns:s=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"" xmlns:des=""http://DescargaMasivaTerceros.sat.gob.mx"" xmlns:xd=""http://www.w3.org/2000/09/xmldsig#"">" +
                        @"<s:Header/>" +
                        @"<s:Body>" +
                            @"<des:VerificaSolicitudDescarga>" +
                                @"<des:solicitud "+
                                @"IdSolicitud=""" + idSolicitud +
                                @""" RfcSolicitante=""" + rfcSolicitante +
                                @""">" +
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
                                                        @"<X509Data>" +
                                                            @"<X509IssuerSerial>" +
                                                                @"<X509IssuerName>" + certificate.Issuer +
                                                                @"</X509IssuerName>" +
                                                                @"<X509SerialNumber>" + certificate.SerialNumber +
                                                                @"</X509SerialNumber>" +
                                                            @"</X509IssuerSerial>" +
                                                            @"<X509Certificate>" + Convert.ToBase64String(certificate.RawData) + "</X509Certificate>" +
                                                        @"</X509Data>" +
                                                    @"</KeyInfo>" +
                                                    @"</Signature>" +
                                                    @"</des:solicitud>" +
                                                @"</des:VerificaSolicitudDescarga>" +
                                            @"</s:Body>" +
                                            @"</s:Envelope>";
            xml = soap_request;
            return soap_request;
        }
        #endregion
        public byte[] Digest(byte[] sourceData)
        {
            return HashAlgorithm.Create("SHA1").ComputeHash(sourceData);
        }
        public byte[] Sign(byte[] sourceData, X509Certificate2 certificate)
        {
            byte[] signature = null;

            using (RSA rsaCryptoServiceProvider = certificate.GetRSAPrivateKey())
            {
                signature = rsaCryptoServiceProvider.SignData(sourceData, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            }
            return signature;
        }
        public override string GetResult(XmlDocument xmlDoc)
        {
            string s = string.Empty;
            int cfdis = Convert.ToInt16(xmlDoc.GetElementsByTagName("VerificaSolicitudDescargaResult")[0].Attributes["NumeroCFDIs"].Value);
            if(cfdis > 0)
            {
               s = xmlDoc.GetElementsByTagName("IdsPaquetes")[0].InnerXml;
            }
            return s;
        }
    }
}
