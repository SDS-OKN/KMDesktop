using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AllegroGraphNetCoreClient.Util
{
    public static class RequestUtil
    {
        private static void MakeReq(string Url, string Method, string Body, string ContentType, bool ReadResponse, string Accept,
        string Username, string Password,
        out HttpStatusCode StatusCode, out string ReturnBody)
        {
            HttpWebRequest req = WebRequest.Create(Url) as HttpWebRequest;
            req.Method = Method;
            req.ContentType = ContentType;
            req.Accept = Accept;
            if (Username != null && Password != null)
            {
                req.Credentials = new NetworkCredential(Username, Password);
            }

            if (Method != "GET" && Body != null)
            {
                Stream requestStream = req.GetRequestStream();
                StreamWriter reqInWriter = new StreamWriter(requestStream);
                reqInWriter.Write(Body);
                reqInWriter.Close();
                requestStream.Close();
            }

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            StatusCode = resp.StatusCode;

            if (ReadResponse)
            {
                Stream responseStream = resp.GetResponseStream();
                StreamReader reqOutReader = new StreamReader(responseStream);
                ReturnBody = reqOutReader.ReadToEnd();
                reqOutReader.Close();
                responseStream.Close();
            }
            else
                ReturnBody = null;

            resp.Close();
        }

        /// <summary>
        /// Make a null-returning HTTP request
        /// </summary>
        /// <param name="Url">Request URL</param>
        /// <param name="Method">Request method</param>
        /// <param name="Body">Request body</param>
        /// <param name="ContentType">Content-type header, default set to JSON</param>
        public static void DoReq(string Url, string Method, string Body, string ContentType = "application/json; utf-8",
            string Username = null, string Password = null)
        {
            MakeReq(Url, Method, Body, ContentType, false, "*/*", Username, Password, out var status, ReturnBody: out var returnBody);
            System.Diagnostics.Debug.WriteLine(returnBody);
            if ((int)status < 200 || (int)status > 204)
                throw new AgRequestException("Error while performing the request with error code " + (int)status);
        }

        /// <summary>
        /// Make a JSON-returning HTTP request
        /// </summary>
        /// <param name="Url">Request URL</param>
        /// <param name="Method">Request method</param>
        /// <param name="Body">Request body</param>
        /// <param name="ContentType">Content-type header, default set to JSON</param>
        /// <returns>The raw http response content</returns>
        public static string DoJsonReq(string Url, string Method, string Body, string ContentType = "application/json; utf-8",
            string Username = null, string Password = null)
        {
            MakeReq(Url, Method, Body, ContentType, true, "application/json; utf-8", Username, Password, out var status, out var returnBody);

            if ((int)status < 200 || (int)status > 204)
                throw new AgRequestException("Error while performing the request with error code " + (int)status);

            return returnBody;
        }

        /// <summary>
        /// DoJsonReq with specific Accept type
        /// </summary>
        /// <seealso cref="DoJsonReq"/>
        public static string DoJsonReq(string Url, string Method, string Body, string Accept,
                                       string ContentType = "application/json; utf-8",
                                       string Username = null, string Password = null)
        {
            MakeReq(Url, Method, Body, ContentType, true, Accept, Username, Password, out var status, out var returnBody);

            if ((int)status < 200 || (int)status > 204)
                throw new AgRequestException("Error while performing the request with error code " + (int)status);

            return returnBody;
        }
    }
}
