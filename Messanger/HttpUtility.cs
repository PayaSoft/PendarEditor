using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Paya.Automation.Editor.Messanger
{
    public class HttpUtility
    {
        private HttpWebRequest request;

        public void Abort()
        {
            if (request != null)
                request.Abort();
        }

        public async Task<string> SendRequest(string baseUrl, string url, object dataBody, string httpMethod, IDictionary<string, string> cookies)
        {
            try
            {
                request = WebRequest.Create(baseUrl + url) as HttpWebRequest;

                if (request == null)
                    throw new Exception("can not create HttpWebRequest");

                TryAddCookies(request, baseUrl, cookies);

                request.Method = httpMethod;
                request.Accept = "*/*";

                if (dataBody != null)
                {
                    var serialized = GetSerializedData(dataBody);

                    var byteData = new UTF8Encoding().GetBytes(serialized);

                    request.ContentType = @"application/x-www-form-urlencoded";
                    request.ContentLength = byteData.Length;

                    await request.GetRequestStream().WriteAsync(byteData, 0, byteData.Length);
                }

                try
                {
                    var response = await request.GetResponseAsync();

                    string responseText;

                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseText = reader.ReadToEnd();
                    }

                    return responseText;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

        }

        private string GetSerializedData(object dataBody)
        {
            var builder = new StringBuilder();
            foreach (var propertyInfo in dataBody.GetType().GetProperties())
            {
                if (builder.Length != 0)
                    builder.Append("&");
                builder.Append(propertyInfo.Name);
                builder.Append('=');
                builder.Append(propertyInfo.GetValue(dataBody, null));
            }

            return builder.ToString();
        }

        internal HttpWebRequest TryAddCookies(HttpWebRequest request, string baseUrl, IDictionary<string, string> cookies)
        {
            if (request.CookieContainer == null)
                request.CookieContainer = new CookieContainer();

            foreach (var cookie in cookies)
            {
                request.CookieContainer.Add(new Uri(baseUrl), new Cookie(cookie.Key, cookie.Value));
            }

            return request;
        }
    }
}
