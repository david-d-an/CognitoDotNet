using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace CognitoDotNet.ApiApp
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
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiApp", Version = "v1" });
            });

            services.AddAuthentication("Bearer")
            // .AddJwtBearer(options => {
            //     options.TokenValidationParameters = GetCognitoTokenValidationParams();
            //     options.Authority = "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_3fV3OuG6q";
            //     options.RequireHttpsMetadata = false;
            // });
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
                options.Authority = "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_3fV3OuG6q";
                options.RequireHttpsMetadata = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiApp v1"));
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            // UseCors must preceed Auth related middlewares
            // app.UseCors(webOrigins);

            // custom jwt auth middleware replaces builtin Auth
            // app.UseMiddleware<JwtMiddleware>();
            // app.UseMiddleware(typeof(JwtMiddleware));

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }

        // private TokenValidationParameters GetCognitoTokenValidationParams() {
        //     var region = Configuration["AWSCognito:Region"];
        //     var poolId = Configuration["AWSCognito:PoolId"];
        //     var appClientId = Configuration["AWSCognito:AppClientId"];
        //     var cognitoIssuer = $"https://cognito-idp.{region}.amazonaws.com/{poolId}";
        //     var jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
        //     var cognitoAudience = appClientId;

        //     return new TokenValidationParameters {
        //         IssuerSigningKeyResolver = (s, tkn, identifier, param) => {
        //             // get JsonWebKeySet from AWS
        //             var json = new WebClient().DownloadString(jwtKeySetUrl);
                    
        //             // serialize the result
        //             var keys = JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                    
        //             // cast the result to be the type expected by IssuerSigningKeyResolver
        //             return (IEnumerable<SecurityKey>)keys;
        //         },
        //         ValidIssuer = cognitoIssuer,
        //         ValidateIssuerSigningKey = true,
        //         ValidateIssuer = true,
        //         ValidateLifetime = true,
        //         ValidAudience = cognitoAudience
        //     };
        // }
    }
}
