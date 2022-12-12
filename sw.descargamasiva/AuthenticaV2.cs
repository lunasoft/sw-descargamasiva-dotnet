using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading.Tasks;
using sw.descargamasiva.Autenticacion;
using sw.descargamasiva.Models;

namespace sw.descargamasiva
{
    public class AuthenticaV2 : ISatService<string>
    {
        private readonly AutenticacionClient _client;

        public AuthenticaV2(string url)
        { 
            var endpointAddress = new EndpointAddress(new Uri(url));
            _client = new AutenticacionClient(GetBinding(), endpointAddress);
        }
        
        public string Generate(IParameters parameters)
        {
            if (!(parameters is AuthenticaParameters authenticateParameters))
                throw new ArgumentException("Invalid Parameters.");

            X509Certificate2 certificate = new X509Certificate2(authenticateParameters.Certificate, authenticateParameters.Password);
            if (_client.ClientCredentials != null)
                _client.ClientCredentials.ClientCertificate.Certificate ??= certificate;

            return string.Empty;
        }
        

        public async Task<string> Call(string xml = "", string authorization = "")
        {
            return await _client.AutenticaAsync();
        }
        
        private BasicHttpBinding GetBinding()
        {
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
            binding.Security.Message = new BasicHttpMessageSecurity()
            {
                ClientCredentialType = BasicHttpMessageCredentialType.Certificate
            };
            return binding;
        }
    }
}