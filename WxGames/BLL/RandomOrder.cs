using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class RandomOrder
    {
        public static readonly RandomOrder Instance = new RandomOrder();

        public string GetRandomOrder()
        {
            //生成 名次
            Random random = new Random();
            string commandOne = random.Next(1, 10).ToString();

            //生成 车
            Random random2 = new Random();
            string commandTwo = random2.Next(1, 10).ToString();

            //生成积分
            Random random3 = new Random();
            List<string> list = new List<string>();
            list.Add("5");
            list.Add("10");
            list.Add("20");
            int n=random3.Next(0, 2);
            string commandThree = list[n];

            return commandOne + "/" + commandTwo + "/" + commandThree;
        }
    }
}
