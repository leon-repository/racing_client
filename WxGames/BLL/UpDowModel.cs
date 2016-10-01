using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 上下分操作
    /// </summary>
    public class UpDowModel
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 指令内容
        /// </summary>
        public string OrderContect { get; set; }

        /// <summary>
        /// 是否同意：显示文本
        /// </summary>
        public string Succ { get; set; }

        /// <summary>
        /// 是否同意：不显示，存的是ID
        /// </summary>
        public string IsSucc { get; set; }

        /// <summary>
        /// 分数:不显示
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 唯一标识:不显示
        /// </summary>
        public string Uin { get; set; }

        /// <summary>
        /// 消息ID:不显示
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 指令操作
        /// </summary>
        public string CommandOne { get; set; }
    }
}
