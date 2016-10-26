using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class Order
    {
       /// <summary>
       /// 原指令内容
       /// </summary>
       public string OrderContent { get; set; }

        /// <summary>
        /// 指令第一段
        /// </summary>
       public string CommandOne { get; set; }

        /// <summary>
        /// 指令第二段(可为空)
        /// </summary>
        public string CommandTwo { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 指令类型
        /// </summary>
        public OrderType CommandType { get; set; }
    }

    public enum OrderType
    {
        上下查=1,
        名次大小单双龙虎=2,
        买名次=3,
        冠亚和=4,
        取消=5,
        指令格式错误=6,
        下注积分范围错误=7,
        和大,
        和小,
        和单,
        和双
    }
}
