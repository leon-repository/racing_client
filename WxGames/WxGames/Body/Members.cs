using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WxGames.Body
{
    public class Members
    {
        //\"id\" : 3,\"userId\" : 5,\"wechatClientId\" : null,\"wechatSn\" : \"2039521281\",\"nickName\" : \"当机游戏\",\"points\" : 1090.00

        public int id { get; set; }

        public int userId { get; set; }

        public string wechatClientId { get; set; }

        /// <summary>
        /// 微信uin
        /// </summary>
        public string  wechatSn { get; set; }

        public string nickName { get; set; }

        /// <summary>
        /// 微信用户积分
        /// </summary>
        public double points { get; set; }
    }
}
