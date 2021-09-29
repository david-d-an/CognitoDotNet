using System;
using System.IO;
using System.Threading.Tasks;
using ConsoleApp.Model;
using ConsoleApp.Repositories;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false);

            IConfiguration config = builder.Build();
            var region = config.GetSection("AppConfig:region")?.Value;
            var userPoolId = config.GetSection("AppConfig:UserPoolId")?.Value;
            var appClientId = config.GetSection("AppConfig:AppClientId")?.Value;
            // Console.WriteLine($"The answer is always {myFirstClass.Option2}");

            var userLogin = new UserLoginModel {
                username = "superdavid73@hotmail.com",
                password = "Soil9303"
            };
            var appConfig = new AppConfigModel {
                Region = region,
                UserPoolId = userPoolId,
                AppClientId = appClientId
            };

            await new UserRepository().TryLoginAsync(userLogin, appConfig);
        }
    }
}
