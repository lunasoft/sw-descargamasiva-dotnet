using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using sw.descargamasiva.Models;
using sw.descargamasiva.VerificaSolicitudDescargaService;

namespace sw.descargamasiva
{
    public class VerificaSolicitudV2 : ISatService<RespuestaVerificaSolicitudDescMasTercero>
    {
        private readonly VerificaSolicitudDescargaServiceClient _client;

        public VerificaSolicitudV2(string url)
        { 
            _client = GetClient(url);
        }
        
        public string Generate(IParameters parameters)
        {
            if (!(parameters is VerifyRequestDownloadParameters verifyRequestDownloadParameters))
                throw new ArgumentException("Invalid Parameters.");

            var request = new VerificaSolicitudDescargaMasivaTercero
            {
                IdSolicitud = verifyRequestDownloadParameters.IdSolicitud,
                RfcSolicitante = verifyRequestDownloadParameters.RfcSolicitante
            };

            var xmlRequest = Serializer.SerializeDocumentToXml(request);
            var signedRequest = Signer.SingXml(
                verifyRequestDownloadParameters.Certificate, 
                verifyRequestDownloadParameters.Password,
                xmlRequest,
                "VerificaSolicitudDescargaMasivaTercero");
            
            return signedRequest;
        }
        

        public async Task<RespuestaVerificaSolicitudDescMasTercero> Call(string xml, string authorization)
        {
            var request = Serializer.Deserialize<VerificaSolicitudDescargaMasivaTercero>(xml);
            
            using OperationContextScope scope = new OperationContextScope(_client.InnerChannel);
            OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = new HttpRequestMessageProperty()
            {
                Headers =
                {
                    { "Authorization", authorization}
                }
            };

            return await _client.VerificaSolicitudDescargaAsync(request);
        }
        
        private VerificaSolicitudDescargaServiceClient GetClient(string endpoint)
        {
            var endpointAddress = new EndpointAddress(new Uri(endpoint));
            var binding = new BasicHttpBinding
            {
                Security =
                {
                    Mode = BasicHttpSecurityMode.Transport
                }
            };
            return new VerificaSolicitudDescargaServiceClient(binding, endpointAddress);
        }
    }
}