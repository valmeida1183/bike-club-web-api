using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeClub.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BikeClub
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
            // Configura o Brotli e compacta todo o response que for do mimeType "application/json"
            services.AddResponseCompression(opt =>
            {
                opt.EnableForHttps = true;
                opt.Providers.Add<BrotliCompressionProvider>();
                opt.MimeTypes = ResponseCompressionDefaults.MimeTypes
                    .Concat(new[] { "application/json; charset=utf-8"});
            });            

            services.AddControllers();
            var key = Encoding.ASCII.GetBytes(Settings.Secret);
            services.AddAuthentication(a => 
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(j => 
            {
                j.RequireHttpsMetadata = false;
                j.SaveToken = true;
                j.TokenValidationParameters = new TokenValidationParameters 
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };        
            });

            // Config do EF com database em memória
            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("MyMemoryDatabase"));

            //Config do EF com databse real
            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("connectionString")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression();

            // Condig para permitir a WEb api servir arquivos estáticos (ex: imagens) em uma pasta diferente de "wwwroot".
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Resources")),
                RequestPath = "/Resources"
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
