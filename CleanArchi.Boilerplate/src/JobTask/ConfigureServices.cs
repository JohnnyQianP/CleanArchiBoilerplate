using CleanArchi.Boilerplate.JobTask.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Spi;

namespace CleanArchi.Boilerplate.JobTask;
public static class ConfigureServices
{
    public static IServiceCollection AddJobTaskServices(this IServiceCollection services, IConfiguration configuration) 
    {
        string dbProviderName = configuration.GetSection("Quartz")["dbProviderName"];
        string connectionString = configuration.GetSection("Quartz")["connectionString"];

        ScheduleManager schedulerCenter = ScheduleManager.getInstance(new DbProvider(dbProviderName, connectionString), services.BuildServiceProvider());
        services.AddSingleton(schedulerCenter);
        //services.AddSingleton<HttpJob>();
        return services;
    }


}