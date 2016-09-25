using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WxGames
{
    public class PanKou
    {
        public static readonly PanKou Instance = new PanKou();

        public bool IsLogin()
        {
            //读取数据库

            return true;
        }

        public bool Login(string userName, string password)
        {
            //验证用户名密码

            return true;
        }

        public List<string> PankouList()
        {
            List<string> list = new List<string>();
            list.Add("盘口：上海");
            list.Add("盘口：北京");

            return list;
        }
    }
}
