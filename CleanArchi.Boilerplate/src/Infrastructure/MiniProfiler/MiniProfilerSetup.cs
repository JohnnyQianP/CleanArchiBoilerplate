using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchi.Boilerplate.Infrastructure.MiniProfiler;

/// <summary>
/// MiniProfiler 启动服务
/// </summary>
public static class MiniProfilerSetup
{
    public static void AddMiniProfilerSetup(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        //miniprofile 路径/profiler/results-index
        bool miniProfilerEnabled = configuration.GetValue<bool>("MiddlewareFlag:EnableMiniProfiler");
        if (miniProfilerEnabled)
        {
            services.AddMiniProfiler(option => {
                option.RouteBasePath = "/profiler";
            });
        }
    }
}
