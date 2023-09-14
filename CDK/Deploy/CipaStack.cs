using Amazon.CDK;
using Amazon.CDK.AWS.CertificateManager;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Route53;
using Amazon.CDK.AWS.Route53.Targets;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SSM;
using Amazon.CDK.CustomResources;
using Constructs;
using System;
using System.Collections.Generic;

namespace Deploy
{
    public class CipaStack : Stack
    {
        private const string StaticFilesBucketArn = "arn:aws:s3:::cipaonline";
        internal CipaStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // The code that defines your stack goes here
            var vpc = Vpc.FromLookup(this, "vpc", new VpcLookupOptions
            {
                IsDefault = true
            });
            SecurityGroup securityGroup = CreateSecurityGroup(vpc);
            Role role = CreateVmRole();
            Instance_ ec2Instance = CreateVmInstance(vpc, securityGroup, role);

            var bucket = Bucket.FromBucketArn(this, "bucket", StaticFilesBucketArn);
            CreateBucketPolicy(role, bucket);

            IHostedZone hostedZone = HostedZone.FromHostedZoneAttributes(
                this, "hosted-zone",
                new HostedZoneAttributes
                {
                    HostedZoneId = Constants.HostedZoneId,
                    ZoneName = Constants.HostedZoneName
                });
            var cipaDomainName = $"cipa.{Constants.HostedZoneName}";
            string certificateArn = GetCertificateArn();
            ICertificate certificate = Certificate.FromCertificateArn(
                this, "cipa-certificate", certificateArn);

            BehaviorOptions s3Behavior = CreateS3StaticFilesCdnBehavior(bucket);
            HttpOrigin httpOrigin = CreateCdnVmOrigin(ec2Instance);
            BehaviorOptions apiBehavior = CreateApiCdnBehavior(httpOrigin);
            Distribution cdn = CreateCdnDistribution(
                httpOrigin, s3Behavior, apiBehavior, certificate, cipaDomainName);

            CreateRoute53AliasRecord(hostedZone, cdn);

            CreateSSMParameters(cdn);
        }

        /// <summary>
        /// This method creates a custom AWS resource responsible for reading the SSM parameter
        /// that contains the certificate ARN, created in the CertificateStack. 
        /// This is necessary because the CertificateStack was created in another region as
        /// CloudFront requires that the certificate to be created in us-east-1.
        /// Cross-region stack import is not possible.
        /// </summary>
        /// <returns></returns>
        private string GetCertificateArn()
        {
            var ssmCall = new AwsSdkCall
            {
                Service = "SSM",
                Action = "getParameter",
                Parameters = new Dictionary<string, string>
                        {
                            { "Name", Constants.CertificateArnSSMParam }
                        },
                Region = "us-east-1",
                PhysicalResourceId = PhysicalResourceId.Of(Guid.NewGuid().ToString())
            };
            var ssmReader = new AwsCustomResource(
                this,
                "GetCertificateParamValue",
                new AwsCustomResourceProps
                {
                    OnCreate = ssmCall,
                    OnUpdate = ssmCall,
                    Policy = AwsCustomResourcePolicy.FromSdkCalls(new SdkCallsPolicyOptions
                    {
                        Resources = AwsCustomResourcePolicy.ANY_RESOURCE
                    })
                });
            var certificateArn = ssmReader.GetResponseField("Parameter.Value").ToString();
            return certificateArn;
        }

        private void CreateRoute53AliasRecord(
            IHostedZone hostedZone, Distribution cdn)
        {
            _ = new ARecord(this, "cipa-a-record", new ARecordProps
            {
                Target = RecordTarget.FromAlias(new CloudFrontTarget(cdn)),
                Zone = hostedZone,
                RecordName = "cipa"
            });
        }

