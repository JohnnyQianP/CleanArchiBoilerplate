using Autofac;
using Autofac.Extensions.DependencyInjection;
using CleanArchi.Boilerplate.WebApi.Extension.AutofacReg;
using CleanArchi.Boilerplate.Infrastructure.AutofacReg;
using CleanArchi.Boilerplate.Infrastructure.MiddlewarePipe;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CleanArchi.Boilerplate.WebApi.Configuration;
using Serilog;
using FluentValidation.AspNetCore;
using CleanArchi.Boilerplate.Infrastructure.Filter;
using Microsoft.OpenApi.Models;
using CleanArchi.Boilerplate.JobTask;
using CleanArchi.Boilerplate.JobTask.Extension;
using CleanArchi.Boilerplate.Shared.Http;

var builder = WebApplication.CreateBuilder(args);
//autofac容器注入
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<AutofacModule>();
        builder.RegisterModule<AutofacControllerModule>();
    });
//configuration
builder.Host.AddConfigurations();
builder.Host.UseSerilog((_, config) =>
{
    config.WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration);
});

//http service
builder.Services.AddHttpClient();
builder.Services.AddTransient<IHttpSender, HttpSender>();

// Add Application services to the container.
builder.Services.AddApplicationServices();

// Add Infrastructure services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddJobTaskServices(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

builder.Services.AddControllers(o => {
    o.Filters.Add(typeof(GlobalExceptionsFilter));
}).AddNewtonsoftJson(options =>
    {
        //忽略循环引用
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        //使用驼峰 首字母小写
        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        //设置时间格式
        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    })
    ;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});



var app = builder.Build();

//ServiceLocator.Instance = app.Services;//for static manually get

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder.Configuration.GetValue<string>("Cors:PolicyName"));
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiniProfilerMiddleware(builder.Configuration);//miniprofiler

app.UseJobTaskMiddleware(builder.Configuration);//job

app.MapControllers();

app.Run();
