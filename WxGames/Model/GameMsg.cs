using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 每期消息表
    /// </summary>
    public class GameMsg
    {
        /// <summary>
        /// uuid
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public int SendTime { get; set; }

        /// <summary>
        /// 消息发送内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否发送
        /// 0,未发送，1，已发送
        /// </summary>
        public string IsSend { get; set; }
    }
}
