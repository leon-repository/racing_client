using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 输出日志
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 输出异常日志
        /// </summary>
        /// <param name="objException">异常对象</param>
        public static void WriteLog(Exception objException)
        {
            string logPath = AppDomain.CurrentDomain.BaseDirectory + @"\\Log\\Log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StringBuilder objStr = new StringBuilder();
            objStr.AppendLine("发生时间：\r\n" + DateTime.Now.ToString() + "");
            objStr.AppendLine("提示信息：\r\n" + objException.Message);
            objStr.AppendLine((objException.InnerException == null ? "" : objException.InnerException.Message));
            File.AppendAllText(logPath, objStr.ToString());
        }

        /// <summary>
        /// 输出一条日志（带日期）
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void WriteLogByDate(string msg)
        {
            string logPath = AppDomain.CurrentDomain.BaseDirectory + @"\\Log\\Log" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            StringBuilder objStr = new StringBuilder();
            objStr.AppendLine("发生时间：\r\n" + DateTime.Now.ToString());
            objStr.AppendLine(msg);
            File.AppendAllText(logPath, objStr.ToString());
        }
    }
}
