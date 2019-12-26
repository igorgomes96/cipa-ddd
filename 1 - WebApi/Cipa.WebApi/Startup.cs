using AutoMapper;
using Cipa.Application;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Cipa.WebApi.BackgroundTasks.Scheduler;

namespace Cipa.WebApi
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<CipaContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseMySql(Configuration.GetConnectionString("MySqlConnection"),
                    b => b.MigrationsAssembly("Cipa.WebApi"));
            });

            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton<TokenConfigurations>(tokenConfigurations);

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

            services.AddAuthorization(auth => {
                auth.AddPolicy(PoliticasAutorizacao.UsuarioSESMT, AuthorizationPolicies.UsuarioSESMTAuthorizationPolicy);
                auth.AddPolicy(PoliticasAutorizacao.UsuarioSESMTContaValida, AuthorizationPolicies.UsuarioSESMTPossuiContaValidaAuthorizationPolicy);
            });

            // AutoMapper
            services.AddSingleton(AutoMapperConfig.MapperConfig());

            // Dependências
            AddDependencies(services);

            services.AddCors(options => options.AddPolicy("AllowAll", builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200")));


            // services.AddHostedService<ImportacaoHostedService>();
            // services.AddHostedService<EmailHostedService>();
            // services.AddHostedService<ProcesssamentoEtapasHostedService>();
            services.AddHostedService<AlteracaoEtapaScheduler>();

            services.AddResponseCompression();

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IHubContext<ProgressHub> hubContext,
            IProgressoImportacaoEvent notificacaoProgressoEvent,
            IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpErrorMiddleware();
            app.UseCors("AllowAll");
            app.UseSignalR(routes =>
            {
                routes.MapHub<ProgressHub>("/api/signalr");
            });
            app.UseAuthentication();
            app.UseResponseCompression();

            notificacaoProgressoEvent.NotificacaoProgresso += (object sender, ProgressoImportacaoEventArgs args) =>
            {
                hubContext.Clients.User(args.EmailUsuario).SendAsync("progressoimportacao", args);
            };

            notificacaoProgressoEvent.ImportacaoFinalizada += (object sender, FinalizacaoImportacaoStatusEventArgs args) =>
            {
                hubContext.Clients.User(args.EmailUsuario).SendAsync("importacaofinalizada", mapper.Map<FinalizacaoImportacaoStatusViewModel>(args));
            };

            // app.UseHttpsRedirection();
            app.UseMvc();
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
