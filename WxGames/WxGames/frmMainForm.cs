using BLL;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WxGames.HTTP;
using WxGames.Body;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.IO;

namespace WxGames
{
    public partial class frmMainForm : Form
    {
        SynchronizationContext syncContext = null;

        public frmMainForm()
        {
            InitializeComponent();
            syncContext = WindowsFormsSynchronizationContext.Current;
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
        /// 当前群昵称
        /// </summary>
        public static string CurrentQunNick;

        /// <summary>
        /// 任务开始状态
        /// false:关闭状态
        /// true:运行状态
        /// </summary>
        public static bool Start = false;

        /// <summary>
        /// 微信服务
        /// </summary>
        public static WXService wxs = new WXService();

        /// <summary>
        /// 当前期号
        /// </summary>
        public static string Perioid;

        /// <summary>
        /// 当前选中行
        /// </summary>
        public int CurrentRow = 0;

        /// <summary>
        /// 账单选中行
        /// </summary>
        public int CurrentZDRow = 0;

        public bool IsLogin = false;


        public static bool IsAllowDown = false;

        /// <summary>
        /// 是否接单
        /// </summary>
        public static bool IsJieDan = false;

        /// <summary>
        /// 是否开奖
        /// </summary>
        public static bool IsKaiJian = false;

        /// <summary>
        /// 是否封盘
        /// </summary>
        public static bool IsFengPan = false;


        /// <summary>
        /// 是否完成比赛
        /// </summary>
        public static bool IsComplete = true;

        /// <summary>
        /// 当前开奖号
        /// </summary>
        public static string PerioidString = "";

        private DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

        /// <summary>
        /// 消息配置
        /// </summary>
        public static List<Config> Configs = new List<Config>();

        /// <summary>
        /// 指令配置
        /// </summary>
        public static List<Config> OrderConfig = new List<Config>();

        public string WxTuanDui = "";

        /// <summary>
        /// 接受消息线程
        /// </summary>
        public Thread ThreadRece = new Thread(new ThreadStart(ReceiveMsg));

        private static void ReceiveMsg()
        {
            while (true)
            {
                ReceiveMessage.Instance.Start();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 发送消息线程
        /// </summary>
        public Thread ThreadSend = new Thread(new ThreadStart(SendMsg));

        private static void SendMsg()
        {
            while (true)
            {
                SendMessage.Instance.Start();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 上报消息线程
        /// </summary>
        public Thread ThreadLobby = new Thread(new ThreadStart(SendLobby));

        private static void SendLobby()
        {
            while (true)
            {
                Lobby.Instance.Start();
                Thread.Sleep(1000);
            }
        }


        /// <summary>
        /// 设置dgvUp的数据源
        /// </summary>
        /// <param name="list"></param>
        private void SetDgvPost(List<UpDowModel> list)
        {
            if (dgvUp.Rows.Count >= CurrentRow && CurrentRow >= 0)
            {
                this.dgvUp.DataSource = list;

                if (CurrentRow >= dgvUp.Rows.Count)
                {
                    CurrentRow = dgvUp.Rows.Count - 1;
                }
                if (CurrentRow <= 0)
                {
                    CurrentRow = 0;
                }
                if (dgvUp.Rows.Count > 0)
                {
                    this.dgvUp.CurrentCell = dgvUp.Rows[CurrentRow].Cells[0];
                }
            }
        }

        private void SetDgvZDPost(List<ZhanDan> list)
        {
            if (dgvZhanDan.Rows.Count >= CurrentZDRow && CurrentZDRow >= 0)
            {
                this.dgvZhanDan.DataSource = list;

                if (CurrentZDRow >= dgvZhanDan.Rows.Count)
                {
                    CurrentZDRow = dgvZhanDan.Rows.Count - 1;
                }
                if (CurrentZDRow <= 0)
                {
                    CurrentZDRow = 0;
                }
                if (dgvZhanDan.Rows.Count > 0)
                {
                    this.dgvZhanDan.CurrentCell = dgvZhanDan.Rows[CurrentZDRow].Cells[0];
                }
            }
        }


        private void frmMainForm_Load(object sender, EventArgs e)
        {
            ///请求服务器获取盘口数据
            List<LoginModel> list = new List<LoginModel>();
            list.Add(new LoginModel() { PankouId = "1", PankouName = "香港赛车PK10", IsLogin = 0, IsSum = 0 });
            cmbPankou.DataSource = list;
            cmbPankou.DisplayMember = "PankouName";
            cmbPankou.ValueMember = "PankouId";

            ////微信心跳
            //((Action)(delegate ()
            //{
            //    string sync_flag = "";
            //    while (true)
            //    {
            //        sync_flag = wxs.WxSyncCheck();  //同步检查,为的是保持与服务器的通讯

            //        if (!sync_flag.Contains("retcode:\"0\"") || sync_flag == null)
            //        {
            //            lblStatus.Text = "刷新中";
            //        }
            //        else
            //        {
            //            lblStatus.Text = "正常登陆";
            //        }
            //    }

            //})).BeginInvoke(null, null);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = txtUser.Text;
            string pwd = txtPwd.Text;
            string msg = "";

            if (PanKou.Instance.Login(userName, pwd, ref msg))
            {
                QunInit();
                Configs = data.GetList<Config>("type='MSG'", "");
                OrderConfig = data.GetList<Config>("type='ORDER'","");
               
                //调用同步积分接口
                string url = "/members/point/all";
                string authStake = PanKou.Instance.GetSha1("", url);
                string jsonStake = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + url, authStake, PanKou.accessKey);

                if (jsonStake != null)
                {
                    JObject jobject = JsonConvert.DeserializeObject(jsonStake) as JObject;
                    if (jobject != null)
                    {
                        string data2 = jobject["data"].ToString();
                        if (!string.IsNullOrEmpty(data2))
                        {
                            List<Members> listMembers = JsonConvert.DeserializeObject<List<Members>>(data2);

                            foreach (Members member in listMembers)
                            {
                                if (member != null && member.wechatSn != null)
                                {
                                    data.ExecuteSql(string.Format(" update ContactScore set totalScore='{0}' where uin='{1}' ", member.points, member.wechatSn));
                                }
                            }

                            List<ContactScore> listContact = data.GetList<ContactScore>("", "");

                            if (listContact != null && listContact.Count >= 0)
                            {
                                //本地存在服务器没有的数据，则删除
                                List<string> listCha = listContact.Select(p => p.Uin).Distinct().ToList().Except(listMembers.Select(p => p.wechatSn).ToList()).ToList();
                                foreach (string uin in listCha)
                                {
                                    data.ExecuteSql(string.Format(" delete from ContactScore where uin='{0}' ", uin));
                                }

                                //服务器存在，本地不存在的数据，则添加
                                List<string> listCha2 = listMembers.Select(p => p.wechatSn).ToList().Except(listContact.Select(p=>p.Uin).Distinct().ToList()).ToList();

                                foreach (string memberId in listCha2)
                                {
                                    Members model = listMembers.FirstOrDefault(p => p.wechatSn == memberId);
                                    if (model == null) continue;
                                    ContactScore contactScore = new ContactScore() { Uuid=Guid.NewGuid().ToString(), Uin=model.wechatSn, NickName=model.nickName, TotalScore=model.points.ToString().ToInt() };
                                    data.Insert<ContactScore>(contactScore, "");
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                MessageBox.Show(msg);
                return;
            }

            ((Action)(delegate ()
            {
                while (true)
                {
                    //获取消息列表，并原样输出
                    string sync_flag = frmMainForm.wxs.WxSyncCheck();//同步检查

                    Thread.Sleep(1);
                }

            })).BeginInvoke(null, null);
        }

        private void QunInit()
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

                        if (user.NickName.Contains("微信团队"))
                        {
                            WxTuanDui = user.UserName;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("微信故障，请重新登陆");
                    Environment.Exit(0);
                }
              

                //获取微信群，并存到数据库
                //删除旧数据
                data.ExecuteSql("delete from quntb");

                foreach (WXUser user in contact_all)
                {
                    if (user.UserName.Contains("@@"))
                    {
                        QunTb model = new QunTb();
                        model.UserName = user.UserName;
                        model.NickName = user.NickName;
                        model.IsStart = 0;
                        data.Insert<QunTb>(model, "");
                    }
                }
                IsLogin = true;
                tab.SelectedIndex = 1;
                List<QunTb> qunList = data.GetList<QunTb>("", "");
                //查找盘口数据是否存在，不存在就创建一个
                cmbQun.DataSource = qunList;
                cmbQun.DisplayMember = "NickName";
                cmbQun.ValueMember = "UserName";

                lblStatus.Text = "正常登陆";
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Start == false)
            {
                Start = true;
                btnStart.Text = "结束";
                CurrentQun = cmbQun.SelectedValue.ToString();
                CurrentQunNick = cmbQun.SelectedText;
                cmbQun.Enabled = false;
                btnRefresh.Enabled = false;
                dgvUp.Enabled = true;
                //删除微信团队
                wxs.DeleteUser(CurrentQun, WxTuanDui);
                Log.WriteLogByDate("移除微信团队成功");
                string qun1 = frmMainForm.wxs.GetQun(frmMainForm.CurrentQun);
                Log.WriteLogByDate("获取群信息成功");
                //在点开始按钮的时候，启动消息
                if (Configs != null)
                {
                    //发消息
                    Config chkStart = Configs.Find(p => p.Key == "CHKSTART");
                    if (chkStart != null)
                    {
                        Config content = Configs.Find(p => p.Key == "STARTCONTENT");
                        if (content != null)
                        {
                            CurrentWX.SendMsg(new WXMsg() { From = CurrentWX.UserName, Msg = content.Value, Readed = false, Time = DateTime.Now, To = CurrentQun, Type = 1 }, false);
                        }
                    }
                }
                Log.WriteLogByDate("发送游戏开始信息成功");

                DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
                //获取开奖信息，并将开奖信息保存到数据库
                string urlConfiger = "/user/client/stake/configer";
                string auth = PanKou.Instance.GetSha1("", urlConfiger);

                //string json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger, auth, PanKou.accessKey);
                //if (!string.IsNullOrEmpty(json))
                //{
                //    return;
                //}
                
                Log.WriteLogByDate("发送当前游戏信息成功");
                ///启动线程
                timeDgv.Start();

                if (ThreadRece.ThreadState == ThreadState.Suspended)
                {
                    ThreadRece.Resume();
                }
                else
                {
                    ThreadRece.Start();
                }
                // TaskManager.Instance.Start(Start);
                if (ThreadSend.ThreadState == ThreadState.Suspended)
                {
                    ThreadSend.Resume();
                }
                else
                {
                    ThreadSend.Start();
                }
                if (ThreadLobby.ThreadState == ThreadState.Suspended)
                {
                    ThreadLobby.Resume();
                }
                else
                {
                    ThreadLobby.Start();
                }


                string json = "";
                while (true)
                {
                    json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger, auth, PanKou.accessKey);
                    if (!string.IsNullOrEmpty(json))
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }

                JObject configHelper = JsonConvert.DeserializeObject(json) as JObject;
                if (configHelper == null)
                {
                    MessageBox.Show("未获取到比赛信息");
                    return;
                }
                if (configHelper["result"].ToString() != "SUCCESS")
                {
                    MessageBox.Show("未获取到比赛信息");
                    return;
                }
                string gameId = configHelper["data"]["racingNum"].ToString();
                if (string.IsNullOrEmpty(gameId))
                {
                    Log.WriteLogByDate("发生异常：当前期号为空，上期期号为" + configHelper["data"]["preRacingNum"].ToString());
                    return;
                }

                string nextStartTime = configHelper["data"]["startRacingTime"].ToString();
                string stage = configHelper["data"]["stage"].ToString();//stage=1,押注阶段；stage=2,上报阶段；stage=3,封盘阶段

                if (stage == "1")
                {
                    CurrentWX.SendMsg(new WXMsg() { From = CurrentWX.UserName, Msg = "---接收下单---", Readed = false, Time = DateTime.Now, To = CurrentQun, Type = 1 }, false);
                }
                else
                {
                    CurrentWX.SendMsg(new WXMsg() { From = CurrentWX.UserName, Msg = "---正在封盘---", Readed = false, Time = DateTime.Now, To = CurrentQun, Type = 1 }, false);
                }
            }
            else
            {
                Start = false;
                btnStart.Text = "开始";
                
                cmbQun.Enabled = true;
                btnRefresh.Enabled = true;
                dgvUp.Enabled = false;

                //结束线程
                try
                {
                    timeDgv.Stop();
                    ThreadRece.Suspend();
                    ThreadSend.Suspend();
                    ThreadLobby.Suspend();
                }
                catch (Exception ex)
                {
                    //结束线程
                    Log.WriteLogByDate("结束线程");
                }

                //在点结束按钮的时候，发送结束消息
                //清理垃圾数据
                data.ExecuteSql(" update OriginMsg set issucc='1' ");//原始消息，全部删除
                data.ExecuteSql("update nowmsg set issucc=1");
                data.ExecuteSql("update nowmsg set isdeal='1'");
                data.ExecuteSql("update nowmsg set isdelete='1'");

                data.ExecuteSql(" update game set issucc=1 ");//期数全部结束
                data.ExecuteSql(" delete from gamemsg");//期数消息全部全部删除
            }

            ((Action)(delegate ()
            {
                while (Start)
                {
                    //获取消息列表，并原样输出
                    string sync_flag = frmMainForm.wxs.WxSyncCheck();//同步检查

                    Thread.Sleep(1000);
                }

            })).BeginInvoke(null, null);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            cmbQun.Enabled = false;
            btnStart.Enabled = false;
            QunInit();
            cmbQun.Enabled = true;
            btnStart.Enabled = true;
            MessageBox.Show("刷新成功");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string where = "";

            if (!string.IsNullOrWhiteSpace(txtNickName.Text))
            {
                where = where + string.Format(" t.msgfromname like '%{0}%'",txtNickName.Text);
            }
           
            if (!string.IsNullOrWhiteSpace(where))
            {
                where = where + " and ";
            }
            where=where + " t.createdate>="+ dtpBegin.Value.DateTimeToUnixTimestamp();
            where = where + "  and t.createdate<=" + dtpEnd.Value.DateTimeToUnixTimestamp();

            List<History> msg = data.GetListNonTable<History>("select t.msgfromname,t.ordercontect,t.opdate,t.period,t.result from nowmsg t where "+where+ " order by createdate desc ");

            dgbHistory.DataSource =msg;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<UpDowModel> list = ScoreManager.Instance.GetUpDowModel();

            SetDgvPost(list);
            //刷新账单
            SetDgvZDPost(ScoreManager.Instance.GetZhanDan());

            //刷新界面展示开奖号码，倒计时，到期时间
            string urlConfiger = "/user/client/stake/configer";
            string auth = PanKou.Instance.GetSha1("", urlConfiger);

            string json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger, auth, PanKou.accessKey);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            JObject configHelper = JsonConvert.DeserializeObject(json) as JObject;
            if (configHelper != null)
            {
                try
                {
                    lblykj.Text = configHelper["data"]["racingNum"].ToString();
                    //倒计时
                    int racingTime = configHelper["data"]["startRacingTime"].ToString().ToInt();

                    toolStripStatusLabel3.Text = (racingTime / 1000).ToString();
                }
                catch (Exception ex)
                {
//什么也不做
                }
            }
            lblKjhm.Text = PerioidString;
        }

        private void dgvUp_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CurrentRow = e.RowIndex;
        }


        private void 同意ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvUp.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            string content = "";

            string msgId = row.Cells["MsgId"].Value.ToString();
            string nickName = row.Cells["NickName"].Value.ToString();
            string uin = row.Cells["Uin"].Value.ToString();
            string score = row.Cells["Score"].Value.ToString();
            string command = row.Cells["CommandOne"].Value.ToString();
            string succ = row.Cells["Succ"].Value.ToString();
            //检查用户是否存在，并添加分数，同意消息（IsSucc=1）,IsDelete=1
            if (succ != "同意")
            {
                List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
                pkList.Add(new KeyValuePair<string, object>("Uin", uin));
                ContactScore model = data.First<ContactScore>(pkList, "");
                if (model == null)
                {
                    model = new ContactScore() { NickName = nickName, TotalScore = 0, Uin = uin, Uuid = Guid.NewGuid().ToString() };
                    data.Insert<ContactScore>(model, "");
                }

                if (command == "上")
                {
                    if (score == "0")
                    {
                        MessageBox.Show("上分不能为0");
                        return;
                    }

                    UpPoint upPoint = new UpPoint();
                    upPoint.nickName = nickName;
                    upPoint.updatePoints = Convert.ToInt32(score);
                    upPoint.wechatSn = uin;
                    string auth = PanKou.Instance.GetSha1(JsonConvert.SerializeObject(upPoint), "/members/point/add");
                    string body = JsonConvert.SerializeObject(upPoint);
                    body = body.Replace(" ", "");
                    body = Regex.Replace(body, "\\s{2,}", ",");

                    string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + "/members/point/add", body, auth, PanKou.accessKey);
                    JObject result = JsonConvert.DeserializeObject(json) as JObject;
                    if (result["result"].ToString() == "SUCCESS")
                    {
                        model.TotalScore = model.TotalScore + Convert.ToInt32(score);
                        data.Update<ContactScore>(model, pkList, "");
                        content = " 上分成功";
                    }
                    else
                    {
                        MessageBox.Show(result["message"].ToString());
                        return;
                    }
                }
                else if (command == "下")
                {
                    //先检查这期有没有参与，有参与不能下分
                    List<NowMsg> listMsg = data.GetList<NowMsg>(string.Format(" period='{0}' and CommandType in ('买名次','冠亚和','名次大小单双龙虎') ", Perioid), "");
                    if ((listMsg != null && listMsg.Count <= 0)|| frmMainForm.IsAllowDown)
                    {
                        if (model.TotalScore >= Convert.ToInt32(score))
                        {
                            if (score == "0")
                            {
                                MessageBox.Show("下分不能为0");
                                return;
                            }

                            UpPoint upPoint = new UpPoint();
                            upPoint.nickName = nickName;
                            upPoint.updatePoints = Convert.ToInt32(score);
                            upPoint.wechatSn = uin;

                            string auth = PanKou.Instance.GetSha1(JsonConvert.SerializeObject(upPoint), "/members/point/subtract");
                            string body = JsonConvert.SerializeObject(upPoint);
                            body = body.Replace(" ", "");
                            body = Regex.Replace(body, "\\s{2,}", ",");

                            string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + "/members/point/subtract", body, auth, PanKou.accessKey);
                            JObject result = JsonConvert.DeserializeObject(json) as JObject;
                            if (result["result"].ToString() == "SUCCESS")
                            {
                                model.TotalScore = model.TotalScore - Convert.ToInt32(score);
                                data.Update<ContactScore>(model, pkList, "");

                                content = " 下分成功";
                            }
                            else
                            {
                                MessageBox.Show(result["message"].ToString());
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("余额不足：" + model.TotalScore);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("当期有交易，不能下分");
                        return;
                    }
                }
                else if (command == "查")
                {
                    List<KeyValuePair<string, object>> pkContact = new List<KeyValuePair<string, object>>();
                    pkContact.Add(new KeyValuePair<string, object>("Uin", uin));
                    ContactScore contactScore = data.First<ContactScore>(pkList, "");
                    if (contactScore != null)
                    {
                        content = " 当前积分：" + contactScore.TotalScore;
                    }
                }
                else
                {
                    return;
                }

                List<KeyValuePair<string, object>> pkMsgList = new List<KeyValuePair<string, object>>();
                pkMsgList.Add(new KeyValuePair<string, object>("MsgId", msgId));
                NowMsg msg = data.First<NowMsg>(pkMsgList, "");
                if (msg != null)
                {
                    msg.IsDeal = "1";
                    msg.IsSucc = 1;
                    msg.IsDelete = "1";
                    msg.Result = "同意";
                    data.Update<NowMsg>(msg, pkMsgList, "");
                    row.Cells["Succ"].Value = "同意";

                    msg = data.First<NowMsg>(pkMsgList, "");

                    WXMsg wxMsg = new WXMsg();
                    //发消息
                    if (String.IsNullOrEmpty(frmMainForm.CurrentQun))
                    {
                        wxMsg.To = "@";
                    }
                    else
                    {
                        wxMsg.To = frmMainForm.CurrentQun;
                    }
                    if (frmMainForm.CurrentWX == null)
                    {
                        Log.WriteLogByDate("当前微信未登陆");
                        wxMsg.From = "@";
                    }
                    else
                    {
                        wxMsg.From = frmMainForm.CurrentWX.UserName;
                    }
                    wxMsg.Type = 1;
                    wxMsg.Time = DateTime.Now;
                    wxMsg.Msg = "@" + nickName + " " + content;
                    frmMainForm.CurrentWX.SendMsg(wxMsg, false);
                }
            }

            ///不再刷新下,Timer自动刷新
            //dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
        }

        private void 不同意ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvUp.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            //不同意消息（IsSucc=2）,IsDelete=1
            string msgId = row.Cells["MsgId"].Value.ToString();
            string succ = row.Cells["Succ"].Value.ToString();

            if (succ != "不同意")
            {
                List<KeyValuePair<string, object>> pkMsgList = new List<KeyValuePair<string, object>>();
                pkMsgList.Add(new KeyValuePair<string, object>("MsgId", msgId));
                NowMsg msg = data.First<NowMsg>(pkMsgList, "");
                if (msg != null)
                {
                    msg.IsDelete = "1";
                }
                msg.Result = "不同意";
                data.Update<NowMsg>(msg, pkMsgList, "");
                row.Cells["Succ"].Value = "不同意";
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvUp.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            //IsDelete=1
            string msgId = row.Cells["MsgId"].Value.ToString();
            string succ = row.Cells["Succ"].Value.ToString();
            if (succ != "已删除")
            {
                List<KeyValuePair<string, object>> pkMsgList = new List<KeyValuePair<string, object>>();
                pkMsgList.Add(new KeyValuePair<string, object>("MsgId", msgId));
                NowMsg msg = data.First<NowMsg>(pkMsgList, "");
                if (msg != null)
                {
                    msg.IsDelete = "1";
                }
                msg.Result = "已删除";
                data.Update<NowMsg>(msg, pkMsgList, "");
                row.Cells["Succ"].Value = "已删除";
            }
        }

        private void btnUpContent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未开发");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未开发");
        }

        

        private void tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tab.SelectedIndex > 0)
            {
                if (!IsLogin)
                {
                    MessageBox.Show("请先点击登录");
                    tab.SelectedIndex = 0;
                    tabPankou.Show();
                }
                else
                {
                    if (tab.SelectedIndex == 3)
                    {
                        InitMsgConfig();
                    }
                    else if (tab.SelectedIndex == 2)
                    {
                        InitOrderConfig();
                    }
                }
            }
        }

