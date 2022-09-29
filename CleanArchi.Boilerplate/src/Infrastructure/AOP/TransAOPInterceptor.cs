using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using CleanArchi.Boilerplate.Repository.Uow;
using Microsoft.Extensions.Logging;

namespace CleanArchi.Boilerplate.Infrastructure.AOP;

/// <summary>
/// find in autofac ioc di
/// </summary>
public class TransAOPInterceptor : IInterceptor
{
    private readonly ILogger<TransAOPInterceptor> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransAOPInterceptor(IUnitOfWork unitOfWork, ILogger<TransAOPInterceptor> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// 实例化IInterceptor唯一方法 
    /// </summary>
    /// <param name="invocation">包含被拦截方法的信息</param>
    public void Intercept(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        //对当前方法的特性验证
        //如果需要验证
        if (method.GetCustomAttribute<UowAttribute>(true) is { } uta)
        {
            try
            {
                Before(method, uta.Propagation);

                invocation.Proceed();

                // 异步获取异常，先执行
                if (IsAsyncMethod(invocation.Method))
                {
                    var result = invocation.ReturnValue;
                    if (result is Task)
                    {
                        Task.WaitAll(result as Task);
                    }
                }

                After(method);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                AfterException(method);
                throw;
            }
        }
        else
        {
            invocation.Proceed(); //直接执行被拦截方法
        }
    }

    private void Before(MethodInfo method, Propagation propagation)
    {
        switch (propagation)
        {
            case Propagation.Required:
                if (_unitOfWork.TranCount <= 0)
                {
                    _logger.LogDebug($"Begin Transaction");
                    Console.WriteLine($"Begin Transaction");
                    _unitOfWork.BeginTran(method);
                }

                break;
            case Propagation.Mandatory:
                if (_unitOfWork.TranCount <= 0)
                {
                    throw new Exception("事务传播机制为:[Mandatory],当前不存在事务");
                }

                break;
            case Propagation.Nested:
                _logger.LogDebug($"Begin Transaction");
                Console.WriteLine($"Begin Transaction");
                _unitOfWork.BeginTran(method);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(propagation), propagation, null);
        }
    }

    private void After(MethodInfo method)
    {
        _unitOfWork.CommitTran(method);
    }

    private void AfterException(MethodInfo method)
    {
        _unitOfWork.RollbackTran(method);
    }

    public static bool IsAsyncMethod(MethodInfo method)
    {
        return (
            method.ReturnType == typeof(Task) ||
            (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        );
    }
}
