using System.Security.Cryptography.X509Certificates;

namespace sw.descargamasiva.Models
{
    public class RequestDownloadParameters : IParameters
    {
        public RequestDownloadParameters(
            X509Certificate2 certificate,
            string rfcEmisor, string[] rfcReceptor, 
            string rfcSolicitante, string fechaInicial = "", string fechaFinal = "", string tipoSolicitud = "CFDI",
            string rfcTpAccount = "", string cfdiType = "", string cfdiComplement = "")
        {
            RfcEmisor = rfcEmisor;
            RfcReceptor = rfcReceptor;
            RfcSolicitante = rfcSolicitante;
            FechaInicial = fechaInicial;
            FechaFinal = fechaFinal;
            TipoSolicitud = tipoSolicitud;
            RfcTPAccount = rfcTpAccount;
            CFDIType = cfdiType;
            CFDIComplement = cfdiComplement;
            Certificate = certificate;
        }
        public readonly X509Certificate2 Certificate;
        public readonly string RfcEmisor;
        public readonly string[] RfcReceptor;
        public readonly string RfcSolicitante;
        public readonly string FechaInicial;
        public readonly string FechaFinal;
        public readonly string TipoSolicitud;
        public readonly string RfcTPAccount;
        public readonly string CFDIType;
        public readonly string CFDIComplement;
    }
}