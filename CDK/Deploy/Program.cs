using Amazon.CDK;

namespace Deploy
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = new CipaStack(app, nameof(CipaStack), new StackProps
            {
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
                }
            });
            _ = new CertificateStack(app, nameof(CertificateStack), new StackProps
            {
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    // Certificates used by CloudFront must be created in us-east-1
                    Region = "us-east-1",
                }
            });
            app.Synth();
        }
    }
}
