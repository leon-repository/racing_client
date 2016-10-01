using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 原始消息列表
    /// </summary>
    public class OriginMsg
    {
        /// <summary>
        /// 消息Id
        /// </summary>
        public string MsgId { get; set; }

        /// <summary>
        /// 发送人UserName
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 接收人UserName
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        public string Status { get; set; }

        public string ImgStatus { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string CreateTime { get; set; }

        public string NewMsgId { get; set; }

        /// <summary>
        /// 发送人昵称
        /// </summary>
        public string FromNickName { get; set; }

        /// <summary>
        /// 发送人Uin
        /// </summary>
        public string FromUin { get; set; }

        /// <summary>
        /// 群昵称
        /// </summary>
        public string QnickName { get; set; }

        /// <summary>
        /// 是否解析
        /// </summary>
        public string IsSucc { get; set; }
    }
}
