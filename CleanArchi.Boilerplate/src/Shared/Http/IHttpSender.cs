using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchi.Boilerplate.Shared.Http;

public interface IHttpSender
{
    Task<HttpResponseMessage> PostAsync(string url, string jsonString, Dictionary<string, string> headers = null);
    Task<HttpResponseMessage> PostAsync<T>(string url, T content, Dictionary<string, string> headers = null) where T : class;

    Task<HttpResponseMessage> GetAsync(string url, Dictionary<string, string> headers = null);

    Task<HttpResponseMessage> PutAsync(string url, string jsonString, Dictionary<string, string> headers = null) ;

    Task<HttpResponseMessage> PutAsync<T>(string url, T content, Dictionary<string, string> headers = null) where T : class;

    Task<HttpResponseMessage> DeleteAsync(string url, Dictionary<string, string> headers = null);
}
