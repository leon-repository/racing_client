using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WxGames.Body
{
    public class StakeVo
    {
        public String racingNum { get; set; }

        /**
         * 指定车号的名次押注情况，有且仅有有10条记录
         */
        public List<AppointStake> appointStakeList { get; set; }

        /**
         * 常规押注，1~5龙虎情况，冠亚和大小单双，冠亚和指定
         */
        public CommonStake commonStake { get; set; }

        /**
         * 名次情况的大小单双押注
         */
        public List<RankingStake> rankingStakeList { get; set; }
    }
}
