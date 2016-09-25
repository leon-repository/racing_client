using BLL;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WxGames.HTTP;

namespace WxGames
{
    public partial class frmMainForm : Form
    {
        public frmMainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static WXUser CurrentWX;

        /// <summary>
        /// 当前群userName
        /// </summary>
        public static string CurrentQun;

        /// <summary>
        /// 是否开始任务
        /// </summary>
        public static bool Start = false;

        /// <summary>
        /// 微信服务
        /// </summary>
        public static WXService wxs = new WXService();

        private void frmMainForm_Load(object sender, EventArgs e)
        {
            ///请求服务器获取盘口数据
            List<LoginModel> list = new List<LoginModel>();
            list.Add(new LoginModel() { PankouId = "1", PankouName = "盘口A", IsLogin = 0, IsSum = 0 });
            list.Add(new LoginModel() { PankouId = "2", PankouName = "盘口B", IsLogin = 0, IsSum = 0 });

            SystemContext context = new SystemContext();
            foreach (LoginModel login in list)
            {
                if (context.Logins.Select(p => p.PankouId == login.PankouId).ToList().Count > 0)
                {
                    context.Logins.Attach(login);
                }
                else
                {
                    context.Logins.Add(login);
                }
            }
            context.SaveChanges();
            cmbPankou.DataSource = list;
            cmbPankou.DisplayMember = "PankouName";
            cmbPankou.ValueMember = "PankouId";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = cmbPankou.SelectedValue.ToString();
            string pwd = txtPwd.Text;

            if (PanKou.Instance.Login(userName, pwd))
            {
                //并将微信的用户列表，群列表存放在系统数据库中
                JObject init_result = wxs.WxInit();  //初始化

                List<object> contact_all = new List<object>();
                if (init_result != null)
                {
                    CurrentWX = new WXUser();
                    CurrentWX.UserName = init_result["User"]["UserName"].ToString();
                    CurrentWX.City = "";
                    CurrentWX.HeadImgUrl = init_result["User"]["HeadImgUrl"].ToString();
                    CurrentWX.NickName = init_result["User"]["NickName"].ToString();
                    CurrentWX.Province = "";
                    CurrentWX.PYQuanPin = init_result["User"]["PYQuanPin"].ToString();
                    CurrentWX.RemarkName = init_result["User"]["RemarkName"].ToString();
                    CurrentWX.RemarkPYQuanPin = init_result["User"]["RemarkPYQuanPin"].ToString();
                    CurrentWX.Sex = init_result["User"]["Sex"].ToString();
                    CurrentWX.Signature = init_result["User"]["Signature"].ToString();

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

                            contact_all.Add(user);
                        }
                    }

                    //获取微信群，并存到数据库
                    //删除旧数据
                    SystemContext context = new SystemContext();
                    context.Database.ExecuteSqlCommand("delete from quntb");
                    foreach (WXUser user in contact_all)
                    {
                        if (user.UserName.Contains("@@"))
                        {
                            QunTb model = new QunTb();
                            model.UserName = user.UserName;
                            model.NickName = user.NickName;
                            model.IsStart = 0;
                            context.QunTbs.Add(model);
                        }
                    }
                    context.SaveChanges();

                    tab.SelectedIndex = 1;
                    //查找盘口数据是否存在，不存在就创建一个
                    cmbQun.DataSource = context.QunTbs.ToList<QunTb>();
                    cmbQun.DisplayMember = "NickName";
                    cmbQun.ValueMember = "UserName";
                }
            }
            else
            {
                MessageBox.Show("登陆失败");
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Start == false)
            {
                Start = true;

                btnStart.Text = "结束";

                CurrentQun = cmbQun.SelectedValue.ToString();
                TaskManager.Instance.Start(true);
                //开始定时处理消息
                //开始更新dataGridView数据表
            }
            else
            {
                Start = false;
                btnStart.Text = "开始";
                TaskManager.Instance.Start(false);
            }

                ((Action)(delegate ()
            {

                //开始定时扫描接收消息
                string sync_flag = "";
                while (true)
                {
                    sync_flag = wxs.WxSyncCheck();  //同步检查
                    //if (sync_flag == null)
                    //{
                    //    continue;
                    //}
                    ////这里应该判断 sync_flag中selector的值
                    //else //有消息
                    //{
                    //    sync_result = wxs.WxSync();  //进行同步
                    //    if (sync_result != null)
                    //    {
                    //        if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
                    //        {
                    //            foreach (JObject m in sync_result["AddMsgList"])
                    //            {
                    //                string from = m["FromUserName"].ToString();
                    //                string to = m["ToUserName"].ToString();
                    //                string content = m["Content"].ToString();
                    //                string type = m["MsgType"].ToString();

                    //                WXMsg msg = new WXMsg();
                    //                msg.From = from;
                    //                msg.Msg = type == "1" ? content : "请在其他设备上查看消息";  //只接受文本消息
                    //                msg.Readed = false;
                    //                msg.Time = DateTime.Now;
                    //                msg.To = to;
                    //                msg.Type = int.Parse(type);

                    //                if (msg.Type == 51)  //屏蔽一些系统数据
                    //                {
                    //                    continue;
                    //                }
                    //                //string uin = m["Uin"].ToString();
                    //                //string nickName = m["NickName"].ToString();

                    //                StringBuilder msgContext = new StringBuilder(string.Format("@{0} 你的uid是{1},发送的内容是{2}", "", "1", content));
                    //                if (from == frmMainForm.CurrentQun)
                    //                {
                    //                    WXMsg msgTo = new WXMsg() { From = frmMainForm.CurrentWX.UserName, Msg = msgContext.ToString(), To = frmMainForm.CurrentQun, Type = 1, Readed = false, Time = DateTime.Now };
                    //                    frmMainForm.CurrentWX.SendMsg(msgTo, false);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    //System.Threading.Thread.Sleep(1);
                }

            })).BeginInvoke(null, null);
        }
    }
}

