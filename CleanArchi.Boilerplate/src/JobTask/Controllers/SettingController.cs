using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.JobTask.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using Newtonsoft.Json;

namespace CleanArchi.Boilerplate.JobTask.Controllers;

[Route("api/[controller]/[Action]")]
//[Authorize]
public class SettingController: ControllerBase
{
    static string filePath = Path.Combine(AppContext.BaseDirectory, "File\\Mail.txt");
    static string refreshIntervalPath = Path.Combine(AppContext.BaseDirectory, "File\\RefreshInterval.json");

    static MailEntity mailData = null;
    /// <summary>
    /// 保存Mail信息
    /// </summary>
    /// <param name="mailEntity"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> SaveMailInfo([FromBody] MailEntity mailEntity)
    {
        mailData = mailEntity;
        await System.IO.File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(mailEntity));
        return true;
    }

    /// <summary>
    /// 保存刷新间隔
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> SaveRefreshInterval([FromBody] RefreshIntervalEntity entity)
    {
        var basePath = AppContext.BaseDirectory;
        await System.IO.File.WriteAllTextAsync(refreshIntervalPath, JsonConvert.SerializeObject(entity));
        return true;
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<RefreshIntervalEntity> GetRefreshInterval()
    {
        return JsonConvert.DeserializeObject<RefreshIntervalEntity>(await System.IO.File.ReadAllTextAsync(refreshIntervalPath));
    }

    /// <summary>
    /// 获取eMail信息
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<MailEntity> GetMailInfo()
    {
        if (mailData == null)
        {
            var mail = await System.IO.File.ReadAllTextAsync(filePath);
            mailData = JsonConvert.DeserializeObject<MailEntity>(mail);
        }
        return mailData;
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> SendMail([FromBody] SendMailModel model)
    {
        try
        {
            if (model.MailInfo == null)
                model.MailInfo = await GetMailInfo();
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(model.MailInfo.MailFrom, model.MailInfo.MailFrom));
            foreach (var mailTo in model.MailInfo.MailTo.Replace("；", ";").Replace("，", ";").Replace(",", ";").Split(';'))
            {
                message.To.Add(new MailboxAddress(mailTo, mailTo));
            }
            message.Subject = string.Format(model.Title);
            message.Body = new TextPart("html")
            {
                Text = model.Content
            };
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(model.MailInfo.MailHost, 465, true);
                client.Authenticate(model.MailInfo.MailFrom, model.MailInfo.MailPwd);
                client.Send(message);
                client.Disconnect(true);
            }
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
