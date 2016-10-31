using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class OrderFirstCheck
    {
        public static readonly OrderFirstCheck Instance = new OrderFirstCheck();

        /// <summary>
        /// 检查是否是上下查
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckOne(string msg)
        {
            if (msg == "查" || msg == "查分")
            {
                return true;
            }

            if (msg.Contains("上分"))
            {
                int msgOne = msg.Replace("上分", "").ToInt();
                if (msgOne >= 1)
                {
                    return true;
                }
                return false;
            }
            else if (msg.Contains("上"))
            {
                int msgTow = msg.Replace("上", "").ToInt();
                if (msgTow >= 1)
                {
                    return true;
                }
                return false;
            }
            else if (msg.Contains("下分"))
            {
                int msgTh = msg.Replace("下分", "").ToInt();
                if (msgTh >= 1)
                {
                    return true;
                }
                return false;
            }
            else if (msg.Contains("下"))
            {
                int msg5 = msg.Replace("下", "").ToInt();
                if (msg5 >= 1)
                {
                    return true;
                }
                return false;
            }


            return false;
        }
    }
}
