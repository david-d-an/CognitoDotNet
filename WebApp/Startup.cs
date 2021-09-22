using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebAppCognito
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var clientId = Configuration["Authentication:Cognito:ClientId"];
            var clientSecret = Configuration["Authentication:Cognito:ClientSecret"];
            var baseUrl = Configuration["Authentication:Cognito:BaseUrl"];
            var logOutUrl = Configuration["Authentication:Cognito:LogOutUrl"];
            var saveToken = bool.Parse(Configuration["Authentication:Cognito:SaveToken"]);
            var responseType = Configuration["Authentication:Cognito:ResponseType"];
            var metadataAddress = Configuration["Authentication:Cognito:MetadataAddress"];

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.Events.OnSigningIn = FilterGroupClaims;
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect(options => {
                options.ResponseType = responseType;
                options.MetadataAddress = metadataAddress;
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
                options.SaveTokens = saveToken;
                options.GetClaimsFromUserInfoEndpoint = true;
                // options.SignedOutRedirectUri = "https://localhost:15156/Account/LoggedOut/";
                options.Scope.Add("email");
                options.Events = new OpenIdConnectEvents {
                    OnRedirectToIdentityProviderForSignOut = (context) => {
                        var logoutUri = logOutUrl;
                        logoutUri += $"?client_id={clientId}&logout_uri={baseUrl}/Account/LoggedOut/";
                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddControllersWithViews();
            services.AddAuthorization();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { 
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}"); 
            });
        }

        //Remove all the claims that are unrelated to our identity
        private static Task FilterGroupClaims(CookieSigningInContext context) {
            var principal = context.Principal;
            if (principal.Identity is ClaimsIdentity identity) {
                var unused = identity.FindAll(GroupsToRemove).ToList();
                unused.ForEach(c => identity.TryRemoveClaim(c));
            }
            return Task.FromResult(principal);
        }

        private static bool GroupsToRemove(Claim claim) {
            string[] _groupObjectIds = new string[] { "identities" };
            return claim.Type == "groups" && !_groupObjectIds.Contains(claim.Type);
        }
    }
}
