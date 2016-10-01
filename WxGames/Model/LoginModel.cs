using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 登陆信息表
    /// </summary>
    public class LoginModel
    {
        /// <summary>
        /// 盘口ID
        /// </summary>
        public string PankouId { get; set; }

        /// <summary>
        /// 盘口名称
        /// </summary>
        public string PankouName { get; set; }

        /// <summary>
        /// 是否登陆
        /// 0，未登录1，登陆
        /// </summary>
        public int IsLogin { get; set; }

        /// <summary>
        /// 是否结算
        /// 0,未结算1，结算
        /// </summary>
        public int IsSum { get; set; }
    }
}
