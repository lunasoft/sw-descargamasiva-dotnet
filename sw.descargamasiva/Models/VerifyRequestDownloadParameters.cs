using System.Security.Cryptography.X509Certificates;

namespace sw.descargamasiva.Models
{
    public class VerifyRequestDownloadParameters : IParameters
    {
        public VerifyRequestDownloadParameters(
            X509Certificate2 certificate,
            string password,
            string rfcSolicitante,
            string idSolicitud)
        {
            RfcSolicitante = rfcSolicitante;
            IdSolicitud = idSolicitud;
            Password = password;
            Certificate = certificate;
        }

        public readonly X509Certificate2 Certificate;
        public readonly string Password;
        public readonly string RfcSolicitante;
        public readonly string IdSolicitud;
    }
}