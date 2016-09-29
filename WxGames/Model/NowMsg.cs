using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 消息解析表
    /// </summary>
    public class NowMsg
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [Key]
        public string MsgId { get; set; }

        /// <summary>
        /// 发送人uin
        /// </summary>
        public string MsgFromId { get; set; }

        /// <summary>
        /// 发送人昵称
        /// </summary>
        public string MsgFromName { get; set; }

        /// <summary>
        /// 接受人ID
        /// </summary>
        public string ReciveId { get; set; }

        /// <summary>
        /// 接收人昵称
        /// </summary>
        public string ReciveName { get; set; }

        /// <summary>
        /// json指令
        /// </summary>
        public string MsgContent { get; set; }

        /// <summary>
        /// 原指令
        /// </summary>
        public string OrderContect { get; set; }

        /// <summary>
        /// 指令第一段
        /// </summary>
        public string CommandOne { get; set; }

        /// <summary>
        /// 指令第二段
        /// </summary>
        public string CommandTwo { get; set; }

        /// <summary>
        /// 指令分数
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 指令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 消息创建日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 消息处理时间
        /// </summary>
        public string OpDate { get; set; }

        /// <summary>
        /// 是否同意
        /// </summary>
        public int IsSucc { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public string IsDelete { get; set; }

        /// <summary>
        /// 消息类型
        /// 上下查=1,
        ///名次大小单双龙虎=2,
        ///买名次=3,
        ///冠亚和=4,
        ///取消=5,
        ///指令格式错误=6
        /// </summary>
        public string IsMsg { get; set; }

        /// <summary>
        /// 是否处理
        /// 0未处理，1处理
        /// </summary>
        public string IsDeal { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 当前期数
        /// </summary>
        public string Period { get; set; }
    }
}
