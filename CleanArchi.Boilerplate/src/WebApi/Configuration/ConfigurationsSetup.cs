namespace CleanArchi.Boilerplate.WebApi.Configuration;

public static class ConfigurationsSetup
{
    public static ConfigureHostBuilder AddConfigurations(this ConfigureHostBuilder host) 
    {
        host.ConfigureAppConfiguration((context, config) =>
        {
            var env = context.HostingEnvironment;
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration/logger.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/logger.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration/database.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/database.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration/setupflag.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/setupflag.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration/cors.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/cors.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration/security.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/security.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"Configuration/quartz.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Configuration/quartz.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                ;

        });
        return host;
    }
}
