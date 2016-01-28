using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using VN.Task.Manager.config;
using VN.DependencyInjection.Extensions;
using VN.DependencyInjection;

namespace VN.Task.Manager
{
    public class Startup
    {
        private static IConfigurationRoot _configuration;
        private static IServiceProvider _provider;
        private readonly IApplicationEnvironment _environment;

        public Startup(IApplicationEnvironment env)
        {
            _environment = env;
        }

        public static TaskManagerConfigurationSettings ConfigurationSettings { get; private set; }

        public void Configure(IConfigurationBuilder configurationBuilder, string[] args)
        {

            var switchMappings = new Dictionary<string, string>
            {
                {"--text", "showText" },
                {"-t", "showText" },
            };

            configurationBuilder
                .SetBasePath(_environment.ApplicationBasePath)
                .AddJsonFile($@"config\development.json");

            //determine if argument is in name/value pair
            if(false) configurationBuilder.AddCommandLine(args, switchMappings);


            _configuration = configurationBuilder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<TaskManagerConfigurationSettings>(_configuration.GetSection(string.Empty));

            ConfigurationSettings = _configuration.Get<TaskManagerConfigurationSettings>();

            services.AddApplicationInsightsTelemetry(_configuration);
            services.AddInstance<IAssemblyLoadContext>(PlatformServices.Default.AssemblyLoadContextAccessor.Default);
            services.AddInstance<ILibraryManager>(PlatformServices.Default.LibraryManager);
            services.AddTransient<IConfigureOptions<RegisterDependencyTypeOptions>, RegisterDependencyTypeOptionsSetup>();
            services.Configure<RegisterDependencyTypeOptions>(options =>
            {
                options.AssemblyPathLocation = ConfigurationSettings.ExternalAssemblyPath;
                options.InjectFromInterfaceName = ConfigurationSettings.InterfaceType;
                options.TaskManifest = ConfigurationSettings.TaskManifest;
            });

            //Custom Application Services
            services.AddDependencyScanner()
                .AddDependencyScan()
                .AddDependencyScanFromAllAssemblies();

            _provider = services.BuildServiceProvider();
            
        }

        public T GetService<T>()
        {
            return _provider.GetRequiredService<T>();
        }

        public IEnumerable<T> GetServices<T>()
        {
            return _provider.GetServices<T>();
        }
    }
}
