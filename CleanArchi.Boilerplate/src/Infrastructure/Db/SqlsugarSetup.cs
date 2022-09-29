using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Shared;
using CleanArchi.Boilerplate.Shared.Output;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SqlSugar;
using StackExchange.Profiling;

namespace CleanArchi.Boilerplate.Infrastructure.Db;

/// <summary>
/// SqlSugar 启动服务
/// </summary>
public static class SqlsugarSetup
{
    public static void AddSqlsugarSetup(this IServiceCollection services, IConfiguration config)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // 单库
        // SqlSugarScope是线程安全，可使用单例注入
        // 参考：https://www.donet5.com/Home/Doc?typeId=1181
        services.AddSingleton<ISqlSugarClient>(o =>
        {
            var memoryCache = o.GetRequiredService<IMemoryCache>();

            // 连接字符串
            var listConfig = new List<ConnectionConfig>{new ConnectionConfig()
            {
                //ConfigId ="",
                ConnectionString = config.GetValue<string>("DatabaseConnection:ConnectionStrings"),
                DbType = (DbType)config.GetValue<int>("DatabaseConnection:DbType"),
                IsAutoCloseConnection = true,
                //IsShardSameThread = false,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        bool sqlAopEnabled = true;//走配置
                        if (sqlAopEnabled)
                        {
                            bool sqlAopOutToLogFileEnabled = config.GetValue<bool>("SetupFlag:EnableSQLAopOutToLogFile");//miniprofile输出到日志文件
                            if (sqlAopOutToLogFileEnabled)
                            {
                                Parallel.For(0, 1, e =>
                                {
                                    //miniprofiler取决于是否开启了miniprofiler:路径/profiler/results-index
                                    StackExchange.Profiling.MiniProfiler.Current.CustomTiming("SQL：", GetParas(p) + "[T-SQL]: " + sql);

                                    Log.Information("SQL:{sqlpara} [T-SQL]:{sql}", GetParas(p), sql);
                                });
                            }
                            bool sqlAopOutToConsoleEnabled = config.GetValue<bool>("SetupFlag:EnableSQLAopOutToConsole");//miniprofile输出到日志文件
                            if (sqlAopOutToConsoleEnabled)
                            {
                                OutPutConsole.WriteColorLine(string.Join("\r\n", new string[] { "--------", "[T-SQL]: " + GetWholeSql(p, sql) }), ConsoleColor.DarkCyan);
                            }
                        }
                    },
                    DataExecuting = (oldValue, entityInfo)=>
                    {
                        //inset生效
                        if (entityInfo.PropertyName == "CreateTime"&&entityInfo.OperationType== DataFilterType.InsertByObject)
                        {
                           entityInfo.SetValue(DateTime.Now);//修改CreateTime字段
                           //entityInfo有字段所有参数
                        }
                        //update生效        
                        if (entityInfo.PropertyName =="UpdateTime" && entityInfo.OperationType == DataFilterType.UpdateByObject)
                        {
                           entityInfo.SetValue(DateTime.Now);//修改UpdateTime字段
                        }
                    }
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                },
                // 从库
                //SlaveConnectionConfigs = listConfig_Slave,
                // 自定义特性
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    DataInfoCacheService = new SqlSugarMemoryCacheService(memoryCache),
                    EntityService = (property, column) =>
                    {
                        if (column.IsPrimarykey && property.PropertyType == typeof(int))
                        {
                            column.IsIdentity = true;
                        }
                    }
                },
                InitKeyType = InitKeyType.Attribute
            }
            };

            return new SqlSugarScope(listConfig);
        });
    }

    private static string GetWholeSql(SugarParameter[] paramArr, string sql)
    {
        foreach (var param in paramArr)
        {
            sql.Replace(param.ParameterName, param.Value.ObjToString());
        }

        return sql;
    }

    private static string GetParas(SugarParameter[] pars)
    {
        string key = "【SQL参数】：";
        foreach (var param in pars)
        {
            key += $"{param.ParameterName}:{param.Value}\n";
        }

        return key;
    }
}