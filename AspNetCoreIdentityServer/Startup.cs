using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreIdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

            //configure identity server with in-memory stores, keys, clients and resources
            services.AddIdentityServer()
                  .AddDeveloperSigningCredential()
                  .AddInMemoryIdentityResources(Config.GetIdentityResources())
                   .AddInMemoryApiResources(Config.GetApiResources())
                  .AddInMemoryClients(Config.GetClients(Configuration.GetServiceUri("MvcAppClient", "https").AbsoluteUri))
                  .AddTestUsers(Config.GetUsers());

            services.AddAuthentication()
                 .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
                 {
                     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                     options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                     options.SaveTokens = true;

                     options.Authority = "https://demo.identityserver.io/";
                     options.ClientId = "interactive.confidential";
                     options.ClientSecret = "secret";
                     options.ResponseType = "code";

                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         NameClaimType = "name",
                         RoleClaimType = "role"
                     };
                 });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();


            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
