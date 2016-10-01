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

        private DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

        /// <summary>
        /// 设置dgvUp的数据源
        /// </summary>
        /// <param name="list"></param>
        private void SetDgvPost(List<UpDowModel> list)
        {
            if (dgvUp.Rows.Count >= CurrentRow&&CurrentRow>=0)
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
            list.Add(new LoginModel() { PankouId = "1", PankouName = "盘口A", IsLogin = 0, IsSum = 0 });
            list.Add(new LoginModel() { PankouId = "2", PankouName = "盘口B", IsLogin = 0, IsSum = 0 });

            DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

            foreach (var item in list)
            {
                List<KeyValuePair<string, object>> pkList = new List<KeyValuePair<string, object>>();
                pkList.Add(new KeyValuePair<string, object>("PankouId",item.PankouId));
                LoginModel pankou=data.First<LoginModel>(pkList, "");
                if (pankou == null)
                {
                    data.Insert<LoginModel>(item, "");
                }
                else
                {
                    data.Update<LoginModel>(item, pkList, "");
                }
            }

            cmbPankou.DataSource = list;
            cmbPankou.DisplayMember = "PankouName";
            cmbPankou.ValueMember = "PankouId";

            //微信心跳
            ((Action)(delegate ()
            {
                string sync_flag = "";
                while (true)
                {
                    sync_flag = wxs.WxSyncCheck();  //同步检查,为的是保持与服务器的通讯

                    if (!sync_flag.Contains("retcode:\"0\"")||sync_flag==null)
                    {
                        lblStatus.Text = "刷新中";
                    }
                    else
                    {
                        lblStatus.Text = "正常登陆";
                    }
                }

            })).BeginInvoke(null, null);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userName = cmbPankou.SelectedValue.ToString();
            string pwd = txtPwd.Text;

            if (PanKou.Instance.Login(userName, pwd))
            {
                QunInit();
            }
            else
            {
                MessageBox.Show("登陆失败");
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
                cmbQun.Enabled = false;
                btnRefresh.Enabled = false;
                //开始定时处理消息
                TaskManager.Instance.Start(true);
                //开始更新dataGridView数据表
                timeDgv.Start();
                //账单信息显示
                dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
                dgvUp.Enabled = true;
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
            }

            ((Action)(delegate ()
            {
                string sync_flag = "";
                while (true)
                {
                    sync_flag = wxs.WxSyncCheck();  //同步检查,为的是保持与服务器的通讯

                    if (sync_flag == null)
                    {
                        lblStatus.Text = "刷新中";
                    }
                    else
                    {
                        lblStatus.Text = "正常登陆";
                    }
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

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<UpDowModel> list = ScoreManager.Instance.GetUpDowModel();

            SetDgvPost(list);
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
                    model.TotalScore = model.TotalScore + Convert.ToInt32(score);
                    data.Update<ContactScore>(model, pkList, "");
                }
                else if (command == "下")
                {
                    if (model.TotalScore >= Convert.ToInt32(score))
                    {
                        model.TotalScore = model.TotalScore - Convert.ToInt32(score);
                        data.Update<ContactScore>(model, pkList, "");
                    }
                    else
                    {
                        MessageBox.Show("余额不足：" + model.TotalScore);
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
            }
        }

        private void dgvZhanDan_DoubleClick(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvZhanDan.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            string nickName= row.Cells["NickName2"].Value.ToString();
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

            if (ScoreManager.Instance.DownScore(txtUin.Text, Convert.ToInt32(txtMoney.Text)))
            {
                dgvZhanDan.DataSource = ScoreManager.Instance.GetZhanDan();
                MessageBox.Show("下分成功");
            }
            else
            {
                MessageBox.Show("下分失败,请核对余额");
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
              int score =  Convert.ToInt32(txtMoney.Text);
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
    }
}

