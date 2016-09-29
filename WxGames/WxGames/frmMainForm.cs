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
using System.Threading;
using System.Windows.Forms;
using WxGames.HTTP;

namespace WxGames
{
    public partial class frmMainForm : Form
    {
        public frmMainForm()
        {
            InitializeComponent();

            //上下分操作dgv
            DataGridViewTextBoxColumn columnNickName = new DataGridViewTextBoxColumn();
            columnNickName.DataPropertyName = "NickName";
            columnNickName.HeaderText = "玩家名称";
            columnNickName.Name = "NickName";
            DataGridViewTextBoxColumn commandType = new DataGridViewTextBoxColumn();
            commandType.DataPropertyName = "CommandType";
            commandType.HeaderText = "指令类型";
            commandType.Name = "CommandType";
            DataGridViewTextBoxColumn columnOp = new DataGridViewTextBoxColumn();
            columnOp.DataPropertyName = "IsSucc";
            columnOp.HeaderText = "是否同意";
            columnOp.Name = "IsSucc";
            DataGridViewTextBoxColumn columnUin = new DataGridViewTextBoxColumn();
            columnUin.DataPropertyName = "uin";
            columnUin.HeaderText = "唯一标识";
            columnUin.Name = "uin";
            columnUin.Visible = false;
            dgvUp.Columns.Add(columnNickName);
            dgvUp.Columns.Add(commandType);
            dgvUp.Columns.Add(columnOp);
            dgvUp.Columns.Add(columnUin);

            dgvUp.DataSource = ScoreManager.Instance.GetUpDowModel();
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

        /// <summary>
        /// 当前期号
        /// </summary>
        public static string Perioid;

        /// <summary>
        /// 刷新dgvUp
        /// </summary>
        Thread dgvThead=null;

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

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (Start == false)
            {
                Start = true;

                btnStart.Text = "结束";

                CurrentQun = cmbQun.SelectedValue.ToString();
                //开始定时处理消息
                TaskManager.Instance.Start(true);
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
                string sync_flag = "";
                while (true)
                {
                    sync_flag = wxs.WxSyncCheck();  //同步检查,为的是保持与服务器的通讯
                }

            })).BeginInvoke(null, null);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            QunInit();
        }

        private void 同意ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dgvUp.CurrentRow;
            if (row == null)
            {
                MessageBox.Show("请选中一行");
                return;
            }

            string nickName = row.Cells["NickName"].Value.ToString();
            string uin = row.Cells["Uin"].Value.ToString();

            MessageBox.Show(nickName + "_" + uin);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

        }
    }
}

