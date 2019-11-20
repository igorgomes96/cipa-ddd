using System;
using System.Threading.Tasks;
using Cipa.Application;
using Cipa.Application.Interfaces;
using Cipa.Domain.Interfaces.Repositories;
using Cipa.Domain.Interfaces.Services;
using Cipa.Domain.Services;
using Cipa.Infra.Data.Context;
using Cipa.Infra.Data.Repositories;
using Cipa.WebApi.Authentication;
using Cipa.WebApi.AutoMapper;
using Cipa.WebApi.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

            // services.AddDbContext<CipaContext>(opt =>
            // {
            //    opt.UseInMemoryDatabase("cipa");
            // });
            services.AddDbContext<CipaContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("MySqlConnection"),
                    b => b.MigrationsAssembly("Cipa.WebApi"));
            });


            var signingConfigurations = new SigningConfigurations();
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations();
            new ConfigureFromConfigurationOptions<TokenConfigurations>(
                Configuration.GetSection("TokenConfigurations"))
                    .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => JwtBeareOptionsConfig.JwtConfiguration(options, signingConfigurations, tokenConfigurations));


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


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseAuthentication();

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
            services.AddScoped<IEleicaoService, EleicaoService>();
            services.AddScoped<IEleicaoRepository, EleicaoRepository>();

            // Estabelecimento
            services.AddScoped<IEstabelecimentoService, EstabelecimentoService>();
            services.AddScoped<IEstabelecimentoRepository, EstabelecimentoRepository>();

            // Conta
            services.AddScoped<IContaService, ContaService>();
            services.AddScoped<IContaRepository, ContaRepository>();

            // Grupo
            services.AddScoped<IGrupoService, GrupoService>();
            services.AddScoped<IGrupoRepository, GrupoRepository>();

            // Usuario
            services.AddScoped<IUsuarioAppService, UsuarioAppService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }

    }
}
