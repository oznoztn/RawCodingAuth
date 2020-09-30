using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raw.IdentityServer.Constants;

namespace Raw.IdentityServer.Mvc
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Raw.IdentityServer.Mvc.Cookie";
                    options.DefaultSignInScheme = "Raw.IdentityServer.Mvc.Cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Raw.IdentityServer.Mvc.Cookie", options =>
                {
                    options.Cookie.Name = "Raw.IdentityServer.Mvc.Cookie";
                })
                .AddOpenIdConnect("oidc", config =>
                {
                    // Alttaki 1. nota bak.
                    config.Authority = RawApplicationUrl.IdentityServer;
                    config.ClientId = RawClientId.Mvc;
                    config.ClientSecret = RawClientSecret.Mvc;
                    config.ResponseType = "code";
                    config.SaveTokens = true;
                    
                    // "secret" scope'unu istiyoruz.
                    // IS taraf�nda yetkilendirme tan�mlanmam��sa hata al�rs�n.
                    config.Scope.Add("secret");
                    config.Scope.Add("area51");
                });

            services.AddHttpClient();

            services
                .AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}

/*
 *      NOT 1) 
 * �nceki basit bir OAuth implementasyon �rne�i olan,
 * OAuthBasics.Client'deki AddOAuth'da belirtti�imiz endpoint'leri burada belirtmemize gerek yok.
 * AddOpenIdConnect servisi o bilgilere Discovery Dok�man� ile vak�f zaten.
 *
 */
