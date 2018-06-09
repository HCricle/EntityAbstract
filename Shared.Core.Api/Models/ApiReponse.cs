using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Api.Models
{
    /// <summary>
    /// api的响应结果
    /// </summary>
    public class ApiReponse
    {
        public ApiReponse(HttpResponseMessage httpResponse)
        {
            HttpResponse = httpResponse;
            if (HttpResponse==null)
            {
                throw new ArgumentNullException("reponse 不能为空");
            }
        }

        /// <summary>
        /// 响应结果
        /// </summary>
        public HttpResponseMessage HttpResponse { get; }
        /// <summary>
        /// 响应状态代码
        /// </summary>
        public HttpStatusCode ResponesCode => HttpResponse.StatusCode;
        /// <summary>
        /// 是否成功200(OK)
        /// </summary>
        public bool IsSucceed => HttpResponse != null && HttpResponse.IsSuccessStatusCode;
        /// <summary>
        /// 将响应内容读为字符串
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetContentStreamAsync()
        {
            return await HttpResponse.Content.ReadAsStringAsync();
        }
        /// <summary>
        /// 将响应内容转换为类型 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>成功返回T实例，失败返回null</returns>
        public async Task<T> GetForAsync<T>()
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(await GetContentStreamAsync());
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
