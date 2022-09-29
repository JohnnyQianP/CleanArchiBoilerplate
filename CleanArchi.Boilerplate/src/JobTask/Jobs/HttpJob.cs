using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CleanArchi.Boilerplate.JobTask.Common;
using CleanArchi.Boilerplate.Shared.Configuration;
using CleanArchi.Boilerplate.Shared.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Quartz;

namespace CleanArchi.Boilerplate.JobTask.Jobs;

[DisallowConcurrentExecution]
[PersistJobDataAfterExecution]
public class HttpJob : IJob
{
    internal static IHttpSender _httpSender ;
    internal static SecurityJWT _securityJWT;

    private static Dictionary<string, string> tokenCache = new Dictionary<string, string>();
    private const string DefaultScope = "RemoteScope";
    public HttpJob() { }
    //public HttpJob(IHttpSender httpSender, IOptions<SecurityJWT> options) 
    //{
    //    _httpSender = httpSender;
    //    _securityJWT = options.Value;
    //}
    public async Task Execute(IJobExecutionContext context)
    {
        //获取相关参数
        var requestUrl = context.JobDetail.JobDataMap.GetString(Constant.REQUESTURL);
        requestUrl = requestUrl?.IndexOf("http") == 0 ? requestUrl : "http://" + requestUrl;
        var requestParameters = context.JobDetail.JobDataMap.GetString(Constant.REQUESTPARAMETERS);
        var headersString = context.JobDetail.JobDataMap.GetString(Constant.HEADERS);
        var mailMessage = (MailMessageEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.MAILMESSAGE) ?? "0");
        var headers = headersString != null ? JsonConvert.DeserializeObject<Dictionary<string, string>>(headersString?.Trim()) : null;
        var requestType = (RequestTypeEnum)int.Parse(context.JobDetail.JobDataMap.GetString(Constant.REQUESTTYPE));

        var loginfo = new LogInfoModel {
            Url = requestUrl,
            BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            RequestType = requestType.ToString(),
            Parameters = requestParameters,
            JobName = $"{context.JobDetail.Key.Group}.{context.JobDetail.Key.Name}"

        };
        var logs = context.JobDetail.JobDataMap[Constant.LOGLIST] as List<string> ?? new List<string>();
        logs.Reverse();
        if (logs.Count >= 20)
            logs.RemoveRange(0, logs.Count - 20);//最新20条
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Restart(); //  开始监视代码运行时间
        HttpResponseMessage response = new HttpResponseMessage();

        try
        {
            if (headers == null)
            {
                headers = new Dictionary<string, string>();
            }
            var token = "";
            if (!tokenCache.ContainsKey(DefaultScope))
            {
                token = CreateJobToken();
                tokenCache.Add(DefaultScope, token);
            }
            else {
                token = tokenCache[DefaultScope];
            }
            if (!string.IsNullOrEmpty(token))
            {
                if (headers.ContainsKey("Authorization"))
                {
                    headers["Authorization"] = $"Bearer {token}";
                }
                else {
                    headers.Add("Authorization", $"Bearer {token}");
                }
            }
            switch (requestType)
            {
                case RequestTypeEnum.Get:
                    response = await _httpSender.GetAsync(requestUrl, headers);
                    break;
                case RequestTypeEnum.Post:
                    response = await _httpSender.PostAsync(requestUrl, requestParameters, headers);
                    break;
                case RequestTypeEnum.Put:
                    response = await _httpSender.PutAsync(requestUrl, requestParameters, headers);
                    break;
                case RequestTypeEnum.Delete:
                    response = await _httpSender.DeleteAsync(requestUrl, headers);
                    break;
            }
            var result = HttpUtility.HtmlEncode(await response.Content.ReadAsStringAsync());

            stopwatch.Stop(); //  停止监视
            loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            loginfo.Seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数;
            loginfo.Result = $"<span class='result'>{ (result.Length < 1000?result: result.Substring(0, 1000))}</span>";
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    stopwatch.Restart();
                    token = CreateJobToken();
                    tokenCache[DefaultScope] = token;
                    headers["Authorization"] = $"Bearer {token}";
                    switch (requestType)
                    {
                        case RequestTypeEnum.Get:
                            response = await _httpSender.GetAsync(requestUrl, headers);
                            break;
                        case RequestTypeEnum.Post:
                            response = await _httpSender.PostAsync(requestUrl, requestParameters, headers);
                            break;
                        case RequestTypeEnum.Put:
                            response = await _httpSender.PutAsync(requestUrl, requestParameters, headers);
                            break;
                        case RequestTypeEnum.Delete:
                            response = await _httpSender.DeleteAsync(requestUrl, headers);
                            break;
                    }
                    result = HttpUtility.HtmlEncode(await response.Content.ReadAsStringAsync());
                    stopwatch.Stop(); //  停止监视
                    loginfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    loginfo.Seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数;
                    loginfo.Result = $"<span class='result'>{ (result.Length < 1000 ? result : result.Substring(0, 1000))}</span>";
                }
                else {
                    loginfo.ErrorMsg = $"<span class='error'>{(result.Length < 3000 ? result : result.Substring(0, 3000))}</span>";
                    context.JobDetail.JobDataMap[Constant.EXCEPTION] = JsonConvert.SerializeObject(loginfo);
                }
                
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop(); //  停止监视            
            double seconds = stopwatch.Elapsed.TotalSeconds;  //总秒数
            loginfo.ErrorMsg = $"<span class='error'>{ex.Message} {ex.StackTrace}</span>";
            context.JobDetail.JobDataMap[Constant.EXCEPTION] = JsonConvert.SerializeObject(loginfo);
            loginfo.Seconds = seconds;
        }
        finally
        {
            logs.Add($"<p>{JsonConvert.SerializeObject(loginfo)}</p>");
            logs.Reverse();
            context.JobDetail.JobDataMap[Constant.LOGLIST] = logs;
        }
    }

    private string CreateJobToken()
    {
        var token = new JwtSecurityToken(
            issuer: _securityJWT.Issuer,
            audience: _securityJWT.Audience,
            claims: new List<Claim> { new(ClaimTypes.Role, "JobApi"), },
            expires: DateTime.Now.AddDays(_securityJWT.JobExpirationInDays),// adddays
            signingCredentials: GetSigningCredentials(_securityJWT.Secret));
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private SigningCredentials GetSigningCredentials(string secretSetting)
    {
        byte[] secret = Encoding.ASCII.GetBytes(secretSetting);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }

}

public class HttpResultModel
{
    /// <summary>
    /// 请求是否成功
    /// </summary>
    public bool IsSuccess { get; set; }
    /// <summary>
    /// 异常消息
    /// </summary>
    public string ErrorMsg { get; set; }
}
