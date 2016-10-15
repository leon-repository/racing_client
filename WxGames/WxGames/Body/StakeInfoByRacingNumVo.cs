using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WxGames.Body
{
    public class StakeInfoByRacingNumVo
    {
        public String racingNum { get; set; }

        public int[] result { get; set; }

        public int stakeCount { get; set; }

        public int stakeAmount { get; set; }

        public int incomeAmount { get; set; }

        public int deficitAmount { get; set; }

        public StakeVo stakeVo { get; set; }
    }
}
