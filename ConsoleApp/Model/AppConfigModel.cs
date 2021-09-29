using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Model
{
    public class AppConfigModel
    {
        public string UserPoolId { get; set; }
        public string AppClientId { get; set; }
        public string Region { get; internal set; }
    }
}