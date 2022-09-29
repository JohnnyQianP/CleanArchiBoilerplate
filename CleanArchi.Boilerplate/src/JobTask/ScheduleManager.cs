﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.JobTask.Common;
using CleanArchi.Boilerplate.JobTask.Entity;
using CleanArchi.Boilerplate.JobTask.Jobs;
using CleanArchi.Boilerplate.Shared.Configuration;
using CleanArchi.Boilerplate.Shared.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Quartz.Impl.AdoJobStore.Common;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using Quartz.Simpl;
using Quartz.Spi;
using Quartz.Util;

namespace CleanArchi.Boilerplate.JobTask;

public class ScheduleManager
{
    /// <summary>
    /// 任务调度对象
    /// </summary>
    private static readonly ScheduleManager instance;

    private static IDbProvider _dbProvider;

    private static ServiceProvider _serviceProvider;
    static ScheduleManager()
    {
        instance = new ScheduleManager();
    }

    public static ScheduleManager getInstance(IDbProvider dbProvider, ServiceProvider serviceProvider)
    {
        _dbProvider = dbProvider;
        _serviceProvider = serviceProvider;
        return instance;
    }
    private IScheduler _scheduler;

    private IScheduler Scheduler
    {
        get
        {
            if (_scheduler != null)
            {
                return _scheduler;
            }

            //创建db

            if (_dbProvider == null)
            {
                throw new Exception("dbProvider or driverDelegateType is null");
            }

            DBConnectionManager.Instance.AddConnectionProvider("default", _dbProvider);
            var serializer = new JsonObjectSerializer();
            serializer.Initialize();
            var jobStore = new JobStoreTX
            {
                DataSource = "default",
                TablePrefix = "QRTZ_",
                InstanceId = "AUTO",
                DriverDelegateType = typeof(StdAdoDelegate).AssemblyQualifiedName,
                ObjectSerializer = serializer,
            };
            DirectSchedulerFactory.Instance.CreateScheduler("Scheduler", "AUTO", new DefaultThreadPool(), jobStore);
            _scheduler = SchedulerRepository.Instance.Lookup("Scheduler").Result;

            //var props = new NameValueCollection
            //{
            //    { "quartz.scheduler.instanceName", "Scheduler"},
            //    { "quartz.scheduler.instanceId", "AUTO"},

            //    //{ "quartz.threadPool.type", "Quartz.Simpl.DefaultThreadPool, Quartz"},
            //    //{ "quartz.threadPool.threadCount", "25"},
            //    //{ "quartz.threadPool.threadPriority", "5"},

            //    { "quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX"},
            //    { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate"},
            //    { "quartz.jobStore.useProperties", "true"},
            //    { "quartz.jobStore.dataSource", "default"},
            //    { "quartz.jobStore.tablePrefix", "QRTZ_"},

            //    //{ "quartz.jobStore.clustered", "true"},
            //    //{ "quartz.jobStore.clusterCheckinInterval", "20000"},                

            //    { "quartz.dataSource.default.provider", "SqlServer"},
            //    { "quartz.dataSource.default.connectionString", _dbProvider.ConnectionString},
            //    { "quartz.serializer.type", "json" }
            //};
            //var factory = new StdSchedulerFactory(props);// _serviceProvider.GetRequiredService<ISchedulerFactory>();
            //_scheduler = factory.GetScheduler().GetAwaiter().GetResult();
            //_scheduler.JobFactory = _serviceProvider.GetRequiredService<IJobFactory>();// new HttpJobFactory(_serviceProvider);
            HttpJob._securityJWT = _serviceProvider.GetRequiredService<IOptions<SecurityJWT>>().Value;
            HttpJob._httpSender = _serviceProvider.GetRequiredService<IHttpSender>();
            _scheduler.Start();//默认开始调度器
            return _scheduler;
        }
    }

