namespace Cipa.Domain.Helpers
{
    public class EmailConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnabledSSL { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Alias { get; set; }
        public string SESArn { get; set; }
    }
}