        private void InitOrderConfig()
        {
            List<Config> list = data.GetList<Config>("type='ORDER'", "");
            if (list == null || list.Count <= 0)
            {
                return;
            }

            //名次指令设置
            txtMC1.Text = list.Find(p => p.Key == "MC1").Value;
            txtMC2.Text = list.Find(p => p.Key == "MC2").Value;
            txtMC3.Text = list.Find(p => p.Key == "MC3").Value;
            txtMC4.Text = list.Find(p => p.Key == "MC4").Value;
            txtMC5.Text = list.Find(p => p.Key == "MC5").Value;
            txtMC6.Text = list.Find(p => p.Key == "MC6").Value;
            txtMC7.Text = list.Find(p => p.Key == "MC7").Value;
            txtMC8.Text = list.Find(p => p.Key == "MC8").Value;
            txtMC9.Text = list.Find(p => p.Key == "MC9").Value;
            txtMC10.Text = list.Find(p => p.Key == "MC10").Value;

            //上下查，取消
            txtShang.Text = list.Find(p => p.Key == "MCSHANG").Value;
            txtXia.Text = list.Find(p => p.Key == "MCXIA").Value;
            txtCha.Text = list.Find(p => p.Key == "MCCHA").Value;
            txtQu.Text = list.Find(p => p.Key == "MCQU").Value;
            txtHe.Text= list.Find(p => p.Key == "MCHE").Value;

            //大小单双龙虎
            txtDa.Text = list.Find(p => p.Key == "MCDA").Value;
            txtXiao.Text = list.Find(p => p.Key == "MCXIAO").Value;
            txtdan.Text = list.Find(p => p.Key == "MCDAN").Value;
            txtshuang.Text = list.Find(p => p.Key == "MCSHUANG").Value;
            txtLong.Text = list.Find(p => p.Key == "MCLONG").Value;
            txtHu.Text = list.Find(p => p.Key == "MCHU").Value;

            //下注高低限额设置
            txtMCdi.Text = list.Find(p => p.Key == "MCDI").Value;
            txtMCgao.Text = list.Find(p => p.Key == "MCGAO").Value;
            txtLongDi.Text = list.Find(p => p.Key == "MCLONGDI").Value;
            txtLongGao.Text = list.Find(p => p.Key == "MCLONGGAO").Value;
            txtDanDi.Text = list.Find(p => p.Key == "MCDANDI").Value;
            txtDanGao.Text = list.Find(p => p.Key == "MCDANGAO").Value;
            txtDaDi.Text = list.Find(p => p.Key == "MCDADI").Value;
            txtDaGao.Text = list.Find(p => p.Key == "MCDAGAO").Value;
        }

