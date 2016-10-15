using BLL;
using Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using WxGames.HTTP;

namespace WxGames.Job
{
    class PerformJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
            List<Performer> performerList = new List<Performer>();
            performerList = data.GetList<Performer>(" isSucc='0' ", "");

            foreach (Performer performer in performerList)
            {
                List<CookieTable> cookieList = data.GetList<CookieTable>(string.Format("uin='{0}'",performer.Uin), "");
                List<Cookie> cookies = new List<Cookie>();
                foreach (CookieTable cookietb in cookieList)
                {
                    Cookie cookie = new Cookie();
                    cookie.Comment = cookietb.Comment;
                    cookie.Discard =Convert.ToBoolean(cookietb.Discard);
                    cookie.Domain = cookietb.Domain;
                    cookie.Expired = Convert.ToBoolean(cookietb.Expired);
                    cookie.HttpOnly = Convert.ToBoolean(cookietb.HttpOnly);
                    cookie.Name = cookietb.Name;
                    cookie.Path = cookietb.Path;
                    cookie.Port = cookietb.Port;
                    cookie.Secure = Convert.ToBoolean(cookietb.Secure);
                    //cookie.TimeStamp = ((long)cookietb.TimeStamp.ToInt()).UnixTimestampToDateTime(DateTimeKind.Local);
                    cookie.Value = cookietb.Value;
                    cookie.Version = cookietb.Version.ToInt();
                    cookies.Add(cookie);

                }
                List<PerformerQun> list = data.GetList<PerformerQun>("uin=" + performer.Uin, "");
                string to = list.FirstOrDefault(p => p.NickName == frmMainForm.CurrentQunNick).UserName;

                PerFormerService service = new PerFormerService();
                
                //编写托发消息的逻辑
                //先检查能否下注
                if (frmMainForm.IsContinue)
                {
                    service.SendMsg(RandomOrder.Instance.GetRandomOrder(), performer.UserName, to, 1, cookies, performer.Skey, performer.PassTick);
                }
            }

        }
    }
}
