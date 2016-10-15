using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace WxGames
{
    /// <summary>
    /// 访问http服务器类
    /// </summary>
    public class HttpService
    {
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public CookieContainer CookiesContainer;
        /// <summary>
        /// 向服务器发送get请求  返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public byte[] SendGetRequest(string url,CookieContainer cookieContainers)
        {

            try
            {
                GC.Collect();
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.101 Safari/537.36";
                request.CookieContainer = cookieContainers;  //启用cookie
                CookiesContainer = cookieContainers;

                request.Timeout = int.MaxValue;
               
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
                //request.Abort();
                //response.Close();
                //response_stream.Close();
                return buf;
            }
            catch (TimeoutException ex)
            {
                return null;
            }
           catch (WebException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public byte[] SendGetRequest2(string url, CookieContainer cookieContainers)
        //{
        //    WebClient client = new WebClient();
        //    client.
        //}


        /// <summary>
        /// 向服务器发送post请求 返回服务器回复数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public byte[] SendPostRequest(string url, string body)
        {
            try
            {
                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.Headers.Add(HttpRequestHeader.UserAgent.ToString(), "Mozilla/5.0 (X11; Linux i686; U;) Gecko/20070322 Kazehakase/0.4.5");
                request.Method = "post";
                request.ContentLength = request_body.Length;
                request.KeepAlive = false;

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
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 获取指定cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Cookie GetCookie(string name,CookieContainer cookieContainer)
        {
            List<Cookie> cookies = GetAllCookies(cookieContainer);
            foreach (Cookie c in cookies)
            {
                if (c.Name == name)
                {
                    return c;
                }
            }
            return null;
        }

        public List<Cookie> GetAllCookies(CookieContainer cc)
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
