using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using sw.descargamasiva.Models;

namespace sw.descargamasiva
{
    class Program
    {
        static byte[] pfx = File.ReadAllBytes(@"Resources\pfx.pfx");
        static string password = "";

        static string urlAutentica = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/Autenticacion/Autenticacion.svc";

        static string urlSolicitud = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/SolicitaDescargaService.svc";

        static string urlVerificarSolicitud = "https://cfdidescargamasivasolicitud.clouda.sat.gob.mx/VerificaSolicitudDescargaService.svc";

        static string urlDescargarSolicitud = "https://cfdidescargamasiva.clouda.sat.gob.mx/DescargaMasivaService.svc";
        static string urlDescargarSolicitudAction = "http://DescargaMasivaTerceros.sat.gob.mx/IDescargaMasivaTercerosService/Descargar";

        static string RfcEmisor = ""; 
        static string RfcSolicitante = "";
        static string[] RfcReceptor = new []{ "" };
        static string FechaInicial = "2018-12-01";
        static string FechaFinal = "2018-12-02";
        
        static void Main(string[] args)
        {
            //Obtener Token
            string token = ObtenerToken();
            string autorization = String.Format("WRAP access_token=\"{0}\"", HttpUtility.UrlDecode(token));
            Console.WriteLine("Token: " + token);

            //Generar Solicitud
            string idSolicitud = GenerarSolicitud(autorization);
            Console.WriteLine("IdSolicitud: " + idSolicitud);

            //Validar Solicitud
            string idPaquete = VerificarSolicitud(autorization, idSolicitud);
            Console.WriteLine("IdPaquete: " + idPaquete);

            if (!string.IsNullOrEmpty(idPaquete))
            {
                //Descargar Solicitud
                string descargaResponse = DescargarSolicitud(autorization, idPaquete);

                GuardarSolicitud(idPaquete, descargaResponse);
            }
            Console.ReadLine();
        }

        private static X509Certificate2 ObtenerX509Certificado(byte[] pfx)
        {
            return new X509Certificate2(pfx, password, X509KeyStorageFlags.MachineKeySet |
                            X509KeyStorageFlags.PersistKeySet |
                            X509KeyStorageFlags.Exportable);
        }
        
        private static string ObtenerToken()
        {
            var service = new AuthenticaV2(urlAutentica);
            var parameters = new AuthenticaParameters(pfx, password);
            service.Generate(parameters); 
            var response = service.Call().Result;
            return response;
        }
        
        private static string GenerarSolicitud(string autorization)
        {
            var certificate = ObtenerX509Certificado(pfx);
            var verifyService = new GenerarSolicitudV2(urlSolicitud);
            var parameters = new RequestDownloadParameters(certificate, password, RfcEmisor, RfcReceptor, RfcEmisor, FechaInicial, FechaFinal);
            var requestXml = verifyService.Generate(parameters);
            var response = verifyService.Call(requestXml, autorization).Result;
            return Serializer.SerializeDocumentToXml(response);
        }
        
        private static string VerificarSolicitud(string autorization, string idSolicitud)
        {
            var certificate = ObtenerX509Certificado(pfx);
            var verifyService = new VerificaSolicitudV2(urlVerificarSolicitud);
            var parameters = new VerifyRequestDownloadParameters(certificate, password, RfcSolicitante, idSolicitud);
            var requestXml = verifyService.Generate(parameters);
            var response = verifyService.Call(requestXml, autorization).Result;
            return Serializer.SerializeDocumentToXml(response);
        }

        private static string DescargarSolicitud(string autorization, string idPaquete)
        {
            var certifcate = ObtenerX509Certificado(pfx);
            DescargarSolicitud descargarSolicitud = new DescargarSolicitud(urlDescargarSolicitud, urlDescargarSolicitudAction);
            string xmlDescarga = descargarSolicitud.Generate(certifcate, RfcEmisor, idPaquete);
            return descargarSolicitud.Send(autorization);
        }
        
        private static void GuardarSolicitud(string idPaquete, string descargaResponse)
        {
            string path = "./Paquetes/";
            byte[] file = Convert.FromBase64String(descargaResponse);
            Directory.CreateDirectory(path);

            using (FileStream fs = File.Create(path + idPaquete + ".gzip", file.Length))
            {
                fs.Write(file, 0, file.Length);
            }
            Console.WriteLine("FileCreated: " + path + idPaquete + ".gzip");
        }
    }
}
