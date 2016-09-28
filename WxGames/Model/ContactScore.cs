using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 联系人积分表
    /// </summary>
    public class ContactScore
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public string Uuid { get; set; }

        /// <summary>
        /// 微信ID
        /// </summary>
        public string Uin { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 总积分
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// 剩余积分
        /// </summary>
        public int SyScore { get; set; }

        /// <summary>
        /// 进行中的积分
        /// </summary>
        public int RunScore { get; set; }

    }
}
