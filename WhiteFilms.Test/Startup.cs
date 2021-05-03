using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WhiteFilms.API.Controllers;
using WhiteFilms.API.Models;
using WhiteFilms.API.Services;

namespace WhiteFilms.Test
{
    public class Startup
    {
        public Startup()
        {
            /* 构造configuration */
            var environmentName = Environment.GetEnvironmentVariable("Hosting:Environment");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .AddEnvironmentVariables()
                .Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<WhiteFilmsDatabaseSettings>(
                Configuration.GetSection(nameof(WhiteFilmsDatabaseSettings)));
            services.AddSingleton<WhiteFilmsDatabaseSettings>(provider =>
                provider.GetRequiredService<IOptions<WhiteFilmsDatabaseSettings>>().Value);

            services.AddSingleton<PasswordsService>();
            services.AddSingleton<AccountsService>();
            services.AddSingleton<TokensService>();
            services.AddSingleton<FilmsService>();
            services.AddSingleton<AccountsController>();
        }
    }
}