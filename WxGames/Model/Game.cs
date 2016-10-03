using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 游戏期数表
    /// </summary>
    public class Game
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// 游戏期数
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 开奖号码
        /// </summary>
        public string GameNum { get; set; }

        /// <summary>
        /// 是否已开奖
        /// 0，未开奖
        /// 1，开奖
        /// </summary>
        public string IsSucc { get; set; }
    }
}
