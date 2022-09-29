using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CleanArchi.Boilerplate.Infrastructure.MiddlewarePipe;

/// <summary>
/// MiniProfiler性能分析
/// </summary>
public static class MiniProfilerMiddleware
{
    //private static readonly ILog Log = LogManager.GetLogger(typeof(MiniProfilerMiddleware));
    public static void UseMiniProfilerMiddleware(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        try
        {
            //miniprofile 路径/profiler/results-index
            bool miniProfilerEnabled = configuration.GetValue<bool>("MiddlewareFlag:EnableMiniProfiler");
            if (miniProfilerEnabled)
            {
                // 性能分析
                app.UseMiniProfiler();
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error was reported when starting the MiniProfilerMildd.\n{e.Message}");
            throw e;
        }
    }
}
