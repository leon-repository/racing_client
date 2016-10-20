using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace WxGames
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //处理未捕获的异常   
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常   
                Application.ThreadException += Application_ThreadException;
                //处理非UI线程异常   
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.Run(new Login());
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
               // MessageBox.Show("系统出现未知异常，请重启系统！");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject;
            if (ex != null)
            {
                Log.WriteLog((Exception)ex);
            }

            //MessageBox.Show("系统出现未知异常，请重启系统！");
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            if (ex != null)
            {
               Log.WriteLog(ex);
            }

            //MessageBox.Show("系统出现未知异常，请重启系统！");
        }
    }
}
