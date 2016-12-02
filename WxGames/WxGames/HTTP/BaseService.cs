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
using System.Text.RegularExpressions;

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
                    Log.WriteLogByDate(setCookie);
                    if (setCookie != null)
                    {
                        if (setCookie.Contains("wx.qq.com"))
                        {
                            //CookiesContainer.SetCookies(new Uri("https://wx.qq.com"), setCookie);
                            CookiesContainer.Add(GetAllCookiesFromHeader(setCookie, "https://wx.qq.com"));

                        }
                        if (setCookie.Contains("wx2.qq.com"))
                        {
                            // CookiesContainer.SetCookies(new Uri("https://wx2.qq.com"), setCookie.ToString());
                            //CookiesContainer.SetCookies(new Uri("https://wx2.qq.com"), "webwx_data_ticket=gSdewl4ryJSJOY8TaCLXgXQ/; Domain=.qq.com; Path=/; Expires=Sat, 03-Dec-2016 02:47:24 GMT");
                            CookiesContainer.Add(GetAllCookiesFromHeader(setCookie, "https://wx2.qq.com"));
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
                List<Cookie> list=GetAllCookies(CookiesContainer);

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
            try
            {
                Log.WriteLogByDate("参数列表：url=" + url + ",file=" + file + ",paramName=" + paramName + ",contentType=" + contentType + ",");

                string result = string.Empty;
                Log.WriteLogByDate("执行1");
                string boundary = "----WebKitFormBoundary4506GCImvIQDt1jF";
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                Log.WriteLogByDate("执行2");
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                Log.WriteLogByDate("执行3");
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

                if (CookiesContainer == null)
                {
                    CookiesContainer = new CookieContainer();
                }
                wr.CookieContainer = CookiesContainer;  //启用cookie
                Log.WriteLogByDate("执行4");
                Stream rs = wr.GetRequestStream();
                Log.WriteLogByDate("执行5");
                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in nvc.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }

                Log.WriteLogByDate("执行6");
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                Log.WriteLogByDate("执行7");
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "filename", file, contentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                Log.WriteLogByDate("执行8");
                rs.Write(headerbytes, 0, headerbytes.Length);
                Log.WriteLogByDate("执行9");
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                Log.WriteLogByDate("执行10");
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                Log.WriteLogByDate("执行11");
                fileStream.Close();
                Log.WriteLogByDate("执行12");
                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                Log.WriteLogByDate("执行13");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();
                Log.WriteLogByDate("执行14");
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
            catch (Exception ex)
            {
                Log.WriteLogByDate("发生图片：发生异常");
                Log.WriteLog(ex);
                return "";
            }
            
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

        public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
        {
            ArrayList al = new ArrayList();
            CookieCollection cc = new CookieCollection();
            if (strHeader != string.Empty)
            {
                al = ConvertCookieHeaderToArrayList(strHeader);
                cc = ConvertCookieArraysToCookieCollection(al, strHost);
            }
            return cc;
        }

        private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
        {
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            string[] strCookTemp = strCookHeader.Split(',');
            ArrayList al = new ArrayList();
            int i = 0;
            int n = strCookTemp.Length;
            while (i < n)
            {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                }
                else
                {
                    al.Add(strCookTemp[i]);
                }
                i = i + 1;
            }
            return al;
        }

        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
        {
            CookieCollection cc = new CookieCollection();

            int alcount = al.Count;
            string strEachCook;
            string[] strEachCookParts;
            for (int i = 0; i < alcount; i++)
            {
                strEachCook = al[i].ToString();
                strEachCookParts = strEachCook.Split(';');
                int intEachCookPartsCount = strEachCookParts.Length;
                string strCNameAndCValue = string.Empty;
                string strPNameAndPValue = string.Empty;
                string strDNameAndDValue = string.Empty;
                string[] NameValuePairTemp;
                Cookie cookTemp = new Cookie();

                for (int j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            int firstEqual = strCNameAndCValue.IndexOf("=");
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            NameValuePairTemp = strPNameAndPValue.Split('=');
                            if (NameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Path = NameValuePairTemp[1];
                            }
                            else
                            {
                                cookTemp.Path = "/";
                            }
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            NameValuePairTemp = strPNameAndPValue.Split('=');

                            if (NameValuePairTemp[1] != string.Empty)
                            {
                                cookTemp.Domain = NameValuePairTemp[1];
                            }
                            else
                            {
                                cookTemp.Domain = strHost;
                            }
                        }
                        continue;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }
                cookTemp.Expires = DateTime.Now.AddDays(1);
                cc.Add(cookTemp);
            }
            return cc;
        }
    }
}
