using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BLL
{
    /// <summary>
    /// 配置文件管理类
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static string ConfigPath = "";

        /// <summary>
        /// XML的全部内容
        /// </summary>
        public static XElement Xml = null;

        static ConfigHelper()
        {
            ConfigPath = AppDomain.CurrentDomain.BaseDirectory + @"\\Config.config";

            if (!File.Exists(ConfigPath))
            {
                throw new ArgumentException("配置文件不存在，请在网站根目录下添加//conf//Config.config");
            }

            Xml = XElement.Load(ConfigPath);
        }

        /// <summary>
        /// 获取全部config内容
        /// </summary>
        /// <returns></returns>
        public static XElement Load()
        {
            return Xml;
        }

        /// <summary>
        /// 获取指定节点列表
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> GetXElements(string nodeName)
        {
            return Xml.Elements(nodeName);
        }

        /// <summary>
        /// 获取指定节点
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static XElement GetXElement(string nodeName)
        {
            return Xml.Element(nodeName);
        }

        /// <summary>
        /// 获取指定节点的value
        /// </summary>
        /// <param name="xElement"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string GetNodeValue(XElement xElement, string nodeName)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException("xElement", "xElement不能为空");
            }
            return xElement.Element(nodeName).Value;
        }

        /// <summary>
        /// 获取nodeName下nodeChildName的值
        /// </summary>
        /// <param name="nodeName">父节点</param>
        /// <param name="nodeChildName">子节点</param>
        /// <returns></returns>
        public static string GetXElementNodeValue(string nodeName, string nodeChildName)
        {
           return Xml.Element(nodeName).Element(nodeChildName).Value;
        }

        public static List<KeyValuePair<string, object>> GetList(string nodeName, string childNodeId, string childNodeName)
        {
            IEnumerable<XElement> payChannels = ConfigHelper.GetXElements(nodeName);
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
            foreach (var item in payChannels)
            {
                list.Add(new KeyValuePair<string, object>(ConfigHelper.GetNodeValue(item, childNodeId), ConfigHelper.GetNodeValue(item, childNodeName)));
            }

            return list;
        }


        /// <summary>
        /// 获取一个ArrayList数组
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="childNodeId"></param>
        /// <param name="childNodeName"></param>
        /// <param name="isAll">是否存在全部</param>
        /// <returns></returns>
        public static ArrayList GetArrayList(string nodeName, string childNodeId, string childNodeName, bool isAll)
        {
            IEnumerable<XElement> feeType = ConfigHelper.GetXElements(nodeName);

            ArrayList arrayList = new ArrayList();
            foreach (var item in feeType)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary.Add("id", ConfigHelper.GetNodeValue(item, childNodeId));
                dictionary.Add("text", ConfigHelper.GetNodeValue(item, childNodeName));
                arrayList.Add(dictionary);
            }

            if (isAll)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id", "");
                dic.Add("text", "全部");
                dic.Add("children", arrayList);

                ArrayList array = new ArrayList();
                array.Add(dic);
                return array;
            }
            return arrayList;
        }
    }
}
