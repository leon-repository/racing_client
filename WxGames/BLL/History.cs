using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class History
    {
        /// <summary>
        /// 发送人昵称
        /// </summary>
        public string MsgFromName { get; set; }

        /// <summary>
        /// 原指令
        /// </summary>
        public string OrderContect { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 当前期数
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// 消息处理时间
        /// </summary>
        public string OpDate { get; set; }
    }
}
