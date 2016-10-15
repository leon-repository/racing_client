using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 演员表
    /// </summary>
    public class Performer
    {
        public string Uuid { get; set; }

        public string Uin { get; set; }

        public string UserName { get; set; }

        public string PassTick { get; set; }

        public string Skey { get; set; }

        /// <summary>
        /// 是否使用
        /// 0，使用
        /// 1，未使用
        /// </summary>
        public string IsSucc { get; set; }
    }
}
