using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Application.Common.Auth;
using CleanArchi.Boilerplate.Application.Common.Exceptions;
using CleanArchi.Boilerplate.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using StackExchange.Profiling;

namespace CleanArchi.Boilerplate.Infrastructure.Filter;

public class GlobalExceptionsFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment _env;
    private readonly IUser _user;
    //private readonly ILogger<GlobalExceptionsFilter> _logger;

    public GlobalExceptionsFilter(IWebHostEnvironment env, IUser user)
    {
        _env = env;
        _user = user;
    }

    public void OnException(ExceptionContext context)
    {
        var json = new MessageModel<string>();

        Type type = context.Exception.GetType();
        if (type == typeof(ValidationException))
        {
            HandleValidationException(context);
            return;
        }
        else {
            HandleUnknownOtherException(context);
            return;
        }

        StackExchange.Profiling.MiniProfiler.Current.CustomTiming("Errors：", json.msg);

        //错误日志记录
        if (_user.ID != 0) LogContext.PushProperty("UserId", _user.ID);
        if (!string.IsNullOrEmpty(_user.Name)) LogContext.PushProperty("UserName", _user.Name);
        LogContext.PushProperty("ErrorId", Guid.NewGuid().ToString());

        Log.Error("\r\n【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3}"
            , new object[] { json.msg, context.Exception.GetType().Name, context.Exception.Message, context.Exception.StackTrace });
    }

    private void HandleValidationException(ExceptionContext context)
    {
        

        var exception = (ValidationException)context.Exception;

        var details = new ValidationProblemDetails(exception.Errors);
        var result = new BadRequestObjectResult(details);

        var json = new MessageModel<string>
        {
            msg = exception.Message,//错误信息
            status = result.StatusCode.Value,
            msgDev = JsonConvert.SerializeObject(exception.Errors) 
        };

        context.Result = new ContentResult { Content = JsonConvert.SerializeObject(json) };
        context.ExceptionHandled = true;
    }

    private void HandleUnknownOtherException(ExceptionContext context) 
    {
        var json = new MessageModel<string>
        {
            msg = context.Exception.Message,//错误信息
            status = 500//500异常 
        };
        var errorAudit = "Unable to resolve service for";
        if (!string.IsNullOrEmpty(json.msg) && json.msg.Contains(errorAudit))
        {
            json.msg = json.msg.Replace(errorAudit, $"（若新添加服务，需要重新编译项目）{errorAudit}");
        }
        if (_env.EnvironmentName.Equals("Development"))
        {
            json.msgDev = context.Exception.StackTrace;//堆栈信息
        }

        context.Result = new ContentResult { Content = JsonConvert.SerializeObject(json) };
        context.ExceptionHandled = true;
    }

}
