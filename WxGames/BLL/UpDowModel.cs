using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 上下分操作
    /// </summary>
    public class UpDowModel
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 指令类型
        /// </summary>
        public string CommandType { get; set; }

        /// <summary>
        /// 是否同意
        /// </summary>
        public string IsSucc { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Uin { get; set; }
    }
}
