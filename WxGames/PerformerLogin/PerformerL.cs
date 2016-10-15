using BLL;
using Model;
using Newtonsoft.Json.Linq;
using PerformerLogin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using WxGames.HTTP;

namespace WxGames
{
    public partial class PerformerL : Form
    {
        private DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

        public PerformerL()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            picLogin.Image = null;
            picLogin.SizeMode = PictureBoxSizeMode.Zoom;

            ((Action)delegate ()
            {
                //异步加载二维码
                WxGames.HTTP.LoginService2 ls = new WxGames.HTTP.LoginService2();
                Image qrcode = ls.GetQRCode();
                if (qrcode != null)
                {
                    this.BeginInvoke((Action)delegate() {
                        picLogin.Image = qrcode;
                    });

                    object login_result = null;

                    while (true) //循环判断手机扫面二维码结果
                    {
                        login_result = ls.LoginCheck();
                        if (login_result is Image)//已扫描 未登录
                        {
                            this.BeginInvoke((Action)delegate ()
                            {
                                picLogin.SizeMode = PictureBoxSizeMode.CenterImage;
                                picLogin.Image = login_result as Image;
                            });
                        }
                        if (login_result is string)//已完成登录
                        {
                            //访问登录跳转URL
                            ls.GetSidUid(login_result as string);

                            //微信初始化，获取userName写入演员表
                            WXService2 wxs = new WXService2();
                            JObject init_result = wxs.WxInit();  //初始化

                            if (init_result != null)
                            {
                                string userName = init_result["User"]["UserName"].ToString();
                                string nickName = init_result["User"]["NickName"].ToString();
                                string uin = init_result["User"]["Uin"].ToString();

                                if (string.IsNullOrEmpty(userName))
                                {
                                    MessageBox.Show("关闭重新登陆");
                                    return;
                                }

                                //将数据写入数据库
                                Performer performer = new Performer();
                                performer.Uuid = Guid.NewGuid().ToString();
                                performer.IsSucc = "0";
                                performer.UserName = userName;
                                performer.Uin = uin;
                                performer.PassTick = LoginService2.Pass_Ticket;
                                performer.Skey = LoginService2.SKey;

                                data.ExecuteSql(string.Format("delete from performer where uin='{0}'", uin));
                                data.Insert<Performer>(performer, "");

                                data.ExecuteSql(string.Format("delete from CookieTable where uin='{0}'", uin));
                                foreach (Cookie cookie in wxs.GetCookList())
                                {
                                    CookieTable cookieTb = new CookieTable();
                                    cookieTb.Uuid = Guid.NewGuid().ToString();
                                    cookieTb.Comment = cookie.Comment.ToString();
                                    cookieTb.Discard = cookie.Discard.ToString();
                                    cookieTb.Domain = cookie.Domain.ToString();
                                    cookieTb.Expired = cookie.Expired.ToString();
                                    cookieTb.HttpOnly = cookie.HttpOnly.ToString();
                                    cookieTb.Name = cookie.Name.ToString();
                                    cookieTb.Path = cookie.Path.ToString();
                                    cookieTb.Port = cookie.Port.ToString();
                                    cookieTb.Secure = cookie.Secure.ToString();
                                    cookieTb.TimeStamp = cookie.TimeStamp.DateTimeToUnixTimestamp().ToString();
                                    cookieTb.Value = cookie.Value.ToString();
                                    cookieTb.Version = cookie.Version.ToString();
                                    cookieTb.Uin = uin;
                                    data.Insert<CookieTable>(cookieTb, "");
                                }


                                data.ExecuteSql("delete from PerformerQun where uin="+uin);
                                JObject contact_result = wxs.GetContact(); //通讯录
                                if (contact_result != null)
                                {
                                    foreach (JObject contact in contact_result["MemberList"])  //完整好友名单
                                    {
                                        WXUser user = new WXUser();
                                        user.UserName = contact["UserName"].ToString();
                                        user.City = contact["City"].ToString();
                                        user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                                        user.NickName = contact["NickName"].ToString();
                                        user.Province = contact["Province"].ToString();
                                        user.PYQuanPin = contact["PYQuanPin"].ToString();
                                        user.RemarkName = contact["RemarkName"].ToString();
                                        user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                                        user.Sex = contact["Sex"].ToString();
                                        user.Signature = contact["Signature"].ToString();

                                        if (user.UserName.Contains("@@"))
                                        {
                                            PerformerQun model = new PerformerQun();
                                            model.Uuid = Guid.NewGuid().ToString();
                                            model.UserName = user.UserName;
                                            model.NickName = user.NickName;
                                            model.Uin = uin;
                                            model.IsStart = 0;
                                            data.Insert<PerformerQun>(model, "");
                                        }

                                    }
                                }

                                List<PerformerQun> list=data.GetList<PerformerQun>("", "");
                                List<CookieTable> listCookieTable = data.GetList<CookieTable>("uin=" + uin, "");
                                List<Cookie> listCookie = new List<Cookie>();
                                foreach (CookieTable item in listCookieTable)
                                {
                                    Cookie cookie = new Cookie();
                                    cookie.Comment = item.Comment;
                                    //cookie.CommentUri
                                    cookie.Discard =Convert.ToBoolean(item.Discard);
                                    cookie.Domain = item.Domain;
                                    cookie.Expired = Convert.ToBoolean(item.Expired);
                                    //cookie.Expires=item
                                    cookie.HttpOnly =Convert.ToBoolean(item.HttpOnly);
                                    cookie.Name = item.Name;
                                    cookie.Path = item.Path;
                                    cookie.Port = item.Port;
                                    cookie.Secure = Convert.ToBoolean(item.Secure);
                                    //cookie.TimeStamp = Convert.ToInt64(item.TimeStamp).UnixTimestampToDateTime(DateTimeKind.Local);
                                    cookie.Value = item.Value;
                                    cookie.Version = Convert.ToInt32(item.Version);
                                    listCookie.Add(cookie);
                                }


                                //PerFormerService service = new PerFormerService();
                                //string to = list.FirstOrDefault(p => p.NickName == "当机游戏,苏子轩,晓明").UserName;
                                //service.SendMsg("托发送消息", userName, to,1, listCookie, performer.Skey, performer.PassTick);

                                MessageBox.Show("托登陆成功");
                            }
                        }
                    }
                }

            }).BeginInvoke(null, null);
        }
    }
}
