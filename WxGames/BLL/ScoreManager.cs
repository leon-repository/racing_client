using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

namespace BLL
{
    public class ScoreManager
    {
        public static readonly ScoreManager Instance = new ScoreManager();

        public List<UpDowModel> GetUpDowModel()
        {
            List<UpDowModel> list = new List<UpDowModel>();
            string conn = ConfigurationManager.AppSettings["conn"].ToString();

            DataHelper data = new DataHelper(conn);
            string sql = "select * from nowMsg t where t.IsSucc=0";
            list = data.GetListNonTable<UpDowModel>(sql);
            

            return list;
        }
    }
}