    /// <summary>
    /// 添加一个工作调度
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<BaseResult> AddScheduleJobAsync(ScheduleEntity entity)
    {
        var result = new BaseResult();
        try
        {
            //检查任务是否已存在
            var jobKey = new JobKey(entity.JobName, entity.JobGroup);
            if (await Scheduler.CheckExists(jobKey))
            {
                result.Code = 500;
                result.Msg = "任务已存在";
                return result;
            }
            //http请求配置
            var httpDir = new Dictionary<string, string>()
                {
                    { "RequestUrl",entity.RequestUrl},
                    { "RequestParameters",entity.RequestParameters},
                    { "RequestType", ((int)entity.RequestType).ToString()},
                    { Constant.HEADERS, entity.Headers},
                    { Constant.MAILMESSAGE, ((int)entity.MailMessage).ToString()},
                };
            // 定义这个工作，并将其绑定到我们的IJob实现类                
            IJobDetail job = JobBuilder.Create<HttpJob>()//CreateForAsync
                .SetJobData(new JobDataMap(httpDir))
                .WithDescription(entity.Description)
                .WithIdentity(entity.JobName, entity.JobGroup)
                .Build();
            // 创建触发器
            ITrigger trigger;
            //校验是否正确的执行周期表达式
            if (entity.TriggerType == TriggerTypeEnum.Cron)//CronExpression.IsValidExpression(entity.Cron))
            {
                trigger = CreateCronTrigger(entity);
            }
            else
            {
                trigger = CreateSimpleTrigger(entity);
            }

            // 告诉Quartz使用我们的触发器来安排作业
            
            await Scheduler.ScheduleJob(job, trigger);
            result.Code = 200;
        }
        catch (Exception ex)
        {
            result.Code = 505;
            result.Msg = ex.Message;
        }
        return result;
    }

    /// <summary>
    /// 暂停/删除 指定的计划
    /// </summary>
    /// <param name="jobGroup">任务分组</param>
    /// <param name="jobName">任务名称</param>
    /// <param name="isDelete">停止并删除任务</param>
    /// <returns></returns>
    public async Task<BaseResult> StopOrDelScheduleJobAsync(string jobGroup, string jobName, bool isDelete = false)
    {
        BaseResult result;
        try
        {
            await Scheduler.PauseJob(new JobKey(jobName, jobGroup));
            if (isDelete)
            {
                await Scheduler.DeleteJob(new JobKey(jobName, jobGroup));
                result = new BaseResult
                {
                    Code = 200,
                    Msg = "删除任务计划成功！"
                };
            }
            else
            {
                result = new BaseResult
                {
                    Code = 200,
                    Msg = "停止任务计划成功！"
                };
            }

        }
        catch (Exception ex)
        {
            result = new BaseResult
            {
                Code = 505,
                Msg = "停止任务计划失败" + ex.Message
            };
        }
        return result;
    }

    /// <summary>
    /// 恢复运行暂停的任务
    /// </summary>
    /// <param name="jobName">任务名称</param>
    /// <param name="jobGroup">任务分组</param>
    public async Task<BaseResult> ResumeJobAsync(string jobGroup, string jobName)
    {
        BaseResult result = new BaseResult();
        try
        {
            //检查任务是否存在
            var jobKey = new JobKey(jobName, jobGroup);
            if (await Scheduler.CheckExists(jobKey))
            {
                //任务已经存在则暂停任务
                await Scheduler.ResumeJob(jobKey);
                result.Msg = "恢复任务计划成功！";
                //Log.Information(string.Format("任务“{0}”恢复运行", jobName));
            }
            else
            {
                result.Msg = "任务不存在";
            }
        }
        catch (Exception ex)
        {
            result.Msg = "恢复任务计划失败！";
            result.Code = 500;
            //Log.Error(string.Format("恢复任务失败！{0}", ex));
        }
        return result;
    }

