using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CleanArchi.Boilerplate.Shared.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CleanArchi.Boilerplate.Shared.Http;

public class HttpSender: IHttpSender
{

    private readonly IHttpClientFactory _httpClientFactory;
    public HttpSender(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="jsonString">请求参数（Json字符串）</param>
    /// <param name="headers">webapi做用户认证</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> PostAsync(string url, string jsonString, Dictionary<string, string> headers = null)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            jsonString = "{}";
        StringContent content = new StringContent(jsonString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var httpClient = _httpClientFactory.CreateClient();
        foreach (var item in headers)
        {
            httpClient.DefaultRequestHeaders.Remove(item.Key);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
        }
        return await httpClient.PostAsync(new Uri(url), content);
    }

    /// <summary>
    /// Post请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url">url地址</param>
    /// <param name="content">请求参数</param>
    /// <param name="headers">webapi做用户认证</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> PostAsync<T>(string url, T content, Dictionary<string, string> headers = null) where T : class
    {
        return await PostAsync(url, JsonConvert.SerializeObject(content), headers);
    }

    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="headers">webapi做用户认证</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null)
    {
        var httpClient = _httpClientFactory.CreateClient();
        if (headers!=null)
        {
            foreach (var item in headers)
            {
                httpClient.DefaultRequestHeaders.Remove(item.Key);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
            }
        }
        
        return await httpClient.GetAsync(url);
    }

    /// <summary>
    /// Put请求
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="jsonString">请求参数（Json字符串）</param>
    /// <param name="headers">webapi做用户认证</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> PutAsync(string url, string jsonString, Dictionary<string, string> headers = null)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            jsonString = "{}";
        StringContent content = new StringContent(jsonString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        var httpClient = _httpClientFactory.CreateClient();
        if (headers != null) 
        {
            foreach (var item in headers)
            {
                httpClient.DefaultRequestHeaders.Remove(item.Key);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
            }
        }
            
        return await httpClient.PutAsync(url, content);
    }

    /// <summary>
    /// Put请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url">url地址</param>
    /// <param name="content">请求参数</param>
    /// <param name="headers">webapi做用户认证</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> PutAsync<T>(string url, T content, Dictionary<string, string> headers = null) where T : class
    {
        return await PutAsync(url, JsonConvert.SerializeObject(content), headers);
    }

    /// <summary>
    /// Delete请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="headers">webapi做用户认证</param>
    /// <returns></returns>
    public async Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null)
    {
        var httpClient = _httpClientFactory.CreateClient();
        if (headers!=null)
        {
            foreach (var item in headers)
            {
                httpClient.DefaultRequestHeaders.Remove(item.Key);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
            }
        }
        
        return await httpClient.DeleteAsync(url);
    }

    

}