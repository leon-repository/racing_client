using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 账单列表
    /// </summary>
    public class ZhanDan
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// Uin
        /// </summary>
        public string Uin { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public string TotalScore { get; set; }

        /// <summary>
        /// 上期盈亏
        /// </summary>
        public string LastScore { get; set; }
    }
}
