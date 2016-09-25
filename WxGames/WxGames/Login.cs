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
            picLogin.Image = null;
            picLogin.SizeMode = PictureBoxSizeMode.Zoom;

            ((Action)delegate () {
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