    /// <summary>
    /// 查询任务
    /// </summary>
    /// <param name="jobGroup"></param>
    /// <param name="jobName"></param>
    /// <returns></returns>
    public async Task<ScheduleEntity> QueryJobAsync(string jobGroup, string jobName)
    {
        var entity = new ScheduleEntity();
        var jobKey = new JobKey(jobName, jobGroup);
        var jobDetail = await Scheduler.GetJobDetail(jobKey);
        var triggersList = await Scheduler.GetTriggersOfJob(jobKey);
        var triggers = triggersList.AsEnumerable().FirstOrDefault();
        var intervalSeconds = (triggers as SimpleTriggerImpl)?.RepeatInterval.TotalSeconds;
        entity.RequestUrl = jobDetail.JobDataMap.GetString(Constant.REQUESTURL);
        entity.BeginTime = triggers.StartTimeUtc.LocalDateTime;
        entity.EndTime = triggers.EndTimeUtc?.LocalDateTime;
        entity.IntervalSecond = intervalSeconds.HasValue ? Convert.ToInt32(intervalSeconds.Value) : 0;
        entity.JobGroup = jobGroup;
        entity.JobName = jobName;
        entity.Cron = (triggers as CronTriggerImpl)?.CronExpressionString;
        entity.RunTimes = (triggers as SimpleTriggerImpl)?.RepeatCount;
        entity.TriggerType = triggers is SimpleTriggerImpl ? TriggerTypeEnum.Simple : TriggerTypeEnum.Cron;
        entity.RequestType = (RequestTypeEnum)int.Parse(jobDetail.JobDataMap.GetString(Constant.REQUESTTYPE));
        entity.RequestParameters = jobDetail.JobDataMap.GetString(Constant.REQUESTPARAMETERS);
        entity.Headers = jobDetail.JobDataMap.GetString(Constant.HEADERS);
        entity.MailMessage = (MailMessageEnum)int.Parse(jobDetail.JobDataMap.GetString(Constant.MAILMESSAGE) ?? "0");
        entity.Description = jobDetail.Description;
        return entity;
    }

    /// <summary>
    /// 立即执行
    /// </summary>
    /// <param name="jobKey"></param>
    /// <returns></returns>
    public async Task<bool> TriggerJobAsync(JobKey jobKey)
    {
        await Scheduler.TriggerJob(jobKey);
        return true;
    }

    /// <summary>
    /// 获取job日志
    /// </summary>
    /// <param name="jobKey"></param>
    /// <returns></returns>
    public async Task<List<string>> GetJobLogsAsync(JobKey jobKey)
    {
        var jobDetail = await Scheduler.GetJobDetail(jobKey);
        return jobDetail.JobDataMap[Constant.LOGLIST] as List<string>;
    }

