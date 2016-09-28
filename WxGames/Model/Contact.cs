using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 微信群联系人
    /// </summary>
    public class Contact
    {
        [Key]
        public string Uuid { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Uin { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 拼音全拼
        /// </summary>
        public string QuanPin { get; set; }

        /// <summary>
        /// 群UserName
        /// </summary>
        public string QuserName { get; set; }

        /// <summary>
        /// 群昵称
        /// </summary>
        public string QnickName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
