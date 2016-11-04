using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WxGames.HTTP;

namespace WxGames
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            DoLogin();
        }

        private void DoLogin()
        {
            //检查配置文件是否存在，不存在，提示后退出
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\\Config.config"))
            {
                MessageBox.Show("缺少配置文件，请配置后再启动");
                Environment.Exit(0);
            }


            ////先check客户端是否合法
            //string msg = PanKou.Instance.Check();

            //if (!string.IsNullOrEmpty(msg))
            //{
            //    MessageBox.Show(msg);
            //    this.Close();
            //}

            picLogin.Image = null;
            picLogin.SizeMode = PictureBoxSizeMode.Zoom;

            ((Action)delegate () {
                //Log.WriteLogByDate("登陆开始");
                //异步加载二维码
                LoginService ls = new LoginService();
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
                            this.BeginInvoke((Action)delegate() {
                                picLogin.SizeMode = PictureBoxSizeMode.CenterImage;
                                picLogin.Image = login_result as Image;
                            });
                        }
                        if (login_result is string)//已完成登录
                        {
                            //访问登录跳转URL
                            ls.GetSidUid(login_result as string);

                            //打开主界面
                            this.BeginInvoke((Action)delegate ()
                            {
                                this.Hide();
                                frmMainForm frmmf = new frmMainForm();
                                frmmf.Show();
                            });
                            break;
                        }
                    }

                }

            }).BeginInvoke(null,null);
        }
    }
}