        private void CreateSSMParameters(Distribution cdn)
        {
            _ = new StringParameter(this, "db-connection-string", new StringParameterProps
            {
                ParameterName = "/Cipa/ConnectionStrings/MySqlConnection",
                StringValue = System.Environment.GetEnvironmentVariable("MySqlConnection")
            });

            _ = new StringParameter(this, "email-username", new StringParameterProps
            {
                ParameterName = "/Cipa/Email/UserName",
                StringValue = System.Environment.GetEnvironmentVariable("EmailUserName")
            });

            _ = new StringParameter(this, "email-password", new StringParameterProps
            {
                ParameterName = "/Cipa/Email/Password",
                StringValue = System.Environment.GetEnvironmentVariable("EmailPassword")
            });

            _ = new StringParameter(this, "jwt-secret", new StringParameterProps
            {
                ParameterName = "/Cipa/TokenConfigurations/Secret",
                StringValue = System.Environment.GetEnvironmentVariable("JwtSecret")
            });

            _ = new StringParameter(this, "fotos-cdn", new StringParameterProps
            {
                ParameterName = "/Cipa/FotosUrlBase",
                StringValue = $"https://{cdn.DistributionDomainName}/"
            });
        }

        private Distribution CreateCdnDistribution(
            HttpOrigin httpOrigin,
            BehaviorOptions s3Behavior,
            BehaviorOptions apiBehavior,
            ICertificate certificate,
            string domainName)
        {
            return new Distribution(this, "cdn", new DistributionProps
            {
                DefaultBehavior = new BehaviorOptions
                {
                    Origin = httpOrigin, // HTML and static files in the VM
                    ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS
                },
                Certificate = certificate,
                DomainNames = new[] { domainName },
                AdditionalBehaviors = new Dictionary<string, IBehaviorOptions>
                {
                    { "/api/*", apiBehavior },
                    { "/fotos/*", s3Behavior },
                    { "/documentos/*", s3Behavior },
                    { "/documentocronograma/*", s3Behavior },
                    { "/importacao/*", s3Behavior }
                }
            });
        }

        private static BehaviorOptions CreateApiCdnBehavior(HttpOrigin httpOrigin)
        {
            return new BehaviorOptions
            {
                Origin = httpOrigin,
                ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS,
                CachePolicy = CachePolicy.CACHING_DISABLED,
                OriginRequestPolicy = OriginRequestPolicy.ALL_VIEWER,
                AllowedMethods = AllowedMethods.ALLOW_ALL,
                ResponseHeadersPolicy = ResponseHeadersPolicy.CORS_ALLOW_ALL_ORIGINS_WITH_PREFLIGHT_AND_SECURITY_HEADERS
            };
        }

        private static HttpOrigin CreateCdnVmOrigin(Instance_ ec2Instance)
        {
            return new HttpOrigin(
                ec2Instance.InstancePublicDnsName,
                new HttpOriginProps
                {
                    ProtocolPolicy = OriginProtocolPolicy.HTTP_ONLY
                });
        }

        private BehaviorOptions CreateS3StaticFilesCdnBehavior(IBucket bucket)
        {
            var originAccessIdentity = new OriginAccessIdentity(this, "oai");
            return new BehaviorOptions
            {
                Origin = new S3Origin(bucket, new S3OriginProps
                {
                    OriginAccessIdentity = originAccessIdentity
                }),
                ViewerProtocolPolicy = ViewerProtocolPolicy.REDIRECT_TO_HTTPS
            };
        }

