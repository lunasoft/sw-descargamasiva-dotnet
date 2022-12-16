namespace sw.descargamasiva.Models
{
    public class AuthenticaParameters : IParameters
    {
        public AuthenticaParameters(
            byte[] certificate,string password)
        {
            this.Certificate = certificate;
            this.Password = password;
        }
        
        public readonly byte[] Certificate;
        public readonly string Password;
    }
}