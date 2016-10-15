using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WxGames
{
    /// <summary>
    /// 微信客户端数据
    /// </summary>
    public class Token
    {
       public List<Cookie> CookList { get; set; }

       public CookieContainer Container { get; set;}
    
       public string SKey { get; set; }

       public string Pass_Ticket { get; set; }
    }
}