        private void CreateBucketPolicy(Role role, IBucket bucket)
        {
            var bucketWritePolicy = new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "s3:*" },
                Resources = new[] { bucket.ArnForObjects("*") },
                Principals = new[] { role },
                Effect = Effect.ALLOW
            });
            var bucketReadPolicy = new PolicyStatement(new PolicyStatementProps
            {
                Actions = new[] { "s3:GetObject" },
                Resources = new[] { bucket.ArnForObjects("*") },
                Principals = new[] { new AnyPrincipal() },
                Effect = Effect.ALLOW
            });
            _ = new CfnBucketPolicy(this, "bucket-policy", new CfnBucketPolicyProps
            {
                Bucket = bucket.BucketName,
                PolicyDocument = new PolicyDocument(new PolicyDocumentProps
                {
                    Statements = new[] { bucketWritePolicy, bucketReadPolicy }
                })
            });
        }

        private Instance_ CreateVmInstance(IVpc vpc, SecurityGroup securityGroup, Role role)
        {
            var vm = new Instance_(this, "cipa-server", new InstanceProps
            {
                InstanceName = "cipa-server",
                Vpc = vpc,
                VpcSubnets = new SubnetSelection
                {
                    SubnetType = SubnetType.PUBLIC
                },
                Role = role,
                SecurityGroup = securityGroup,
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE2, InstanceSize.SMALL),
                KeyName = "ProdKeyPair",
                MachineImage = MachineImage.LatestAmazonLinux2023(),
                UserData = BuildUserData()
            });
            _ = new CfnEIP(this, "elastic-ip", new CfnEIPProps
            {
                InstanceId = vm.InstanceId
            });
            return vm;
        }

        private Role CreateVmRole()
        {
            return new Role(this, "cipa-vm-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ec2.amazonaws.com"),
                RoleName = "cipa-vm-role",
                ManagedPolicies = new IManagedPolicy[]
                {
                    ManagedPolicy.FromAwsManagedPolicyName("AmazonS3FullAccess"),
                    ManagedPolicy.FromAwsManagedPolicyName("CloudWatchLogsFullAccess"),
                    ManagedPolicy.FromAwsManagedPolicyName("AmazonSESFullAccess"),
                    ManagedPolicy.FromAwsManagedPolicyName("AmazonSSMReadOnlyAccess")
                }
            });
        }

        private SecurityGroup CreateSecurityGroup(IVpc vpc)
        {
            var securityGroup = new SecurityGroup(this, "cipa-sg", new SecurityGroupProps
            {
                SecurityGroupName = "cipa-vm-sg",
                AllowAllOutbound = true,
                Vpc = vpc
            });
            securityGroup.AddIngressRule(
                Peer.AnyIpv4(),
                Port.Tcp(80),
                "Allow HTTP access from anywhere.");
            return securityGroup;
        }

        private static UserData BuildUserData()
        {
            var userData = UserData.ForLinux();
            userData.AddCommands(
                "yum update -y",
                "yum install nginx -y",
                "rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm  # add the Microsoft package signing key to your list of trusted keys",
                "yum install dotnet-sdk-6.0 -y  # install .net sdk",
                "yum install git -y",
                "mkdir /src",
                "cd /src",
                "git clone https://github.com/igorgomes96/cipa-api.git",
                "cd \"/src/cipa-api/1 - WebApi/Cipa.WebApi\"",
                "export DOTNET_CLI_HOME=/tmp",
                "/usr/bin/dotnet publish --configuration Release /p:CustomTransformFileName=aws.transform -o /var/www/cipa",
                "chmod -R 777 /var/www/cipa/Assets",
                "mkdir /var/www/cipa/StaticFiles",
                "chmod -R 777 /var/www/cipa/StaticFiles",
                "rm -rf /etc/systemd/system/cipa.service ",
                "touch /etc/systemd/system/cipa.service # cria o arquivo de definição de serviço",
                "chmod -R 777 /etc/systemd/system/cipa.service",
                "echo \"[Unit]\" >> /etc/systemd/system/cipa.service",
                "echo \"Description=Servico da CIPA\" >> /etc/systemd/system/cipa.service",
                "echo \" \" >> /etc/systemd/system/cipa.service",
                "echo \"[Service]\" >> /etc/systemd/system/cipa.service",
                "echo \"WorkingDirectory=/var/www/cipa\" >> /etc/systemd/system/cipa.service",
                "echo \"ExecStart=/usr/bin/dotnet /var/www/cipa/Cipa.WebApi.dll\" >> /etc/systemd/system/cipa.service",
                "echo \"Restart=always\" >> /etc/systemd/system/cipa.service",
                "echo \"# Restart service after 10 seconds if the dotnet service crashes:\" >> /etc/systemd/system/cipa.service",
                "echo \"RestartSec=10\" >> /etc/systemd/system/cipa.service",
                "echo \"KillSignal=SIGINT\" >> /etc/systemd/system/cipa.service",
                "echo \"SyslogIdentifier=cipa\" >> /etc/systemd/system/cipa.service",
                "echo \"Environment=ASPNETCORE_ENVIRONMENT=Production\" >> /etc/systemd/system/cipa.service",
                "echo \"Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false\" >> /etc/systemd/system/cipa.service",
                "echo \" \" >> /etc/systemd/system/cipa.service",
                "echo \"# How many seconds to wait for the app to shut down after it receives the initial interrupt signal. \" >> /etc/systemd/system/cipa.service",
                "echo \"# If the app doesn't shut down in this period, SIGKILL is issued to terminate the app. \" >> /etc/systemd/system/cipa.service",
                "echo \"# The default timeout for most distributions is 90 seconds.\" >> /etc/systemd/system/cipa.service",
                "echo \"TimeoutStopSec=90\" >> /etc/systemd/system/cipa.service",
                "echo \" \" >> /etc/systemd/system/cipa.service",
                "echo \"[Install]\" >> /etc/systemd/system/cipa.service",
                "echo \"WantedBy=multi-user.target\" >> /etc/systemd/system/cipa.service",
                "systemctl enable cipa.service # habilita inicialização automática",
                "systemctl start cipa",
                "chmod -R 777 /etc/nginx/nginx.conf",
                "echo \"user nginx;\" > /etc/nginx/nginx.conf",
                "echo \"worker_processes auto;\" >> /etc/nginx/nginx.conf",
                "echo \"error_log /var/log/nginx/error.log notice;\" >> /etc/nginx/nginx.conf",
                "echo \"pid /run/nginx.pid;\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"# Load dynamic modules. See /usr/share/doc/nginx/README.dynamic.\" >> /etc/nginx/nginx.conf",
                "echo \"include /usr/share/nginx/modules/*.conf;\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"events {\" >> /etc/nginx/nginx.conf",
                "echo \"    worker_connections 1024;\" >> /etc/nginx/nginx.conf",
                "echo \"}\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"http {\" >> /etc/nginx/nginx.conf",
                "echo \"    log_format  main  '\\$remote_addr - \\$remote_user [\\$time_local] \"\\$request\" '\" >> /etc/nginx/nginx.conf",
                "echo \"                      '\\$status \\$body_bytes_sent \"\\$http_referer\" '\" >> /etc/nginx/nginx.conf",
                "echo \"                      '\"\\$http_user_agent\" \"\\$http_x_forwarded_for\"';\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"    access_log  /var/log/nginx/access.log  main;\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"    sendfile            on;\" >> /etc/nginx/nginx.conf",
                "echo \"    tcp_nopush          on;\" >> /etc/nginx/nginx.conf",
                "echo \"    keepalive_timeout   65;\" >> /etc/nginx/nginx.conf",
                "echo \"    types_hash_max_size 4096;\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"    include             /etc/nginx/mime.types;\" >> /etc/nginx/nginx.conf",
                "echo \"    default_type        application/octet-stream;\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"    include /etc/nginx/conf.d/*.conf;\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"    server {\" >> /etc/nginx/nginx.conf",
                "echo \"        listen       80;\" >> /etc/nginx/nginx.conf",
                "echo \"        listen       [::]:80;\" >> /etc/nginx/nginx.conf",
                "echo \"        server_name  [cipa.4uptech.com.br];\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"        location / {\" >> /etc/nginx/nginx.conf",
                "echo \"            root         /var/www/cipa/wwwroot/;\" >> /etc/nginx/nginx.conf",
                "echo \"            index        index.html;\" >> /etc/nginx/nginx.conf",
                "echo \"            try_files \\$uri \\$uri/ /index.html;\" >> /etc/nginx/nginx.conf",
                "echo \"        }\" >> /etc/nginx/nginx.conf",
                "echo \"\" >> /etc/nginx/nginx.conf",
                "echo \"        location /api/ {\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_pass         http://localhost:5000;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_http_version 1.1;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_set_header   Upgrade \\$http_upgrade;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_set_header   Connection keep-alive;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_set_header   Host \\$host;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_cache_bypass \\$http_upgrade;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_set_header   X-Forwarded-For \\$proxy_add_x_forwarded_for;\" >> /etc/nginx/nginx.conf",
                "echo \"            proxy_set_header   X-Forwarded-Proto \\$scheme;\" >> /etc/nginx/nginx.conf",
                "echo \"        }\" >> /etc/nginx/nginx.conf",
                "echo \"    } \" >> /etc/nginx/nginx.conf",
                "echo \"}\" >> /etc/nginx/nginx.conf",
                "systemctl restart nginx");
            return userData;
        }
    }
}
