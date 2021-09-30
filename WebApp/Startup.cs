using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// using System.Collections.Generic;
// using Microsoft.AspNetCore.Http;

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
            var logOutUrl = Configuration["Authentication:Cognito:LogOutUrl"];
            // loggedOutUrl must match Sign out URL of Cognito App Client
            var loggedOutUrl = Configuration["Authentication:Cognito:LoggedOutUrl"];
            var saveToken = bool.Parse(Configuration["Authentication:Cognito:SaveToken"]);
            var responseType = Configuration["Authentication:Cognito:ResponseType"];
            var metadataAddress = Configuration["Authentication:Cognito:MetadataAddress"];
            // Client Secret and BaseUrl no longer used
            // var clientSecret = Configuration["Authentication:Cognito:ClientSecret"];
            // var baseUrl = Configuration["Authentication:Cognito:BaseUrl"];

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                // options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
                options.Events.OnSigningIn = FilterGroupClaims;
                options.SlidingExpiration = true;
            })
            .AddOpenIdConnect(options => {
                options.ResponseType = responseType;
                options.MetadataAddress = metadataAddress;
                options.ClientId = clientId;
                options.Events = new OpenIdConnectEvents {
                    OnRedirectToIdentityProviderForSignOut = (context) => {
                        var logoutUri = logOutUrl;
                        logoutUri += $"?client_id={clientId}&logout_uri={loggedOutUrl}";
                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
                // options.SaveTokens = saveToken;
                // options.ClientSecret = clientSecret;
                // options.GetClaimsFromUserInfoEndpoint = true;
                // options.SignedOutRedirectUri = "https://www.google.com";
                // options.Scope.Add("email");
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
                app.UseExceptionHandler("/Home/Error");
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
