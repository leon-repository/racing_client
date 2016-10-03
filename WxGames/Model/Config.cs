using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 配置表
    /// </summary>
    public class Config
    {
        /// <summary>
        /// uuid
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// 一级分类
        /// 消息类型：MSG
        /// 指令类型：ORDER
        /// 开奖类型：LOTTERY
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 二级分类(可为空)
        /// </summary>
        public string Typetwo { get; set; }

        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// value
        /// </summary>
        public string Value { get; set; }
    }
}
