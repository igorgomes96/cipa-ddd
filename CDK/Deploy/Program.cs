using Amazon.CDK;

namespace Deploy
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            new CipaStack(app, nameof(CipaStack), new StackProps
            {
                Env = new Environment
                {
                    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
                }
            });
            app.Synth();
        }
    }
}
