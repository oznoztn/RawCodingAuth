using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RawCodingAuth.Basics.Auth.CustomAuthorizationRequirements;

namespace RawCodingAuth.Basics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "Grandmas.Cookie";
                    config.LoginPath = "/Home/Index";
                });

            /* AUTHORIZATION POLICY
             *
             * Authorization, Authorization Requirement'lar�n sa�lanmas�yla olu�an bir olgudur.
             * AuthorizationRequirement'lar AuthorizationHandler s�n�flar� taraf�ndan i�lenir edilir.
             *
             * T�m bunlar Authorization Policy kavram�n� olu�tururlar.
             *
             * .NET TARAFINDAK� KAR�ILIKLAR
             * Authorization Requirement => IAuthorizationRequirement
             * Authorization Handler     => AuthorizationHandler<IAuthorizationRequirement_Implementation>
             */

            services.AddAuthorization(config =>
            {
                // A��klamada yazana g�re varsay�lan poli�e authenticated user ister, ba�ka bir �ey de�il. 
                // Varsay�lan poli�eye config.DefaultPolicy prop'u ile eri�ebilirsin.

                // Biz �imdi default policy'yi kendimiz olu�turup config.DefaultPolicy'ye set edece�iz.

                var authorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                var authorizationPolicy = 
                    authorizationPolicyBuilder
                            // ��te varsay�lan policy'nin gerektirdi�i tek �ey bu
                            // Default Policy'yi default policy yapan tek gereksinim.
                            // Bunu da eklemezsen "en az bir tane auth requirement'� eklemen gerek" diye hata al�rs�n.
                        .RequireAuthenticatedUser() 
                            // Bu Authorization poli�esi belirli bir CLAIM tipini zorunlu k�l�yor
                            //      Bu arada, "Role" Microsoft'un tan�mlad��� CUSTOM bir CLAIM.
                            //      �rne�in name, email gibi standart claim'lerden biri de�il.
                            .RequireClaim(ClaimTypes.Role, "admin")
                            // Bir CLAIM'in daha varl���n� �art ko�uyor:
                            //      Kullan�c�n�n authorized olabilmesi i�in.
                            .RequireClaim(ClaimTypes.Email, "ozan@ozten.com")

                            // �stteki RequireClaim'in taklidi olan olu�turdu�um custom requirement'� kullan�yorum
                            // UserPrincipal.Claims i�erisindeki "secretGarden:level" claiminin varl���n� kontrol ediyor
                            .AddRequirements(new CustomRequireClaimRequirement("secretGarden:level"))
                        .Build();

                // kendi olu�turdu�umuz �eyi set ediyoruz:
                config.DefaultPolicy = authorizationPolicy;
            });

            // custom handler registeration:
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimRequirementHandler>();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseAuthentication();

            app.UseRouting();
            
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
