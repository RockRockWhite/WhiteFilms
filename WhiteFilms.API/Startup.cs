using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WhiteFilms.API.Models;
using WhiteFilms.API.Services;

namespace WhiteFilms.API
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
            // 用json配置服务器
            services.Configure<WhiteFilmsDatabaseSettings>(Configuration.GetSection(nameof(WhiteFilmsDatabaseSettings)));
            services.AddSingleton<WhiteFilmsDatabaseSettings>(provider =>
                provider.GetRequiredService<IOptions<WhiteFilmsDatabaseSettings>>().Value);
            services.AddSingleton<PasswordsService>();
            services.AddSingleton<AccountsService>();
            services.AddSingleton<TokensService>();
            services.AddSingleton<FilmsService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WhiteFilms.API", Version = "v1"});
            }).AddSwaggerGenNewtonsoftSupport();

            // TODO 使用转换
            services.AddControllers().AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WhiteFilms.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}