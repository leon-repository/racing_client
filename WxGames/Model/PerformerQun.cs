using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 演员的群信息表
    /// </summary>
    public class PerformerQun
    {
        public string Uuid { get; set; }

        public string Uin { get; set; }

        /// <summary>
        /// 群昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 群UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 是否开始
        /// </summary>
        public int IsStart { get; set; }
    }
}
