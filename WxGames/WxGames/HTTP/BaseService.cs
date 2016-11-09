using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections;
using BLL;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

namespace WxGames.HTTP
{
    /// <summary>
    /// 访问http服务器类
    /// </summary>
    static class BaseService
    {
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public static CookieContainer CookiesContainer;

        public static byte[] SendGetRequest(string url)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";
                //if (CookiesContainer != null)
                //{
                //    request.CookieContainer = CookiesContainer;
                //}
                //else
                //{
                //    request.CookieContainer = new CookieContainer();
                //}
                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream response_stream = response.GetResponseStream();

                //输出cookieContainer
                //Log.WriteLogByDate("GetUrl:" + url);
                //List<Cookie> list = GetAllCookies(CookiesContainer);
                //foreach (Cookie item in list)
                //{
                //    Log.WriteLogByDate("Name:" + item.Name + "  Value: " + item.Value);
                //}

                ////输出header
                //WebHeaderCollection headers = request.Headers;

                //foreach (var key in headers.AllKeys)
                //{
                //    Log.WriteLogByDate("Header: key=" + key + " value:" + headers.GetValues(key)[0]);
                //}


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
                return buf;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
                return null;
            }
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
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
                //Log.WriteLogByDate("调用URL:" + url);
                //Log.WriteLogByDate(body);

                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.Headers.Add(HttpRequestHeader.UserAgent.ToString(), "Mozilla/5.0 (X11; Linux i686; U;) Gecko/20070322 Kazehakase/0.4.5");
                request.Method = "post";
                request.ContentLength = request_body.Length;

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

                //response.Close();

                return buf;
            }
            catch
            {
                return null;
            }
        }


        public static byte[] SendPostRequestAndSetCookies(string url, string body)
        {
            try
            {
                //Log.WriteLogByDate("调用URL:" + url);
                //Log.WriteLogByDate(body);

                byte[] request_body = Encoding.UTF8.GetBytes(body);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.Headers.Add(HttpRequestHeader.UserAgent.ToString(), "Mozilla/5.0 (X11; Linux i686; U;) Gecko/20070322 Kazehakase/0.4.5");
                request.Method = "post";
                request.ContentLength = request_body.Length;

                Stream request_stream = request.GetRequestStream();

                request_stream.Write(request_body, 0, request_body.Length);

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                    CookiesContainer.Add(response.Cookies);
                }

                //foreach (Cookie cookie in response.Cookies)
                //{
                //    CookiesContainer.SetCookies(cookie.CommentUri);
                //}


                if (response.StatusCode.ToString() == "OK")
                {
                    string setCookie = response.Headers.Get("Set-Cookie");

                    if (setCookie != null)
                    {
                        if (setCookie.Contains("wx.qq.com"))
                        {
                            CookiesContainer.SetCookies(new Uri("https://wx.qq.com"), setCookie);
                        }
                        if (setCookie.Contains("wx2.qq.com"))
                        {
                            CookiesContainer.SetCookies(new Uri("https://wx2.qq.com"), setCookie);
                        }
                    }
                }

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

                //response.Close();

                return buf;
            }
            catch(Exception ex)
            {
                Log.WriteLog(ex);
                return null;
            }
        }


        public static string HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string result = string.Empty;
            string boundary = "----WebKitFormBoundary4506GCImvIQDt1jF";
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            if (CookiesContainer == null)
            {
                CookiesContainer = new CookieContainer();
            }
            wr.CookieContainer = CookiesContainer;  //启用cookie

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "filename", file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {

                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);

                result = reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            return result;
        }


        public static byte[] SendGetRequestAndSetCookies(string url)
        {
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
                System.Net.ServicePointManager.DefaultConnectionLimit = 512;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "get";
                //if (CookiesContainer != null)
                //{
                //    request.CookieContainer = CookiesContainer;
                //}
                //else
                //{
                //    request.CookieContainer = new CookieContainer();
                //}

                //request.CookieContainer = CookiesContainer;  //启用cookie

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }

                request.CookieContainer = CookiesContainer;  //启用cookie

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                    CookiesContainer.Add(response.Cookies); ;
                }

                Stream response_stream = response.GetResponseStream();

                ////输出cookieContainer
                //Log.WriteLogByDate("GetUrl:" + url);
                //List<Cookie> list = GetAllCookies(CookiesContainer);
                //foreach (Cookie item in list)
                //{
                //    Log.WriteLogByDate("Name:" + item.Name + "  Value: " + item.Value);
                //}

                ////输出header
                //WebHeaderCollection headers = request.Headers;

                //foreach (var key in headers.AllKeys)
                //{
                //    Log.WriteLogByDate("Header: key=" + key + " value:" + headers.GetValues(key)[0]);
                //}


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
                return buf;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
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
            if (cc == null)
            {
                //throw new Exception("系统错误，请重新登陆");
            }
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
