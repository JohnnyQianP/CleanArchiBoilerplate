using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.Infrastructure.AOP;

/// <summary>
/// UowAttribute就是使用时候的验证，把它添加到需要执行事务的方法中，即可完成事务的操作。
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class UowAttribute : Attribute
{
    /// <summary>
    /// 事务传播方式
    /// </summary>
    public Propagation Propagation { get; set; } = Propagation.Required;
}

public enum Propagation
{
    /// <summary>
    /// 默认：如果当前没有事务，就新建一个事务，如果已存在一个事务中，加入到这个事务中。
    /// </summary>
    Required = 0,

    /// <summary>
    /// 使用当前事务，如果没有当前事务，就抛出异常
    /// </summary>
    Mandatory = 1,

    /// <summary>
    /// 以嵌套事务方式执行
    /// </summary>
    Nested = 2,
}
