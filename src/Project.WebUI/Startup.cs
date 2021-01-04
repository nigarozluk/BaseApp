using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.WebUI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // DATABASE CONFIGURATION

            // SERVICES
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ITempDataProvider, SessionStateTempDataProvider>();

            // LIBRARIES

            // SETTINGS
            services.AddMemoryCache();
            services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromHours(2);
            });
            services.AddResponseCompression(option =>
            {
                option.Providers.Add<GzipCompressionProvider>();
                option.EnableForHttps = true;
            });

            // MVC 
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // EXCEPTION & STATUS PAGES
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();

            // Compression & Static Files
            app.UseResponseCompression();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24 * 365;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            // SESSION
            app.UseSession();

            // ROUTING
            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}"
                   );
            });
        }
    }
}
