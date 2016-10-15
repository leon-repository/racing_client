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

        public bool IsLogin = false;

        /// <summary>
        /// 是否处于押注状态，false是不可以押注的
        /// </summary>
        public static bool IsContinue = true;

        private DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

        public static List<Config> Configs = new List<Config>();

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

        private void frmMainForm_Load(object sender, EventArgs e)
        {
            ///请求服务器获取盘口数据
            List<LoginModel> list = new List<LoginModel>();
            list.Add(new LoginModel() { PankouId = "1", PankouName = "香港赛车PK10", IsLogin = 0, IsSum = 0 });
            //list.Add(new LoginModel() { PankouId = "2", PankouName = "盘口B", IsLogin = 0, IsSum = 0 });

            //DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());
            //foreach (var item in list)
            //{
            //    List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
            //    pkList.Add(new KeyValuePair<string, object>("PankouId",item.PankouId));
            //    LoginModel pankou=data.First<LoginModel>(pkList, "");
            //    if (pankou == null)
            //    {
            //        data.Insert<LoginModel>(item, "");
            //    }
            //    else
            //    {
            //        data.Update<LoginModel>(item, pkList, "");
            //    }
            //}

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
            }
            else
            {
                MessageBox.Show(msg);
            }
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
                    }
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
                //开始定时处理消息
                TaskManager.Instance.Start(true);
                //开始更新dataGridView数据表
                timeDgv.Start();
                //账单信息显示
                dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
                dgvUp.Enabled = true;

                //在点开始按钮的时候，启动消息
                List<Config> configs = data.GetList<Config>(" Type='MSG' and Key in ('CHKSTART','STARTCONTENT') ", "");
                if (configs != null && configs.Count == 2)
                {
                    //发消息
                    Config chkStart = configs.Find(p => p.Key == "CHKSTART");
                    if (chkStart != null)
                    {
                        Config content = configs.Find(p => p.Key == "STARTCONTENT");
                        if (content != null)
                        {
                            CurrentWX.SendMsg(new WXMsg() { From = CurrentWX.UserName, Msg = content.Value, Readed = false, Time = DateTime.Now, To = CurrentQun, Type = 1 }, false);
                        }
                    }
                }
            }
            else
            {
                Start = false;
                btnStart.Text = "开始";
                TaskManager.Instance.Start(false);
                timeDgv.Stop();
                cmbQun.Enabled = true;
                btnRefresh.Enabled = true;
                dgvUp.Enabled = false;

                //在点结束按钮的时候，发送结束消息
                //清理垃圾数据
                data.ExecuteSql(" update OriginMsg set issucc='1' ");//原始消息，全部删除
                data.ExecuteSql("update nowmsg set issucc=1");
                data.ExecuteSql("update nowmsg set isdeal='1'");
                data.ExecuteSql("update nowmsg set isdelete='1'");

                data.ExecuteSql(" update game set issucc=1 ");//期数全部结束
                data.ExecuteSql(" update gamemsg set issend=1");//期数消息全部发送

            }

            //((Action)(delegate ()
            //{
            //    string sync_flag = "";
            //    while (true)
            //    {
            //        sync_flag = wxs.WxSyncCheck();  //同步检查,为的是保持与服务器的通讯

            //        if (sync_flag == null)
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

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<UpDowModel> list = ScoreManager.Instance.GetUpDowModel();

            SetDgvPost(list);

            //刷新界面展示开奖号码，倒计时，到期时间
            //string urlConfiger = "/user/client/stake/configer";
            //string auth = PanKou.Instance.GetSha1("", urlConfiger);

            //string json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + urlConfiger, auth, PanKou.accessKey);

            //JObject configHelper = JsonConvert.DeserializeObject(json) as JObject;

            //if (IsContinue&& configHelper!=null)
            //{
            //    lblykj.Text = configHelper["data"]["racingNum"].ToString();
            //    toolStripStatusLabel3.Text= configHelper["data"]["startRacingTime"].ToString();
            //}
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

                    string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + "/members/point/add",body,auth,PanKou.accessKey);
                    JObject result = JsonConvert.DeserializeObject(json) as JObject;
                    if (result["result"].ToString() == "SUCCESS")
                    {
                        model.TotalScore = model.TotalScore + Convert.ToInt32(score);
                        data.Update<ContactScore>(model, pkList, "");
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
                    List<NowMsg> listMsg = data.GetList<NowMsg>(string.Format(" period='{0}' and CommandType not in ('上下查','指令格式错误') ", Perioid), "");
                    if (listMsg != null && listMsg.Count <= 0)
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

                            string json = WebService.SendPutRequest2(ConfigHelper.GetXElementNodeValue("Client", "url") + "/members/point/subtract",body,auth,PanKou.accessKey );
                            JObject result = JsonConvert.DeserializeObject(json) as JObject;
                            if (result["result"].ToString() == "SUCCESS")
                            {
                                model.TotalScore = model.TotalScore - Convert.ToInt32(score);
                                data.Update<ContactScore>(model, pkList, "");
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
                    wxMsg.Msg = "@" + nickName + " " + command + "分成功";
                    frmMainForm.CurrentWX.SendMsg(wxMsg, false);
                }
            }

            ///刷新下
            dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
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

        private void timerRecive_Tick(object sender, EventArgs e)
        {
            //接收消息
            ReciveMsg.Instance.Receive();
        }

        private void timerSend_Tick(object sender, EventArgs e)
        {
            //处理消息
            SendMsg.Instance.Send();
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
                }
            }
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

            List<NowMsg> listMsg = data.GetList<NowMsg>(string.Format(" period='{0}' ", Perioid), "");
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

            ///封盘
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
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}

