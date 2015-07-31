using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace BuildSharp.Agent.GitLab
{
    public static class WebAPI
    {
        private static string DoWebRequest(string URL, string method, string content)
        {
            var request = WebRequest.Create(URL);
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            request.ContentLength = content.Length;
            request.Timeout = 2500;
            request.Method = method;

            if (method != "GET")
            {
                var requestStream = new StreamWriter(request.GetRequestStream(),
                    Encoding.ASCII);
                requestStream.Write(content);
                requestStream.Close();
            }

            var responseStream = request.GetResponse().GetResponseStream();
            var responseRequest = new StreamReader(responseStream, Encoding.UTF8);
            return responseRequest.ReadToEnd();
        }

        private const int NumRequestTries = 5;

        /// <summary>
        /// PUT a String to an URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <param name="content">String to PUT</param>
        /// <returns>Server Response</returns>
        public static string GET(string URL, string content, out HttpStatusCode status)
        {
            status = 0;

            var numTries = 0;
            while (numTries <= NumRequestTries)
            {
                try
                {
                    return DoWebRequest(URL, "GET", content);
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.ProtocolError)
                        throw;

                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                        status = response.StatusCode;

                    return null;
                }
                catch (Exception)
                {
                    numTries++;
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        /// <summary>
        /// PUT a String to an URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <param name="content">String to PUT</param>
        /// <returns>Server Response</returns>
        public static string PUT(string URL, string content, out HttpStatusCode status)
        {
            status = 0;

            var numTries = 0;
            while (numTries <= NumRequestTries)
            {
                try
                {
                    return DoWebRequest(URL, "PUT", content);
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.ProtocolError)
                        throw;

                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                        status = response.StatusCode;

                    return null;
                }
                catch (Exception)
                {
                    numTries++;
                    Thread.Sleep(1000);
                }
            }

            return null;
        }

        /// <summary>
        /// POST a String to an URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <param name="content">String to POST</param>
        /// <returns>Server Response</returns>
        public static string POST(string URL, string content, out HttpStatusCode status)
        {
            status = 0;

            var numTries = 0;
            while (numTries <= NumRequestTries)
            {
                try
                {
                    return DoWebRequest(URL, "POST", content);
                }
                catch (WebException ex)
                {
                    if (ex.Status != WebExceptionStatus.ProtocolError)
                        throw;

                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                        status = response.StatusCode;

                    return null;
                }
                catch (Exception)
                {
                    numTries++;
                    Thread.Sleep(1000);
                }
            }

            return null;
        }
    }
}
