using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WxGames.Body
{
    public class MemberStake
    {
        public string id { get; set; }

        public string membersId { get; set; }

        public string racingNum { get; set; }

        /// <summary>
        /// 总押注金额
        /// </summary>
        public double totalStakeAmount { get; set; }

        /// <summary>
        /// 开奖后的收入金额
        /// </summary>
        public double totalIncomeAmount { get; set; }

        /// <summary>
        /// 盈亏
        /// </summary>
        public double totalDeficitAmount { get; set; }

        /// <summary>
        /// 总押注数
        /// </summary>
        public int totalStakeCount { get; set; }

        public bool isComplateStatistics { get; set; }

        public string createTime { get; set; }

    }
}
