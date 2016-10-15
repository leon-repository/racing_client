using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;

namespace BLL
{
    /// <summary>
    /// 访问http服务器类
    /// </summary>
    public static class WebService
    {
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public static CookieContainer CookiesContainer;
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] SendGetRequest(string url)
        {
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }

                request.CookieContainer = CookiesContainer;  //启用cookie
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }

                response.Close();

                return buf;
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("请求url:" + url);
                return null;
            }
        }


        public static string SendGetRequest2(string url,string auth,string acckey)
        {
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.Headers.Add("Client:"+ConfigHelper.GetXElementNodeValue("Client","id"));
                request.Headers.Add("Accesskey:" + acckey);
                request.Headers.Add("Authorization:" + auth);


                request.CookieContainer = CookiesContainer;  //启用cookie
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                //response.Close();

                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("请求url:" + url);
                Log.WriteLogByDate("请求acckey:" + acckey);
                return null;
            }
        }


        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static byte[] SendPostRequest(string url, string body)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.ContentLength = request_body.Length;

                request.Headers.Add("Client:"+ ConfigHelper.GetXElementNodeValue("Client", "id"));

                Stream request_stream = request.GetRequestStream();

                request_stream.Write(request_body, 0, request_body.Length);

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)  //读取返回数据
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0) break;
                    count -= n;
                    offset += n;
                }

                response.Close();

                return buf;
            }
            catch(Exception ex)
            {
                Log.WriteLogByDate("请求url:" + url);
                Log.WriteLogByDate("请求body:" + body);
                return null;
            }
        }


        public static string SendPostRequest2(string url, string body)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.ContentLength = request_body.Length;

                request.Headers.Add("Client:"+ ConfigHelper.GetXElementNodeValue("Client", "id"));

                Stream request_stream = request.GetRequestStream();

                request_stream.Write(request_body, 0, request_body.Length);

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("请求url:" + url);
                Log.WriteLogByDate("请求body:" + body);
                return null;
            }
        }

        public static string SendPostRequest2(string url, string body,string authorization,string acckey)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.ContentLength = request_body.Length;

                request.Headers.Add("Client:"+ ConfigHelper.GetXElementNodeValue("Client", "id"));
                request.Headers.Add("Accesskey:"+acckey);
                request.Headers.Add("Authorization:"+authorization);

                Stream request_stream = request.GetRequestStream();

                request_stream.Write(request_body, 0, request_body.Length);

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("请求url:" + url);
                Log.WriteLogByDate("请求body:" + body);
                Log.WriteLogByDate("请求authorization:" + authorization);
                Log.WriteLogByDate("请求acckey:" + acckey);
                return null;
            }
        }


        public static string SendPutRequest2(string url, string body, string authorization, string acckey)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "put";
                request.ContentLength = request_body.Length;

                request.Headers.Add("Client:" + ConfigHelper.GetXElementNodeValue("Client", "id"));
                request.Headers.Add("Accesskey:" + acckey);
                request.Headers.Add("Authorization:" + authorization);

                Stream request_stream = request.GetRequestStream();

                request_stream.Write(request_body, 0, request_body.Length);

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.WriteLogByDate("请求url:"+ url);
                Log.WriteLogByDate("请求body:" + body);
                Log.WriteLogByDate("请求authorization:" + authorization);
                Log.WriteLogByDate("请求acckey:" + acckey);
                return null;
            }
        }
        /// <summary>
        /// 获取指定cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Cookie GetCookie(string name)
        {
            List<Cookie> cookies = GetAllCookies(CookiesContainer);
            foreach (Cookie c in cookies)
            {
                if (c.Name == name)
                {
                    return c;
                }
            }
            return null;
        }

        private static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();

            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }
    }
}
