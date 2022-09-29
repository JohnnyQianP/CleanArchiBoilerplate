using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.DynamicProxy;
using CleanArchi.Boilerplate.Application.Common.Interfaces;
using CleanArchi.Boilerplate.Infrastructure.AOP;
using CleanArchi.Boilerplate.Repository.Base;
using CleanArchi.Boilerplate.Repository.Uow;
using Microsoft.AspNetCore.Mvc;
using Assembly = System.Reflection.Assembly;

namespace CleanArchi.Boilerplate.Infrastructure.AutofacReg;

public class AutofacModule: Autofac.Module
{
    protected override void Load(ContainerBuilder builder) 
    {
        var basePath = AppContext.BaseDirectory;

        var cacheType = new List<Type>();
        //appsettings todo
        bool tranAOPEnabled = true;
        if (tranAOPEnabled)
        {
            builder.RegisterType<TransAOPInterceptor>();
            cacheType.Add(typeof(TransAOPInterceptor));
        }

        var applicationAss = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("Application"));
        var infraAss = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("Infrastructure"));
        var asses = applicationAss.Concat(infraAss);
        //builder.RegisterAssemblyTypes(assemblysRepository)
        //    .AsImplementedInterfaces()
        //    .PropertiesAutowired()
        //    .InstancePerDependency();

        builder.RegisterAssemblyTypes(asses.ToArray())
                .Where(t => typeof(ITransientService).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .AsImplementedInterfaces().InstancePerDependency()
                .PropertiesAutowired()
                .EnableInterfaceInterceptors()       //引用Autofac.Extras.DynamicProxy;
                .InterceptedBy(cacheType.ToArray()); //允许将拦截器服务的列表分配给注册。;

        builder.RegisterAssemblyTypes(asses.ToArray())
                .Where(t => typeof(IScopedService).GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .AsImplementedInterfaces().InstancePerLifetimeScope()
                .PropertiesAutowired()
                .EnableInterfaceInterceptors()       //引用Autofac.Extras.DynamicProxy;
                .InterceptedBy(cacheType.ToArray()); //允许将拦截器服务的列表分配给注册。;
        
        builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency(); //注册仓储

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired();
    }
}
