using AutoMapper;
using Cipa.Application.Events;
using Cipa.Application.Events.EventsArgs;
using Cipa.Application.Interfaces;
using Cipa.Application.Services.Implementation;
using Cipa.Application.Services.Interfaces;
using Cipa.Domain.Factories;
using Cipa.Domain.Factories.Interfaces;
using Cipa.Domain.Helpers;
using Cipa.Application.Repositories;
using Cipa.Infra.Data.Context;
using Cipa.Infra.Data.Repositories;
using Cipa.WebApi.Authentication;
using Cipa.WebApi.AutoMapper;
using Cipa.WebApi.BackgroundTasks;
using Cipa.WebApi.Configurations;
using Cipa.WebApi.Middleware;
using Cipa.WebApi.SignalR;
using Cipa.WebApi.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Rewrite;
using Amazon.SimpleEmail;
using Cipa.Domain.Services.Interfaces;
using Cipa.Infra.Data.Utils;
using Cipa.Application.Implementation;

namespace Cipa.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAWSServices(Configuration);
            services.AddAWSService<IAmazonSimpleEmailService>();
            
            services.AddDbContext<CipaContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.LogTo(Console.WriteLine);
                var connectionString = Configuration.GetConnectionString("MySqlConnection");
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly("Cipa.WebApi"));
            });

            var signingConfigurations = new SigningConfigurations(Configuration.GetSection("TokenConfigurations:Secret").Value);
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            var importacaoConfiguration = new ImportacaoServiceConfiguration();
            new ConfigureFromConfigurationOptions<ImportacaoServiceConfiguration>(
                Configuration.GetSection("Importacao"))
                    .Configure(importacaoConfiguration);
            services.AddSingleton<IImportacaoServiceConfiguration>(importacaoConfiguration);

            var emailConfiguration = new EmailConfiguration();
            new ConfigureFromConfigurationOptions<EmailConfiguration>(
                Configuration.GetSection("Email"))
                    .Configure(emailConfiguration);
            services.AddSingleton(emailConfiguration);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => JwtBeareOptionsConfig.JwtConfiguration(options, signingConfigurations, tokenConfigurations));

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(PoliticasAutorizacao.UsuarioSESMT, AuthorizationPolicies.UsuarioSESMTAuthorizationPolicy);
                auth.AddPolicy(PoliticasAutorizacao.UsuarioSESMTContaValida, AuthorizationPolicies.UsuarioSESMTPossuiContaValidaAuthorizationPolicy);
            });

            // AutoMapper
            services.AddSingleton(AutoMapperConfig.MapperConfig(Configuration["FotosUrlBase"]));

            // Dependências
            AddDependencies(services);

            services.AddCors(options => options.AddPolicy("AllowAll", builder => builder
                .WithMethods("DELETE", "GET", "HEAD", "OPTIONS", "PATCH", "POST", "PUT ")
                .WithHeaders("Content-Type", "X-Amz-Date", "Authorization", "X-Api-Key", "X-Amz-Security-Token", "X-Requested-With")
                .AllowCredentials()
                .WithOrigins("http://localhost:4200", "https://cipa.4uptech.com.br")));

            
            // services.AddHostedService<ImportacaoHostedService>();
            // services.AddHostedService<EmailHostedService>();
            // services.AddHostedService<ProcesssamentoEtapasHostedService>();
            // services.AddHostedService<AlteracaoEtapaService>();

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromSeconds(63072000);
            });

            services.AddResponseCompression(options =>
            {
                IEnumerable<string> MimeTypes = new[]
                {
                    // General
                    "text/plain",
                    "text/html",
                    "text/css",
                    "font/woff2",
                    "application/javascript",
                    "image/x-icon",
                    "image/png"
                 };

                options.EnableForHttps = true;
                options.MimeTypes = MimeTypes;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
                loggingBuilder.AddAWSProvider(Configuration.GetAWSLoggingConfigSection());
            });
            services.AddRouting();
            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHubContext<ProgressHub> hubContext,
            IProgressoImportacaoEvent notificacaoProgressoEvent,
            IMapper mapper)
        {

            app.UseForwardedHeaders();
            app.UseCors("AllowAll");

            app.UseHttpErrorMiddleware();

            // app.UseHsts();

            notificacaoProgressoEvent.NotificacaoProgresso += (object sender, ProgressoImportacaoEventArgs args) =>
            {
                hubContext.Clients.User(args.EmailUsuario).SendAsync("progressoimportacao", args);
            };

            notificacaoProgressoEvent.ImportacaoFinalizada += (object sender, FinalizacaoImportacaoStatusEventArgs args) =>
            {
                hubContext.Clients.User(args.EmailUsuario).SendAsync("importacaofinalizada", mapper.Map<FinalizacaoImportacaoStatusViewModel>(args));
            };

            var options = new RewriteOptions().AddRewrite(@"^((?!.*?\b(web$.*|health.*|api\/.*)))((\w+))*\/?(\.\w{{5,}})?\??([^.]+)?$", "index.html", false);
            app.UseRewriter(options);

            app.UseResponseCompression();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ProgressHub>("/api/signalr");
            });
        }

        private void AddDependencies(IServiceCollection services)
        {
            // Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Autenticacão
            services.AddScoped<ILoginService, LoginService>();

            // Eleição
            services.AddScoped<IEleicaoAppService, EleicaoAppService>();
            services.AddScoped<IEleicaoRepository, EleicaoRepository>();

            // Grupo
            services.AddScoped<IGrupoAppService, GrupoAppService>();
            services.AddScoped<IGrupoRepository, GrupoRepository>();

            // Empresa
            services.AddScoped<IEmpresaAppService, EmpresaAppService>();
            services.AddScoped<IEmpresaRepository, EmpresaRepository>();

            // Estabelecimento
            services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();
            services.AddScoped<IEstabelecimentoAppService, EstabelecimentoAppService>();

            // Conta
            services.AddScoped<IContaRepository, ContaRepository>();
            services.AddScoped<IContaAppService, ContaAppService>();

            // Grupo
            services.AddScoped<IGrupoRepository, GrupoRepository>();

            // Usuario
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            // Excel
            services.AddScoped<IExcelService, ExcelService>();

            // Email
            services.AddScoped<IEmailSender, EmailSender>();

            // Importações
            services.AddScoped<IImportacaoAppService, ImportacaoAppService>();
            services.AddScoped<IImportacaoRepository, ImportacaoRepository>();

            // Arquivos
            services.AddScoped<IArquivoAppService, ArquivoAppService>();
            services.AddScoped<IArquivoRepository, ArquivoRepository>();

            // Emails
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IEmailAppService, EmailAppService>();
            services.AddSingleton<IFormatadorEmailServiceFactory, FormatadorEmailServiceFactory>();

            // Processamento de Etapas
            services.AddScoped<IProcessamentoEtapaRepository, ProcessamentoEtapaRepository>();
            services.AddScoped<IProcessamentoEtapaAppService, ProcessamentoEtapaAppService>();

            // Notificação de Progresso
            services.AddSingleton<IProgressoImportacaoEvent, ProgressoImportacaoEvent>();
        }

    }
}
