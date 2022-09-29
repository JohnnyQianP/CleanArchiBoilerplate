using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchi.Boilerplate.WebApi.Extension.AutofacReg;

public class AutofacControllerModule: Module
{
    protected override void Load(ContainerBuilder builder) 
    {
        var controllerBaseType = typeof(ControllerBase);
        builder.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
            .PropertiesAutowired();
    }
}