        private void InitMsgConfig()
        {
            List<Config> list = data.GetList<Config>("type='MSG'", "");
            ///封盘
            txtFp.Text = list.Find(p => p.Type == "MSG" && p.Key == "FPTIME").Value;
            ckbFp.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "FPCHK").Value);
            txtMulFp.Text = list.Find(p => p.Type == "MSG" && p.Key == "FPCONTENT").Value;

            //普通
            txtPt.Text = list.Find(p => p.Type == "MSG" && p.Key == "PTTIME").Value;
            ckbPt.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK").Value);
            txtMulPt.Text = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT").Value;

            //普通2
            txtPt2.Text = list.Find(p => p.Type == "MSG" && p.Key == "PTTIME2").Value; ;
            ckbpt2.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK2").Value);
            txtMulPt2.Text = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT2").Value;

            //普通3
            txtPt3.Text = list.Find(p => p.Type == "MSG" && p.Key == "PTTIME3").Value;
            ckbpt3.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "PTCHK3").Value);
            txtMulPt3.Text = list.Find(p => p.Type == "MSG" && p.Key == "PTCONTENT3").Value;

            //开奖信息
            ckbkj.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "KJCHK").Value);
            txtMulKj.Text = list.Find(p => p.Type == "MSG" && p.Key == "KJCONTENT").Value;

            //启用信息
            ckbStart.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "CHKSTART").Value);
            txtMulStart.Text = list.Find(p => p.Type == "MSG" && p.Key == "STARTCONTENT").Value;

            //开奖后
            txtkjh.Text = list.Find(p => p.Type == "MSG" && p.Key == "KJHTIME").Value;
            ckbkjh.Checked = Convert.ToBoolean(list.Find(p => p.Type == "MSG" && p.Key == "KJHCHK").Value);
        }

        private void dgvZhanDan_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvZhanDan.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            string nickName = row.Cells["NickName2"].Value.ToString();
            string uin = row.Cells["Uid"].Value.ToString();
            txtWxUserName.Text = nickName;
            txtUin.Text = uin;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUin.Text))
            {
                MessageBox.Show("请选择会员");
                return;
            }

            try
            {
                Convert.ToInt32(txtMoney.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字");
                return;
            }

            if (Convert.ToInt32(txtMoney.Text)==0)
            {
                MessageBox.Show("上分不能为0");
                return;
            }

            UpPoint upPoint = new UpPoint();
            upPoint.nickName = txtNickName.Text;
            upPoint.updatePoints = Convert.ToInt32(txtMoney.Text);
            upPoint.wechatSn = txtUin.Text;

            string auth = PanKou.Instance.GetSha1(JsonConvert.SerializeObject(upPoint), "/members/point/add");
            string body = JsonConvert.SerializeObject(upPoint);
            body = body.Replace(" ", "");
            body = Regex.Replace(body, "\\s{2,}", ",");

            string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + "/members/point/add", body, auth, PanKou.accessKey);
            JObject result = JsonConvert.DeserializeObject(json) as JObject;
            if (result["result"].ToString() == "SUCCESS")
            {
                if (ScoreManager.Instance.UpScore(txtUin.Text, Convert.ToInt32(txtMoney.Text)))
                {
                    dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
                    MessageBox.Show("上分成功");
                }
                else
                {
                    MessageBox.Show("上分失败");
                }
            }
            else
            {
                MessageBox.Show(result["message"].ToString());
                return;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUin.Text))
            {
                MessageBox.Show("请选择会员");
                return;
            }

            try
            {
                Convert.ToInt32(txtMoney.Text);
            }
            catch
            {
                MessageBox.Show("请输入数字");
                return;
            }

            if (Convert.ToInt32(txtMoney.Text) == 0)
            {
                MessageBox.Show("下分不能为0");
                return;
            }

            List<NowMsg> listMsg = data.GetList<NowMsg>(string.Format(" period='{0}' and CommandType in('名次大小单双龙虎','买名次','冠亚和')", Perioid), "");
            if (listMsg != null && listMsg.Count <= 0)
            {

                UpPoint upPoint = new UpPoint();
                upPoint.nickName = txtNickName.Text;
                upPoint.updatePoints = Convert.ToInt32(txtMoney.Text);
                upPoint.wechatSn = txtUin.Text;
                string auth = PanKou.Instance.GetSha1(JsonConvert.SerializeObject(upPoint), "/members/point/subtract");
                string body = JsonConvert.SerializeObject(upPoint);
                body = body.Replace(" ", "");
                body = Regex.Replace(body, "\\s{2,}", ",");

                string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + "/members/point/subtract", body, auth, PanKou.accessKey);
                JObject result = JsonConvert.DeserializeObject(json) as JObject;
                if (result["result"].ToString() == "SUCCESS")
                {
                    if (ScoreManager.Instance.DownScore(txtUin.Text, Convert.ToInt32(txtMoney.Text)))
                    {
                        dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
                        MessageBox.Show("下分成功");
                    }
                    else
                    {
                        MessageBox.Show("下分失败,请核对余额");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(result["message"].ToString());
                    return;
                }
            }
            else
            {
                MessageBox.Show("当期有交易，不能下分");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUin.Text))
            {
                MessageBox.Show("请选择会员");
                return;
            }

            try
            {
                int score = Convert.ToInt32(txtMoney.Text);
                if (score < 0)
                {
                    MessageBox.Show("积分不能为负数");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("请输入数字");
                return;
            }

            if (ScoreManager.Instance.UpdateScore(txtUin.Text, Convert.ToInt32(txtMoney.Text)))
            {
                dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
                MessageBox.Show("改分成功");
            }
            else
            {
                MessageBox.Show("改分失败");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //删除消息配置
            data.ExecuteSql("delete from config where type='MSG' ");

            ///下注信息核对提示
            string fpTime = txtFp.Text;
            string fpChk = ckbFp.Checked.ToString();
            string fpContent = txtMulFp.Text;
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "FPTIME", Value = fpTime }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "FPCHK", Value = fpChk }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "FPCONTENT", Value = fpContent }, "");

            //普通
            string ptTime = txtPt.Text;
            string ptChk = ckbPt.Checked.ToString();
            string ptContent = txtMulPt.Text;
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTTIME", Value = ptTime }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTCHK", Value = ptChk }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTCONTENT", Value = ptContent }, "");

            //普通2
            string ptTime2 = txtPt2.Text;
            string ptChk2 = ckbpt2.Checked.ToString();
            string ptContent2 = txtMulPt2.Text;
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTTIME2", Value = ptTime2 }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTCHK2", Value = ptChk2 }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTCONTENT2", Value = ptContent2 }, "");

            //普通3
            string ptTime3 = txtPt3.Text;
            string ptChk3 = ckbpt3.Checked.ToString();
            string ptContent3 = txtMulPt3.Text;
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTTIME3", Value = ptTime3 }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTCHK3", Value = ptChk3 }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "PTCONTENT3", Value = ptContent3 }, "");

            //开奖信息
            string kjChk = ckbkj.Checked.ToString();
            string kjContent = txtMulKj.Text;
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "KJCHK", Value = kjChk }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "KJCONTENT", Value = kjContent }, "");

            //启用信息
            string chkStart = ckbStart.Checked.ToString();
            string startContent = txtMulStart.Text.ToString();
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "CHKSTART", Value = chkStart }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "STARTCONTENT", Value = startContent }, "");

            //开奖后
            string kjhTime = txtkjh.Text;
            string kjhChk = ckbkjh.Checked.ToString();
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "KJHTIME", Value = kjhTime }, "");
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "KJHCHK", Value = kjhChk }, "");

            //封盘提示
            string fpx = txtFpx.Text;
            data.Insert<Config>(new Config() { Uuid = Guid.NewGuid().ToString(), Type = "MSG", Typetwo = "", Key = "FPX", Value = fpx }, "");

            Configs = data.GetList<Config>("type='MSG'", "");
            MessageBox.Show("保存成功");
        }

        private void frmMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void frmMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void txtZhiLin_Click(object sender, EventArgs e)
        {
            data.ExecuteSql("delete from config where type='ORDER' ");

            //名次指令设置
            Config mc1 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC1", Value = txtMC1.Text };
            data.Insert<Config>(mc1, "");
            Config mc2 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC2", Value = txtMC2.Text };
            data.Insert<Config>(mc2, "");
            Config mc3 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC3", Value = txtMC3.Text };
            data.Insert<Config>(mc3, "");
            Config mc4 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC4", Value = txtMC4.Text };
            data.Insert<Config>(mc4, "");
            Config mc5 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC5", Value = txtMC5.Text };
            data.Insert<Config>(mc5, "");
            Config mc6 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC6", Value = txtMC6.Text };
            data.Insert<Config>(mc6, "");
            Config mc7 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC7", Value = txtMC7.Text };
            data.Insert<Config>(mc7, "");
            Config mc8 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC8", Value = txtMC8.Text };
            data.Insert<Config>(mc8, "");
            Config mc9 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC9", Value = txtMC9.Text };
            data.Insert<Config>(mc9, "");
            Config mc10 = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MC10", Value = txtMC10.Text };
            data.Insert<Config>(mc10, "");
            //上下查，取消
            Config mcShang = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCSHANG", Value = txtShang.Text };
            data.Insert<Config>(mcShang, "");
            Config mcxia = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCXIA", Value = txtXia.Text };
            data.Insert<Config>(mcxia, "");
            Config mcCha = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCCHA", Value = txtCha.Text };
            data.Insert<Config>(mcCha, "");
            Config mcQu = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCQU", Value = txtQu.Text };
            data.Insert<Config>(mcQu, "");
            Config mcHe = new Config() {Uuid=Guid.NewGuid().ToString(),Type="ORDER",Key="MCHE",Value=txtHe.Text };
            data.Insert<Config>(mcHe, "");

            //大小单双龙虎
            Config mcDa = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDA", Value = txtDa.Text };
            data.Insert<Config>(mcDa, "");
            Config mcXiao = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCXIAO", Value = txtXiao.Text };
            data.Insert<Config>(mcXiao, "");
            Config mcDang = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDAN", Value = txtdan.Text };
            data.Insert<Config>(mcDang, "");
            Config mcShuang = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCSHUANG", Value = txtshuang.Text };
            data.Insert<Config>(mcShuang, "");
            Config mcLong = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCLONG", Value = txtLong.Text };
            data.Insert<Config>(mcLong, "");
            Config mcHu = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCHU", Value = txtHu.Text };
            data.Insert<Config>(mcHu, "");

            //下注高低限额设置
            Config mcDi = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDI", Value = txtMCdi.Text.ToInt().ToString() };
            data.Insert<Config>(mcDi, "");
            Config mcGao = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCGAO", Value = txtMCgao.Text.ToInt().ToString() };
            data.Insert<Config>(mcGao, "");
            Config longDi = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCLONGDI", Value = txtLongDi.Text.ToInt().ToString() };
            data.Insert<Config>(longDi, "");
            Config longGao = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCLONGGAO", Value =txtLongGao.Text.ToInt().ToString() };
            data.Insert<Config>(longGao, "");
            Config danDi = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDANDI", Value = txtDanDi .Text.ToInt().ToString() };
            data.Insert<Config>(danDi, "");
            Config danGao = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDANGAO", Value = txtDanGao.Text.ToInt().ToString() };
            data.Insert<Config>(danGao, "");
            Config daDi = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDADI", Value = txtDaDi.Text.ToInt().ToString() };
            data.Insert<Config>(daDi, "");
            Config daGao = new Config() { Uuid = Guid.NewGuid().ToString(), Type = "ORDER", Key = "MCDAGAO", Value = txtDaGao.Text.ToInt().ToString() };
            data.Insert<Config>(daGao, "");

            OrderConfig = data.GetList<Config>("type='ORDER'", "");
            MessageBox.Show("保存成功");
        }

        private void dgvZhanDan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            CurrentZDRow = e.RowIndex;
        }

        private void 删除玩家ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //删除玩家
            DataGridViewRow row = dgvZhanDan.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            string nickName = row.Cells["NickName2"].Value.ToString();
            string uin = row.Cells["Uid"].Value.ToString();
            //删除用户积分信息，有下注
            List<NowMsg> listMsg = data.GetList<NowMsg>(string.Format(" period='{0}' and CommandType in ('买名次','冠亚和','名次大小单双龙虎') ", Perioid), "");
            if (listMsg != null && listMsg.Count > 0)
            {
                MessageBox.Show("玩家有下注，不能删除");
                return;
            }
            else
            {
                data.ExecuteSql(string.Format("delete from contactscore where Uin='{0}'",uin));
                string urlConfiger = "/user/members/"+uin;
                string auth = PanKou.Instance.GetSha1("", urlConfiger);
                string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger,"", auth, PanKou.accessKey);
                Log.WriteLogByDate("删除玩家：" + uin);
                Log.WriteLogByDate("删除结果："+json);

                JObject delJson = JsonConvert.DeserializeObject(json) as JObject;
                MessageBox.Show(delJson["message"].ToString());
            }
        }
    }
}

