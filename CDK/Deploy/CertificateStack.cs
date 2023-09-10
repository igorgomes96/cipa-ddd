using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace Deploy;
internal class CertificateStack : Stack
{
    internal CertificateStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
    {
        IHostedZone hostedZone = HostedZone.FromHostedZoneAttributes(
                this, "hosted-zone",
                new HostedZoneAttributes
                {
                    HostedZoneId = Constants.HostedZoneId,
                    ZoneName = Constants.HostedZoneName
                });
        var cipaDomainName = $"cipa.{Constants.HostedZoneName}";
        var certificate = GenerateAcmCertificate(hostedZone, cipaDomainName);

        _ = new StringParameter(this, "db-connection-string", new StringParameterProps
        {
            ParameterName = Constants.CertificateArnSSMParam,
            StringValue = certificate.CertificateArn
        });
    }

    private Certificate GenerateAcmCertificate(IHostedZone hostedZone, string domainName)
    {
        return new Certificate(this, "cipa-domain-certificate", new CertificateProps
        {
            DomainName = domainName,
            CertificateName = "Cipa certificate",
            Validation = CertificateValidation.FromDns(hostedZone)
        });
    }
}
