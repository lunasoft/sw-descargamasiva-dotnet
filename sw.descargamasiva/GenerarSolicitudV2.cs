using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using sw.descargamasiva.Models;
using sw.descargamasiva.SolicitaDescargaService;

namespace sw.descargamasiva
{
    internal class GenerarSolicitudV2 : ISatService<RespuestaSolicitudDescMasTercero>
    {
        private readonly SolicitaDescargaServiceClient _client;

        public GenerarSolicitudV2(string url)
        {
            _client = GetClient(url);
        }

        public string Generate(IParameters parameters)
        {
               if (!(parameters is RequestDownloadParameters downloadParameters))
                throw new ArgumentException(
                    "Invalid Parameters.");
               
               Enum.TryParse(downloadParameters.TipoSolicitud, out TipoDescargaMasivaTerceros tipoSolicitud);

               var request = new SolicitudDescargaMasivaTercero()
               {
                   RfcEmisor = downloadParameters.RfcEmisor,
                   RfcSolicitante = downloadParameters.RfcSolicitante,
                   RfcReceptores = downloadParameters.RfcReceptor,
                   FechaInicial = GetDateTime($"{downloadParameters.FechaInicial} 00:00:00"),
                   FechaInicialSpecified = true,
                   FechaFinal =  GetDateTime($"{downloadParameters.FechaFinal} 23:59:59"),
                   FechaFinalSpecified = true,
                   TipoSolicitud = tipoSolicitud,
                   EstadoComprobanteSpecified = false,
                   RfcACuentaTerceros = !string.IsNullOrEmpty(downloadParameters.RfcTPAccount) ? downloadParameters.RfcTPAccount : null,
                   Complemento = !string.IsNullOrEmpty(downloadParameters.CFDIComplement) ? downloadParameters.CFDIComplement : null,
               };
            
               if (!string.IsNullOrEmpty(downloadParameters.CFDIType))
               {
                   Enum.TryParse(downloadParameters.CFDIType, out TipoDeComprobante tipoComprobante);
                   request.TipoComprobante = tipoComprobante;
                   request.TipoComprobanteSpecified = true;
               }

               string xmlRequest = Serializer.SerializeDocumentToXml(request);
               string signedRequest = Signer.SingXml(
                   downloadParameters.Certificate,
                   xmlRequest);

               return signedRequest;
        }

        public RespuestaSolicitudDescMasTercero Call(string xml, string authorization)
        {
            var request = Serializer.Deserialize<SolicitudDescargaMasivaTercero>(xml);
            using OperationContextScope scope = new OperationContextScope(_client.InnerChannel);
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty()
            {
                Headers =
                {
                    { "Authorization", authorization}
                }
            };
            
            return _client.SolicitaDescargaAsync(request).Result;
        }
        
        private SolicitaDescargaServiceClient GetClient(string endpoint)
        {
            var endpointAddress = new EndpointAddress(new Uri(endpoint));
            var binding = new BasicHttpBinding
            {
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport
                }
            };
            return new SolicitaDescargaServiceClient(binding, endpointAddress);
        }
        private DateTime GetDateTime(string date) =>  DateTime.ParseExact(date, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

    }
}