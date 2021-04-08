using AzureDemos.QStorage.WritingRecognizer;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AzureDemos.QStorage.HoldOldServer
{ 
    class Program
    {
        
        static async Task Main(string[] args)
        {
            IServiceCollection Services = null;
            IConfigurationRoot Config = null;

           try
            {
                Console.WriteLine("Starting Reservation Service");
                var builder = new HostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        config.AddEnvironmentVariables();
                        config.AddUserSecrets<Program>();
                        config.AddCommandLine(args);
                        Config = config.Build();
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.SetMinimumLevel(LogLevel.Trace);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<WritingRecognizerConfiguration>(
                            Config.GetSection(nameof(WritingRecognizerConfiguration)));

                        services.AddHostedService<WritingRecognizerService>();

                        Services = services;

                    });


                await builder.RunConsoleAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured: {ex.Message}");
            }
            
        }
    }
}
