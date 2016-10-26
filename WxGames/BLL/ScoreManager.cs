using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Model;

namespace BLL
{
    public class ScoreManager
    {
        public static readonly ScoreManager Instance = new ScoreManager();

        private DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

        public List<UpDowModel> GetUpDowModel()
        {
            List<UpDowModel> list = new List<UpDowModel>();

            string sql = "select t.msgId,t.msgFromName as NickName,t.OrderContect,case t.issucc when 0 then '未处理' when 1 then '同意' when 2 then '不同意' else '未知' end as succ,t.IsSucc,t.Score,t.msgFromid as uin,t.CommandOne from nowMsg t where t.IsSucc = 0 and t.CommandType in ('上下查') and isDelete=0 and msgFromid<>'0'";
            list = data.GetListNonTable<UpDowModel>(sql);
            return list;
        }

        /// <summary>
        /// 获取账单列表
        /// </summary>
        /// <returns></returns>
        public List<ZhanDan> GetZhanDan()
        {
            List<ZhanDan> list = new List<ZhanDan>();
            string sql = "select t.nickname,t.uin,t.totalScore from contactScore t";
            list = data.GetListNonTable<ZhanDan>(sql);
           
            //计算上期盈亏

            return list;
        }

        /// <summary>
        /// 上分操作
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool UpScore(string uin, int score)
        {
            List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
            pkList.Add(new KeyValuePair<string, object>("Uin", uin));
            ContactScore contactScore = data.First<ContactScore>(pkList, "");
            if (contactScore == null)
            {
                Log.WriteLogByDate("uin不存在Contact表中：" + uin);
                return false;
            }
            else
            {
                contactScore.TotalScore = contactScore.TotalScore + score;
            }

            return data.Update<ContactScore>(contactScore, pkList, "") > 0;
        }

        /// <summary>
        /// 下分操作
        /// </summary>
        /// <param name="uin"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool DownScore(string uin, int score)
        {
            List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
            pkList.Add(new KeyValuePair<string, object>("Uin", uin));
            ContactScore contactScore = data.First<ContactScore>(pkList, "");
            if (contactScore == null)
            {
                Log.WriteLogByDate("uin不存在Contact表中：" + uin);
                return false;
            }
            else
            {
                contactScore.TotalScore = contactScore.TotalScore - score;
                if (contactScore.TotalScore <= 0)
                {
                    Log.WriteLogByDate("下分失败，请核对总分");
                    return false;
                }
            }

            return data.Update<ContactScore>(contactScore, pkList, "") > 0;
        }

        public bool UpdateScore(string uin, int score)
        {
            List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
            pkList.Add(new KeyValuePair<string, object>("Uin", uin));
            ContactScore contactScore = data.First<ContactScore>(pkList, "");
            if (contactScore == null)
            {
                Log.WriteLogByDate("uin不存在Contact表中：" + uin);
                return false;
            }
            else
            {
                contactScore.TotalScore = score;
                if (contactScore.TotalScore <= 0)
                {
                    Log.WriteLogByDate("改分失败，分数不能为负数");
                    return false;
                }
            }

            return data.Update<ContactScore>(contactScore, pkList, "") > 0;
        }
    }
}
