using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace CleanArchi.Boilerplate.JobTask;

public static class JobTaskMiddleware
{
    public static void UseJobTaskMiddleware(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        try
        {
            var jobEnabled = bool.Parse(configuration.GetSection("Quartz")["Enabled"]);
            if (jobEnabled)
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path.Value.ToLower().StartsWith("/jobui"))
                    {
                        context.Request.Path = "/index.html";
                        await next();
                    }
                    else
                    {
                        await next();
                    }

                });
                //app.UseStaticFiles();
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetExecutingAssembly(), "wwwroot")
                });
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
