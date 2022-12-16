using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Security.Cryptography.Xml;

namespace sw.descargamasiva
{
    public static class Signer
    {
        public static string SingXml(X509Certificate2 cert, string xmlRequest)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlRequest);
            CspParameters rsaParameters = new CspParameters();
            rsaParameters.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider key = new RSACryptoServiceProvider(cert.PrivateKey.KeySize, rsaParameters);
            SignedXml signedXml = new SignedXml(doc) { SigningKey = key };
            Reference reference = new Reference() { Uri = String.Empty };
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            XmlDsigExcC14NTransform c14N = new XmlDsigExcC14NTransform();
            reference.AddTransform(env);
            reference.AddTransform(c14N);
            KeyInfoX509Data kdata = new KeyInfoX509Data(cert);
            if (cert.SerialNumber != null) kdata.AddIssuerSerial(cert.Issuer, cert.SerialNumber);
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(kdata);
            signedXml.KeyInfo = keyInfo;
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            var xmlDigitalSignature =  signedXml.GetXml();
            doc.DocumentElement?.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            if (doc.FirstChild is XmlDeclaration)  
            {
                doc.RemoveChild(doc.FirstChild);
            }

            return doc.OuterXml;
        }
    }
}