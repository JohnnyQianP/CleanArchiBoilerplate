﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.JobTask.Common;
using CleanArchi.Boilerplate.JobTask.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace CleanArchi.Boilerplate.JobTask.Controllers;

/// <summary>
/// 任务调度
/// </summary>
[Route("api/[controller]/[Action]")]
//[Authorize]
public class JobController : ControllerBase
{
    private ScheduleManager scheduler;

    /// <summary>
    /// 任务调度对象
    /// </summary>
    /// <param name="schedulerCenter"></param>
    public JobController(ScheduleManager schedulermr)
    {
        this.scheduler = schedulermr;
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<BaseResult> AddJob([FromBody] ScheduleEntity entity)
    {
        return await scheduler.AddScheduleJobAsync(entity);
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<BaseResult> StopJob([FromBody] JobKey job)
    {
        return await scheduler.StopOrDelScheduleJobAsync(job.Group, job.Name);
    }

    /// <summary>
    /// 删除任务
    /// </summary> 
    /// <returns></returns>
    [HttpPost]
    public async Task<BaseResult> RemoveJob([FromBody] JobKey job)
    {
        return await scheduler.StopOrDelScheduleJobAsync(job.Group, job.Name, true);
    }

    /// <summary>
    /// 恢复运行暂停的任务
    /// </summary> 
    /// <returns></returns>
    [HttpPost]
    public async Task<BaseResult> ResumeJob([FromBody] JobKey job)
    {
        return await scheduler.ResumeJobAsync(job.Group, job.Name);
    }

    /// <summary>
    /// 查询任务
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<ScheduleEntity> QueryJob([FromBody] JobKey job)
    {
        return await scheduler.QueryJobAsync(job.Group, job.Name);
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<BaseResult> ModifyJob([FromBody] ScheduleEntity entity)
    {
        await scheduler.StopOrDelScheduleJobAsync(entity.JobGroup, entity.JobName, true);
        await scheduler.AddScheduleJobAsync(entity);
        return new BaseResult() { Msg = "修改计划任务成功！" };
    }

    /// <summary>
    /// 立即执行
    /// </summary>
    /// <param name="job"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> TriggerJob([FromBody] JobKey job)
    {
        await scheduler.TriggerJobAsync(job);
        return true;
    }

    /// <summary>
    /// 获取job日志
    /// </summary>
    /// <param name="jobKey"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<List<string>> GetJobLogs([FromBody] JobKey jobKey)
    {
        return await scheduler.GetJobLogsAsync(jobKey);
    }

    /// <summary>
    /// 启动调度
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<bool> StartSchedule()
    {
        return await scheduler.StartScheduleAsync();
    }

    /// <summary>
    /// 停止调度
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<bool> StopSchedule()
    {
        return await scheduler.StopScheduleAsync();
    }

    /// <summary>
    /// 获取所有任务
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<JobInfoEntity>> GetAllJob()
    {
        return await scheduler.GetAllJobAsync();
    }

    /// <summary>
    /// 获取所有Job信息（简要信息 - 刷新数据的时候使用）
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<List<JobBriefInfoEntity>> GetAllJobBriefInfo()
    {
        return await scheduler.GetAllJobBriefInfoAsync();
    }

    /// <summary>
    /// 移除异常信息
    /// </summary>
    /// <param name="jobKey"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> RemoveErrLog([FromBody] JobKey jobKey)
    {
        return false;
    }
}