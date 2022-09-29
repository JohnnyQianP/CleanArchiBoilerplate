﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchi.Boilerplate.Application.AutoMapper;

/// <summary>
/// Automapper 启动服务
/// </summary>
public static class AutoMapperSetup
{
    public static void AddAutoMapperSetup(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddAutoMapper(typeof(AutoMapperConfig));
        AutoMapperConfig.RegisterMappings();
    }
}
