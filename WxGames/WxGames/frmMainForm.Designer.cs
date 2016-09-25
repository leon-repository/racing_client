namespace WxGames
{
    partial class frmMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bottomStatus = new System.Windows.Forms.StatusStrip();
            this.lblFinish = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tab = new System.Windows.Forms.TabControl();
            this.tabPankou = new System.Windows.Forms.TabPage();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbPankou = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabZhangDan = new System.Windows.Forms.TabPage();
            this.tabZhiling = new System.Windows.Forms.TabPage();
            this.tabXiaoxi = new System.Windows.Forms.TabPage();
            this.tabYingkui = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWxUserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMoney = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.xuan = new System.Windows.Forms.Label();
            this.cmbQun = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.checkBox8 = new System.Windows.Forms.CheckBox();
            this.checkBox9 = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.checkBox10 = new System.Windows.Forms.CheckBox();
            this.checkBox11 = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnUpContent = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.bottomStatus.SuspendLayout();
            this.tab.SuspendLayout();
            this.tabPankou.SuspendLayout();
            this.tabZhangDan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // bottomStatus
            // 
            this.bottomStatus.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.bottomStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblFinish,
            this.lblStatus});
            this.bottomStatus.Location = new System.Drawing.Point(0, 999);
            this.bottomStatus.Name = "bottomStatus";
            this.bottomStatus.Size = new System.Drawing.Size(1530, 29);
            this.bottomStatus.TabIndex = 0;
            this.bottomStatus.Text = "statusStrip1";
            // 
            // lblFinish
            // 
            this.lblFinish.Name = "lblFinish";
            this.lblFinish.Size = new System.Drawing.Size(100, 24);
            this.lblFinish.Text = "微信状态：";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(64, 24);
            this.lblStatus.Text = "加载中";
            // 
            // tab
            // 
            this.tab.Controls.Add(this.tabPankou);
            this.tab.Controls.Add(this.tabZhangDan);
            this.tab.Controls.Add(this.tabZhiling);
            this.tab.Controls.Add(this.tabXiaoxi);
            this.tab.Controls.Add(this.tabYingkui);
            this.tab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tab.Location = new System.Drawing.Point(0, 0);
            this.tab.Name = "tab";
            this.tab.SelectedIndex = 0;
            this.tab.Size = new System.Drawing.Size(1292, 997);
            this.tab.TabIndex = 1;
            // 
            // tabPankou
            // 
            this.tabPankou.Controls.Add(this.txtPwd);
            this.tabPankou.Controls.Add(this.btnLogin);
            this.tabPankou.Controls.Add(this.label2);
            this.tabPankou.Controls.Add(this.cmbPankou);
            this.tabPankou.Controls.Add(this.label1);
            this.tabPankou.Location = new System.Drawing.Point(4, 28);
            this.tabPankou.Name = "tabPankou";
            this.tabPankou.Padding = new System.Windows.Forms.Padding(3);
            this.tabPankou.Size = new System.Drawing.Size(1287, 965);
            this.tabPankou.TabIndex = 0;
            this.tabPankou.Text = "盘口登陆";
            this.tabPankou.UseVisualStyleBackColor = true;
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(383, 265);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(226, 28);
            this.txtPwd.TabIndex = 4;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(332, 330);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(252, 46);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "登陆";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(287, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码";
            // 
            // cmbPankou
            // 
            this.cmbPankou.FormattingEnabled = true;
            this.cmbPankou.Location = new System.Drawing.Point(381, 218);
            this.cmbPankou.Name = "cmbPankou";
            this.cmbPankou.Size = new System.Drawing.Size(226, 26);
            this.cmbPankou.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(287, 218);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "盘口";
            // 
            // tabZhangDan
            // 
            this.tabZhangDan.Controls.Add(this.splitContainer2);
            this.tabZhangDan.Location = new System.Drawing.Point(4, 28);
            this.tabZhangDan.Name = "tabZhangDan";
            this.tabZhangDan.Padding = new System.Windows.Forms.Padding(3);
            this.tabZhangDan.Size = new System.Drawing.Size(1284, 965);
            this.tabZhangDan.TabIndex = 1;
            this.tabZhangDan.Text = "账单";
            this.tabZhangDan.UseVisualStyleBackColor = true;
            // 
            // tabZhiling
            // 
            this.tabZhiling.Location = new System.Drawing.Point(4, 28);
            this.tabZhiling.Name = "tabZhiling";
            this.tabZhiling.Padding = new System.Windows.Forms.Padding(3);
            this.tabZhiling.Size = new System.Drawing.Size(1293, 965);
            this.tabZhiling.TabIndex = 2;
            this.tabZhiling.Text = "指令设置";
            this.tabZhiling.UseVisualStyleBackColor = true;
            // 
            // tabXiaoxi
            // 
            this.tabXiaoxi.Location = new System.Drawing.Point(4, 28);
            this.tabXiaoxi.Name = "tabXiaoxi";
            this.tabXiaoxi.Padding = new System.Windows.Forms.Padding(3);
            this.tabXiaoxi.Size = new System.Drawing.Size(1293, 965);
            this.tabXiaoxi.TabIndex = 3;
            this.tabXiaoxi.Text = "消息设置";
            this.tabXiaoxi.UseVisualStyleBackColor = true;
            // 
            // tabYingkui
            // 
            this.tabYingkui.Location = new System.Drawing.Point(4, 28);
            this.tabYingkui.Name = "tabYingkui";
            this.tabYingkui.Padding = new System.Windows.Forms.Padding(3);
            this.tabYingkui.Size = new System.Drawing.Size(1293, 965);
            this.tabYingkui.TabIndex = 4;
            this.tabYingkui.Text = "盈亏计算";
            this.tabYingkui.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tab);
            this.splitContainer1.Panel1MinSize = 0;
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(1530, 999);
            this.splitContainer1.SplitterDistance = 1294;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.btnDel);
            this.splitContainer2.Panel2.Controls.Add(this.btnUpdate);
            this.splitContainer2.Panel2.Controls.Add(this.btnDown);
            this.splitContainer2.Panel2.Controls.Add(this.btnClear);
            this.splitContainer2.Panel2.Controls.Add(this.btnUpContent);
            this.splitContainer2.Panel2.Controls.Add(this.btnUp);
            this.splitContainer2.Panel2.Controls.Add(this.label16);
            this.splitContainer2.Panel2.Controls.Add(this.label14);
            this.splitContainer2.Panel2.Controls.Add(this.label12);
            this.splitContainer2.Panel2.Controls.Add(this.label10);
            this.splitContainer2.Panel2.Controls.Add(this.label8);
            this.splitContainer2.Panel2.Controls.Add(this.label15);
            this.splitContainer2.Panel2.Controls.Add(this.label13);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox11);
            this.splitContainer2.Panel2.Controls.Add(this.label11);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox9);
            this.splitContainer2.Panel2.Controls.Add(this.label9);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox7);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox10);
            this.splitContainer2.Panel2.Controls.Add(this.label7);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox8);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox5);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox6);
            this.splitContainer2.Panel2.Controls.Add(this.btnStart);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox4);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox3);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox2);
            this.splitContainer2.Panel2.Controls.Add(this.checkBox1);
            this.splitContainer2.Panel2.Controls.Add(this.cmbQun);
            this.splitContainer2.Panel2.Controls.Add(this.txtMoney);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.txtContent);
            this.splitContainer2.Panel2.Controls.Add(this.label6);
            this.splitContainer2.Panel2.Controls.Add(this.xuan);
            this.splitContainer2.Panel2.Controls.Add(this.label5);
            this.splitContainer2.Panel2.Controls.Add(this.txtWxUserName);
            this.splitContainer2.Panel2.Controls.Add(this.label3);
            this.splitContainer2.Size = new System.Drawing.Size(1278, 959);
            this.splitContainer2.SplitterDistance = 632;
            this.splitContainer2.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(1278, 632);
            this.dataGridView1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 18);
            this.label3.TabIndex = 0;
            this.label3.Text = "玩家名称";
            // 
            // txtWxUserName
            // 
            this.txtWxUserName.Location = new System.Drawing.Point(97, 34);
            this.txtWxUserName.Name = "txtWxUserName";
            this.txtWxUserName.Size = new System.Drawing.Size(211, 28);
            this.txtWxUserName.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(328, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 18);
            this.label4.TabIndex = 0;
            this.label4.Text = "金额";
            // 
            // txtMoney
            // 
            this.txtMoney.Location = new System.Drawing.Point(389, 38);
            this.txtMoney.Name = "txtMoney";
            this.txtMoney.Size = new System.Drawing.Size(91, 28);
            this.txtMoney.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 82);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 18);
            this.label5.TabIndex = 0;
            this.label5.Text = "下注内容";
            // 
            // txtContent
            // 
            this.txtContent.Location = new System.Drawing.Point(97, 82);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(383, 28);
            this.txtContent.TabIndex = 1;
            // 
            // xuan
            // 
            this.xuan.AutoSize = true;
            this.xuan.Location = new System.Drawing.Point(11, 142);
            this.xuan.Name = "xuan";
            this.xuan.Size = new System.Drawing.Size(80, 18);
            this.xuan.TabIndex = 0;
            this.xuan.Text = "选择群组";
            // 
            // cmbQun
            // 
            this.cmbQun.FormattingEnabled = true;
            this.cmbQun.Location = new System.Drawing.Point(132, 133);
            this.cmbQun.Name = "cmbQun";
            this.cmbQun.Size = new System.Drawing.Size(323, 26);
            this.cmbQun.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 182);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(134, 18);
            this.label6.TabIndex = 0;
            this.label6.Text = "开奖发送结果图";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(167, 181);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(52, 22);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "开";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(498, 142);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(174, 76);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "开始";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(846, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = "下注成功提示";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(988, 30);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(52, 22);
            this.checkBox2.TabIndex = 3;
            this.checkBox2.Text = "开";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(1210, 30);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(52, 22);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Text = "开";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1068, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 18);
            this.label8.TabIndex = 5;
            this.label8.Text = "错误格式提示";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(988, 76);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(52, 22);
            this.checkBox4.TabIndex = 3;
            this.checkBox4.Text = "开";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(1210, 76);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(52, 22);
            this.checkBox5.TabIndex = 3;
            this.checkBox5.Text = "开";
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(846, 75);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 18);
            this.label9.TabIndex = 5;
            this.label9.Text = "余额不足提示";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(1068, 75);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 18);
            this.label10.TabIndex = 5;
            this.label10.Text = "投注限制提示";
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(988, 127);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(52, 22);
            this.checkBox6.TabIndex = 3;
            this.checkBox6.Text = "开";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Location = new System.Drawing.Point(1210, 127);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Size = new System.Drawing.Size(52, 22);
            this.checkBox7.TabIndex = 3;
            this.checkBox7.Text = "开";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(846, 126);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(116, 18);
            this.label11.TabIndex = 5;
            this.label11.Text = "显示中奖名单";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(1068, 126);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(116, 18);
            this.label12.TabIndex = 5;
            this.label12.Text = "支持识别撤回";
            // 
            // checkBox8
            // 
            this.checkBox8.AutoSize = true;
            this.checkBox8.Location = new System.Drawing.Point(988, 165);
            this.checkBox8.Name = "checkBox8";
            this.checkBox8.Size = new System.Drawing.Size(52, 22);
            this.checkBox8.TabIndex = 3;
            this.checkBox8.Text = "开";
            this.checkBox8.UseVisualStyleBackColor = true;
            // 
            // checkBox9
            // 
            this.checkBox9.AutoSize = true;
            this.checkBox9.Location = new System.Drawing.Point(1210, 165);
            this.checkBox9.Name = "checkBox9";
            this.checkBox9.Size = new System.Drawing.Size(52, 22);
            this.checkBox9.TabIndex = 3;
            this.checkBox9.Text = "开";
            this.checkBox9.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(846, 164);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(116, 18);
            this.label13.TabIndex = 5;
            this.label13.Text = "托自动上下分";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(1068, 164);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(116, 18);
            this.label14.TabIndex = 5;
            this.label14.Text = "托不计入统计";
            // 
            // checkBox10
            // 
            this.checkBox10.AutoSize = true;
            this.checkBox10.Location = new System.Drawing.Point(988, 208);
            this.checkBox10.Name = "checkBox10";
            this.checkBox10.Size = new System.Drawing.Size(52, 22);
            this.checkBox10.TabIndex = 3;
            this.checkBox10.Text = "开";
            this.checkBox10.UseVisualStyleBackColor = true;
            // 
            // checkBox11
            // 
            this.checkBox11.AutoSize = true;
            this.checkBox11.Location = new System.Drawing.Point(1210, 208);
            this.checkBox11.Name = "checkBox11";
            this.checkBox11.Size = new System.Drawing.Size(52, 22);
            this.checkBox11.TabIndex = 3;
            this.checkBox11.Text = "开";
            this.checkBox11.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(846, 207);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(116, 18);
            this.label15.TabIndex = 5;
            this.label15.Text = "封盘下单提示";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(1068, 207);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(116, 18);
            this.label16.TabIndex = 5;
            this.label16.Text = "余额不足提示";
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(498, 34);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(75, 32);
            this.btnUp.TabIndex = 6;
            this.btnUp.Text = "上分";
            this.btnUp.UseVisualStyleBackColor = true;
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(579, 35);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(75, 32);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "下分";
            this.btnDown.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(660, 35);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 32);
            this.btnUpdate.TabIndex = 6;
            this.btnUpdate.Text = "改分";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(741, 35);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(99, 32);
            this.btnDel.TabIndex = 6;
            this.btnDel.Text = "删除玩家";
            this.btnDel.UseVisualStyleBackColor = true;
            // 
            // btnUpContent
            // 
            this.btnUpContent.Location = new System.Drawing.Point(498, 78);
            this.btnUpContent.Name = "btnUpContent";
            this.btnUpContent.Size = new System.Drawing.Size(100, 32);
            this.btnUpContent.TabIndex = 6;
            this.btnUpContent.Text = "更改下注";
            this.btnUpContent.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(614, 78);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 32);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "清空下注";
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // frmMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1530, 1028);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.bottomStatus);
            this.Name = "frmMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "操作台";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmMainForm_Load);
            this.bottomStatus.ResumeLayout(false);
            this.bottomStatus.PerformLayout();
            this.tab.ResumeLayout(false);
            this.tabPankou.ResumeLayout(false);
            this.tabPankou.PerformLayout();
            this.tabZhangDan.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip bottomStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblFinish;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.TabControl tab;
        private System.Windows.Forms.TabPage tabPankou;
        private System.Windows.Forms.TabPage tabZhangDan;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tabZhiling;
        private System.Windows.Forms.TabPage tabXiaoxi;
        private System.Windows.Forms.TabPage tabYingkui;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPankou;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWxUserName;
        private System.Windows.Forms.TextBox txtMoney;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label xuan;
        private System.Windows.Forms.ComboBox cmbQun;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBox11;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBox9;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkBox7;
        private System.Windows.Forms.CheckBox checkBox10;
        private System.Windows.Forms.CheckBox checkBox8;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnUpContent;
        private System.Windows.Forms.Button btnClear;
    }
}