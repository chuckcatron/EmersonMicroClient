using clientProcessing.DAL;
using clientProcessing.Helpers;
using clientProcessing.Interfaces;
using clientProcessing.MessageCenter;
using clientProcessing.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace clientProcessing
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            ClientProcessing app = serviceProvider.GetService<ClientProcessing>();
            // Start up logic here
            app?.Run();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services
                .AddLogging(configure =>
                {
                    configure.AddConsole();
                    configure.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Critical);
                })
                .AddTransient<ClientProcessing>()
                .AddDbContext<centraldbContext>()
                .AddSingleton<ISubSocket, SubSocket>()
                .AddSingleton<IScreenHelper, ScreenHelper>()
                .AddSingleton<IFlowRouting, FlowRouting>()
                .AddSingleton<IChannelRepository, ChannelRepository>()
                .BuildServiceProvider();
        }
    }
}