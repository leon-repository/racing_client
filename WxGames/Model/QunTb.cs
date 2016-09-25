using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Model
{
    public class QunTb
    {
        /// <summary>
        /// 群UserName
        /// </summary>
        [Key]
        public string UserName { get; set; }

        /// <summary>
        /// 群昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 是否开始
        /// </summary>
        public int IsStart { get; set; }
    }
}
