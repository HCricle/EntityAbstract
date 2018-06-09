using Shared.Core.Api.Extensions;
using Shared.Core.Api.Models;
using SharedEA.Core.WebApi.JWT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Core.Api.WebApi
{
    public class WebApiConnecter
    {
        public Uri BaseUri { get; }
        public WebApiConnecter(Uri baseUri)
        {
            BaseUri = baseUri;
        }
        protected TokenEntity accountToken;
        /// <summary>
        /// 是否存在用户token
        /// </summary>
        public bool HasAccountToken => AccountToken != null;
        /// <summary>
        /// 当前用户token
        /// </summary>
        public TokenEntity AccountToken => accountToken;
        /// <summary>
        /// 如果没有token进行操作
        /// </summary>
        public event Action NonTokenAction;

        public HttpClient Client => new HttpClient() { BaseAddress = BaseUri };
        /// <summary>
        /// get 
        /// </summary>
        /// <param name="url">组合路径</param>
        /// <param name="needLogin">是否需要登陆</param>
        /// <returns></returns>
        public async Task<ApiReponse> GetAsync(string url, bool needLogin = false)
        {

            if (needLogin && !HasAccountToken)
            {
                NonTokenAction?.Invoke();
                return null;
            }
            HttpResponseMessage respones;
            using (var client = Client)
            {
                if (needLogin)
                {
                    client.AddAuthorizationToken(AccountToken.AccessToken);
                }
                respones = await client.GetAsync(url);
            }
            return new ApiReponse(respones);
        }
        /// <summary>
        /// put 多表单提交
        /// </summary>
        /// <param name="url">组合路径</param>
        /// <param name="files">发送的文件</param>
        /// <param name="needLogin">是否需要登陆才能提交</param>
        /// <param name="contents">提交内容</param>
        /// <returns></returns>
        public async Task<ApiReponse> PutAsync(string url, IEnumerable<ApiFileModel> files,
            bool needLogin = true, params (string, object)[] contents)
        {
            if (needLogin && !HasAccountToken)
            {
                NonTokenAction?.Invoke();
                return null;
            }
            HttpResponseMessage respones;
            using (var client = Client)
            {
                using (var formContent = new MultipartFormDataContent())
                {
                    if (contents != null)
                    {
                        foreach (var item in contents)
                        {
                            formContent.Add(new StringContent(item.Item2.ToString()), item.Item1);
                        }
                    }
                    if (files != null)
                    {
                        ApiFileModel s;
                        for (int i = 0; i < files.Count(); i++)
                        {
                            s = files.ElementAt(i);
                            if (s.FileStream == null && !string.IsNullOrEmpty(s.Path))
                            {
                                if (s.FileStream!=null)
                                {
                                    formContent.AddFile(s.Name, s.FileStream);
                                }
                                else if(File.Exists(s.Path))
                                {
                                    var stream = File.OpenRead(s.Path);
                                    formContent.AddFile(s.Name, stream);
                                }
                                else
                                {
                                    throw new FileNotFoundException(s.Path);
                                }
                            }
                            else
                            {
                                throw new ArgumentException("文件传输时，流或路径必须指定一个");
                            }
                        }
                    }
                    if (needLogin)
                    {
                        client.AddAuthorizationToken(AccountToken.AccessToken);
                    }
                    respones = await client.PutAsync(url, formContent);
                }
            }
            return new ApiReponse(respones);
        }
        /// <summary>
        /// post 表单提交
        /// </summary>
        /// <param name="url">组合路径</param>
        /// <param name="needLogin">是否需要登陆</param>
        /// <param name="contents">提交内容</param>
        /// <returns></returns>
        public async Task<ApiReponse> PostAsync(string url, bool needLogin = true, params (string, object)[] contents)
        {
            if (needLogin && !HasAccountToken)
            {
                NonTokenAction?.Invoke();
                return null;
            }
            HttpResponseMessage respones;
            using (var client = Client)
            {
                if (needLogin)
                {
                    client.AddAuthorizationToken(AccountToken.AccessToken);
                }
                IEnumerable<KeyValuePair<string, string>> cs = null;
                if (contents != null)
                {
                    cs = contents.Select(c => new KeyValuePair<string, string>(c.Item1, c.Item2.ToString()));
                }
                cs = cs ?? new KeyValuePair<string, string>[] { };
                using (var formContent = new FormUrlEncodedContent(cs))
                {
                    /*
                    if (contents != null)
                    {
                        foreach (var item in contents)
                        {
                            formContent.Add(new StringContent(item.Item1), item.Item2.ToString());
                        }
                    }
                    */
                    respones = await client.PostAsync(url, formContent);
                }
            }
            return new ApiReponse(respones);
        }
        /// <summary>
        /// delete
        /// </summary>
        /// <param name="url">组合路径</param>
        /// <param name="needLogin">是否需要登陆</param>
        /// <returns></returns>
        public async Task<ApiReponse> DeleteAsync(string url, bool needLogin = true)
        {
            if (needLogin && !HasAccountToken)
            {
                NonTokenAction?.Invoke();
                return null;
            }
            HttpResponseMessage respones;
            using (var client = Client)
            {
                if (needLogin)
                {
                    client.AddAuthorizationToken(AccountToken.AccessToken);
                }
                respones = await client.DeleteAsync(url);
            }
            return new ApiReponse(respones);

        }
        /// <summary>
        /// 确保存在token，不存在返回true
        /// </summary>
        public bool EnSureToken()
        {
            if (!HasAccountToken)
            {
                NonTokenAction?.Invoke();
            }
            return !HasAccountToken;
        }

    }
}
