using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Web;
using AllegroGraphNetCoreClient.Util;
// ReSharper disable InvalidXmlDocComment

namespace AllegroGraphNetCoreClient.Mini
{
    public static class AgRequestService
    {
        private static string GenerateUrlParameters(Dictionary<string, object> parameters)
        {
            StringBuilder builder = new StringBuilder();

            void AppendParameter(string key, string value)
            {
                if (builder.Length > 0) builder.Append("&");
                builder.Append(key + "=" + value);
            }

            foreach (string key in parameters.Keys)
            {
                object value = parameters[key];
                if (value is string)
                    AppendParameter(key, HttpUtility.UrlEncode(value as string));
                else if (value is List<string>)
                {
                    foreach (string v in value as List<string>)
                        AppendParameter(key, HttpUtility.UrlEncode(v));
                }
                else
                    AppendParameter(key, HttpUtility.UrlEncode(value.ToString()));
            }
            return builder.ToString();
        }

        private static void PrepareReq(IAgUrl Base, string method, string relativeUrl, object body, out string absUrl, out string bodyString, out string contentType)
        {
            absUrl = Base.Url + relativeUrl;
            contentType = "application/json; utf-8";
            //Console.WriteLine(AbsUrl);
            bodyString = null;
            if (body == null)
            {
            }
            else
            {
                if (body is Dictionary<string, object> objects)
                {
                    string parameters = GenerateUrlParameters(objects);
                    // If it's "GET" or "DELETE", put the parameters into the URL
                    if (method == "GET" || method == "DELETE")
                    {
                        if (parameters.Length > 0)
                            absUrl += "?" + parameters;
                    }
                    else if (method == "POST" || method == "PUT")
                        // Else, parameters will go into the body
                        bodyString = parameters;
                }
                else if (body is string)
                {
                    contentType = "plain/text; utf-8";
                    bodyString = (string)body;
                }
                else
                {
                    // If body is not string, jsonize it
                    bodyString = JsonSerializer.Serialize(body);
                    //Console.WriteLine(BodyString);
                }
            }
        }

        /// <summary>
        /// Execute a non-returning HTTP request
        /// </summary>
        /// <param name="Base">URL</param>
        /// <param name="method">HTTP Method</param>
        /// <param name="relativeUrl">Relative URL to the Base</param>
        /// <param name="needsAuth">If authorization is needed, default true</param>
        /// <param name="body">HTTP Body</param>
        public static void DoReq(IAgUrl Base, string method, string relativeUrl, object body = null, bool needsAuth = true)
        {
            PrepareReq(Base, method, relativeUrl, body, out var absUrl, out var bodyString, out var contentType);
            string username = null, password = null;
            if (needsAuth)
            {
                username = Base.Username;
                password = Base.Password;
            }
            Console.WriteLine(absUrl);
            RequestUtil.DoReq(absUrl, method, bodyString, contentType, username, password);
        }

        /// <summary>
        /// DoReq with specific content-type
        /// <seealso cref="DoReq"/>
        /// </summary>
        public static void DoReq(IAgUrl Base, string method, string relativeUrl, string contentType, object body = null, bool needsAuth = true)
        {
            // ReSharper disable once UnusedVariable
            // ReSharper disable once InconsistentNaming
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            PrepareReq(Base, method, relativeUrl, body, out var absUrl, out var bodyString, out var _contentType);
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            string username = null, password = null;
            if (needsAuth)
            {
                username = Base.Username;
                password = Base.Password;
            }
            
            RequestUtil.DoReq(absUrl, method, bodyString, contentType, username, password);
        }

        /// <summary>
        /// Make a HTTP request with return value in the JSON format
        /// </summary>
        /// <seealso cref="DoReq"/>
        public static string DoReqAndGet(IAgUrl Base, string method, string relativeUrl, object body = null, bool needsAuth = true)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));
            if (relativeUrl == null) throw new ArgumentNullException(nameof(relativeUrl));
            PrepareReq(Base, method, relativeUrl, body, out var absUrl, out var bodyString, out var contentType);
            Console.WriteLine(absUrl);
            string username = null, password = null;
            if (needsAuth)
            {
                username = Base.Username;
                password = Base.Password;
            }

            return RequestUtil.DoJsonReq(absUrl, method, bodyString, contentType, username, password);
        }

        /// <summary>
        /// Make a HTTP request with return value in the JSON format and deserialize into type T
        /// </summary>
        /// <seealso cref="DoReqAndGet"/>
        public static T DoReqAndGet<T>(IAgUrl Base, string method, string relativeUrl, object body = null, bool needsAuth = true)
        {
            if (Base == null) throw new ArgumentNullException(nameof(Base));
            if (method == null) throw new ArgumentNullException(nameof(method));
            string result = DoReqAndGet(Base, method, relativeUrl, body, needsAuth);
            return JsonSerializer.Deserialize<T>(result);
        }

        /// <summary>
        /// DoReqAndGet with specific accept headers
        /// <seealso cref="DoReqAndGet"/>
        /// </summary>
        public static string DoReqAndGet(IAgUrl Base, string method, string relativeUrl, string accept, object body = null, bool needsAuth = true)
        {
            PrepareReq(Base, method, relativeUrl, body, out var absUrl, out var bodyString, out var contentType);
            Console.WriteLine(absUrl);
            string username = null, password = null;
            if (needsAuth)
            {
                username = Base.Username;
                password = Base.Password;
            }
            return RequestUtil.DoJsonReq(absUrl, method, bodyString, accept, contentType, username, password);
        }

        /// <summary>
        /// DoReqAndGet with specific accept headers and type T
        /// <seealso cref="DoReqAndGet"/>
        /// </summary>
        public static T DoReqAndGet<T>(IAgUrl Base, string method, string relativeUrl, string accept, object body = null, bool needsAuth = true)
        {
            string result = DoReqAndGet(Base, method, relativeUrl, accept, body, needsAuth);
            return JsonSerializer.Deserialize<T>(result);
        }
    }
}