    /// <summary>
    /// 获取所有Job（详情信息 - 初始化页面调用）
    /// </summary>
    /// <returns></returns>
    public async Task<List<JobInfoEntity>> GetAllJobAsync()
    {
        List<JobKey> jboKeyList = new List<JobKey>();
        List<JobInfoEntity> jobInfoList = new List<JobInfoEntity>();
        var groupNames = await Scheduler.GetJobGroupNames();
        foreach (var groupName in groupNames.OrderBy(t => t))
        {
            jboKeyList.AddRange(await Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
            jobInfoList.Add(new JobInfoEntity() { GroupName = groupName });
        }
        foreach (var jobKey in jboKeyList.OrderBy(t => t.Name))
        {
            var jobDetail = await Scheduler.GetJobDetail(jobKey);
            var triggersList = await Scheduler.GetTriggersOfJob(jobKey);
            var triggers = triggersList.AsEnumerable().FirstOrDefault();

            var interval = string.Empty;
            if (triggers is SimpleTriggerImpl)
                interval = (triggers as SimpleTriggerImpl)?.RepeatInterval.ToString();
            else
                interval = (triggers as CronTriggerImpl)?.CronExpressionString;

            foreach (var jobInfo in jobInfoList)
            {
                if (jobInfo.GroupName == jobKey.Group)
                {
                    jobInfo.JobInfoList.Add(new JobInfo()
                    {
                        Name = jobKey.Name,
                        LastErrMsg = jobDetail.JobDataMap.GetString(Constant.EXCEPTION),
                        RequestUrl = jobDetail.JobDataMap.GetString(Constant.REQUESTURL),
                        TriggerState = await Scheduler.GetTriggerState(triggers.Key),
                        PreviousFireTime = triggers.GetPreviousFireTimeUtc()?.LocalDateTime,
                        NextFireTime = triggers.GetNextFireTimeUtc()?.LocalDateTime,
                        BeginTime = triggers.StartTimeUtc.LocalDateTime,
                        Interval = interval,
                        EndTime = triggers.EndTimeUtc?.LocalDateTime,
                        Description = jobDetail.Description
                    });
                    continue;
                }
            }
        }
        return jobInfoList;
    }


    /// <summary>
    /// 获取所有Job信息（简要信息 - 刷新数据的时候使用）
    /// </summary>
    /// <returns></returns>
    public async Task<List<JobBriefInfoEntity>> GetAllJobBriefInfoAsync()
    {
        List<JobKey> jboKeyList = new List<JobKey>();
        List<JobBriefInfoEntity> jobInfoList = new List<JobBriefInfoEntity>();
        var groupNames = await Scheduler.GetJobGroupNames();
        foreach (var groupName in groupNames.OrderBy(t => t))
        {
            jboKeyList.AddRange(await Scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)));
            jobInfoList.Add(new JobBriefInfoEntity() { GroupName = groupName });
        }
        foreach (var jobKey in jboKeyList.OrderBy(t => t.Name))
        {
            var jobDetail = await Scheduler.GetJobDetail(jobKey);
            var triggersList = await Scheduler.GetTriggersOfJob(jobKey);
            var triggers = triggersList.AsEnumerable().FirstOrDefault();

            foreach (var jobInfo in jobInfoList)
            {
                if (jobInfo.GroupName == jobKey.Group)
                {
                    jobInfo.JobInfoList.Add(new JobBriefInfo()
                    {
                        Name = jobKey.Name,
                        LastErrMsg = jobDetail.JobDataMap.GetString(Constant.EXCEPTION),
                        TriggerState = await Scheduler.GetTriggerState(triggers.Key),
                        PreviousFireTime = triggers.GetPreviousFireTimeUtc()?.LocalDateTime,
                        NextFireTime = triggers.GetNextFireTimeUtc()?.LocalDateTime,
                    });
                    continue;
                }
            }
        }
        return jobInfoList;
    }

    /// <summary>
    /// 开启调度器
    /// </summary>
    /// <returns></returns>
    public async Task<bool> StartScheduleAsync()
    {
        //开启调度器
        if (Scheduler.InStandbyMode)
        {
            await Scheduler.Start();
            //Log.Information("任务调度启动！");
        }
        return Scheduler.InStandbyMode;
    }

    /// <summary>
    /// 停止任务调度
    /// </summary>
    public async Task<bool> StopScheduleAsync()
    {
        //判断调度是否已经关闭
        if (!Scheduler.InStandbyMode)
        {
            //等待任务运行完成
            await Scheduler.Standby(); //TODO  注意：Shutdown后Start会报错，所以这里使用暂停。
                                       //Log.Information("任务调度暂停！");
        }
        return !Scheduler.InStandbyMode;
    }

    /// <summary>
    /// 创建类型Simple的触发器
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private ITrigger CreateSimpleTrigger(ScheduleEntity entity)
    {
        //作业触发器
        if (entity.RunTimes.HasValue && entity.RunTimes > 0)
        {
            return TriggerBuilder.Create()
           .WithIdentity(entity.JobName, entity.JobGroup)
           .StartAt(entity.BeginTime)//开始时间
           .EndAt(entity.EndTime)//结束数据
           .WithSimpleSchedule(x =>
           {
               x.WithIntervalInSeconds(entity.IntervalSecond.Value)//执行时间间隔，单位秒
                    .WithRepeatCount(entity.RunTimes.Value)//执行次数、默认从0开始
                    .WithMisfireHandlingInstructionFireNow();
           })
           .ForJob(entity.JobName, entity.JobGroup)//作业名称
           .Build();
        }
        else
        {
            return TriggerBuilder.Create()
           .WithIdentity(entity.JobName, entity.JobGroup)
           .StartAt(entity.BeginTime)//开始时间
           .EndAt(entity.EndTime)//结束数据
           .WithSimpleSchedule(x =>
           {
               x.WithIntervalInSeconds(entity.IntervalSecond.Value)//执行时间间隔，单位秒
                    .RepeatForever()//无限循环
                    .WithMisfireHandlingInstructionFireNow();
           })
           .ForJob(entity.JobName, entity.JobGroup)//作业名称
           .Build();
        }

    }

    /// <summary>
    /// 创建类型Cron的触发器
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private ITrigger CreateCronTrigger(ScheduleEntity entity)
    {
        // 作业触发器
        return TriggerBuilder.Create()

               .WithIdentity(entity.JobName, entity.JobGroup)
               .StartAt(entity.BeginTime)//开始时间
               .EndAt(entity.EndTime)//结束时间
               .WithCronSchedule(entity.Cron, cronScheduleBuilder => cronScheduleBuilder.WithMisfireHandlingInstructionFireAndProceed())//指定cron表达式
               .ForJob(entity.JobName, entity.JobGroup)//作业名称
               .Build();
    }
}
