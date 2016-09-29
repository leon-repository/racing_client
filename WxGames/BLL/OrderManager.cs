using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    /// <summary>
    /// 指令转化
    /// </summary>
    public class OrderManager
    {
        private OrderManager() { }
        public static readonly OrderManager Instance = new OrderManager();

        public Order ToOrder(string order)
        {
            Order result = new Order();

            ///上分，下分，查分
            List<KeyValuePair<string, string>> daKey = new List<KeyValuePair<string, string>>();
            daKey.Add(new KeyValuePair<string, string>("上", "上|上分"));
            daKey.Add(new KeyValuePair<string, string>("下", "下|下分"));
            daKey.Add(new KeyValuePair<string, string>("查", "查|查分"));

            foreach (KeyValuePair<string,string> pair in daKey)
            {
                string[] key = pair.Value.Split(new char[] {'|'},StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < key.Length; i++)
                {
                    if (order.Contains(key[i]))
                    {
                        result.OrderContent = order;
                        result.CommandOne = key[i];
                        result.Score = order.GetNumber();
                        result.CommandType = OrderType.上下查;

                        return result;
                    }
                }
            }

            ///大小单双龙虎
            List<KeyValuePair<string, string>> longKey = new List<KeyValuePair<string, string>>();
            longKey.Add(new KeyValuePair<string, string>("大", "大"));
            longKey.Add(new KeyValuePair<string, string>("小", "小"));
            longKey.Add(new KeyValuePair<string, string>("单", "单"));
            longKey.Add(new KeyValuePair<string, string>("双", "双"));
            longKey.Add(new KeyValuePair<string, string>("龙", "龙"));
            longKey.Add(new KeyValuePair<string, string>("虎", "虎"));

            foreach (KeyValuePair<string, string> pair in longKey)
            {
                string[] key = pair.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < key.Length; i++)
                {
                    if (order.Contains(key[i]))
                    {
                        result.OrderContent = order;

                        string[] command = order.Split(new string[] { key[i].ToString()}, StringSplitOptions.RemoveEmptyEntries);
                        if (command.Length != 2)
                        {
                            continue;
                        }
                        result.CommandOne = command[0];
                        result.CommandTwo = key[i];
                        result.Score = command[1];
                        result.CommandType = OrderType.名次大小单双龙虎;
                        return result;
                    }
                }
            }

            //买名次
            //指令类型一
            List<KeyValuePair<string, string>> guanKey = new List<KeyValuePair<string, string>>();
            guanKey.Add(new KeyValuePair<string, string>("/","/"));
            foreach (KeyValuePair<string,string> key in guanKey)
            {
                //检查是否存在汉字
                if (order.ExitHanZi())
                {
                    break;
                }

                string[] command = order.Split(new string[] { key.Value }, StringSplitOptions.RemoveEmptyEntries);
                if (command == null)
                {
                    continue;
                }
                if (command.Length != 3)
                {
                    continue;
                }
                

                result.OrderContent = order;
                result.CommandOne = command[0];
                result.CommandTwo = command[1];
                result.Score = command[2];
                result.CommandType = OrderType.买名次;
                return result;
            }

            //指令类型二
            List<KeyValuePair<string, string>> guanKey2 = new List<KeyValuePair<string, string>>();
            guanKey2.Add(new KeyValuePair<string, string>("冠", "冠|冠军"));
            guanKey2.Add(new KeyValuePair<string, string>("亚", "亚|亚军"));
            guanKey2.Add(new KeyValuePair<string, string>("三", "三"));
            guanKey2.Add(new KeyValuePair<string, string>("四", "四"));
            guanKey2.Add(new KeyValuePair<string, string>("五", "五"));
            guanKey2.Add(new KeyValuePair<string, string>("六", "六"));
            guanKey2.Add(new KeyValuePair<string, string>("七", "七"));
            guanKey2.Add(new KeyValuePair<string, string>("八", "八"));
            guanKey2.Add(new KeyValuePair<string, string>("九", "九"));
            guanKey2.Add(new KeyValuePair<string, string>("十", "十"));

            foreach (KeyValuePair<string, string> key in guanKey2)
            {
                string[] keyA = key.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < keyA.Length; i++)
                {
                    if (order.Contains(keyA[i]))
                    {
                        string command=order.Substring(order.IndexOf(keyA[i])+1);
                        string[] commanPair = command.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        if (commanPair == null)
                        {
                            continue;
                        }
                        if (commanPair.Length != 2)
                        {
                            continue;
                        }
                        result.OrderContent = order;
                        result.CommandOne = keyA[i];
                        result.CommandTwo = commanPair[0];
                        result.Score = commanPair[1];
                        result.CommandType = OrderType.买名次;

                        return result;
                    }
                }
            }

            //类型三：冠军简写
            List<KeyValuePair<string, string>> guanKey3 = new List<KeyValuePair<string, string>>();
            guanKey3.Add(new KeyValuePair<string, string>(".", ".|/"));
            foreach (KeyValuePair<string, string> key in guanKey3)
            {
                //检查是否存在汉字
                if (order.ExitHanZi())
                {
                    break;
                }

                string[] keyA = key.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < keyA.Length; i++)
                {
                    string[] command= order.Split(new string[] { keyA[i] }, StringSplitOptions.RemoveEmptyEntries);
                    if (command == null)
                    {
                        continue;
                    }
                    if (command.Length != 2)
                    {
                        continue;
                    }
                    result.CommandOne = "冠";
                    result.CommandTwo = command[0];
                    result.Score = command[1];
                    result.CommandType = OrderType.买名次;
                    result.OrderContent = order;
                    return result;
                }
            }

            //买冠亚和
            List<KeyValuePair<string, string>> heKey = new List<KeyValuePair<string, string>>();
            heKey.Add(new KeyValuePair<string, string>("和", "和|和值"));
            foreach (KeyValuePair<string, string> key in heKey)
            {
                string[] keyA = key.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < keyA.Length; i++)
                {
                    if (order.Contains(keyA[i]))
                    {
                        string command = order.Substring(order.IndexOf(keyA[i])+1);
                        string[] commandA = command.Split(new char[] { '/'},StringSplitOptions.RemoveEmptyEntries);
                        if (commandA == null)
                        {
                            continue;
                        }
                        if (commandA.Length <= 0)
                        {
                            continue;
                        }
                        result.OrderContent = order;
                        result.CommandOne = keyA[i];
                        for (int j = 0; j < commandA.Length-1; j++)
                        {
                            result.CommandTwo +=commandA[j]+"/";
                        }
                        
                        result.Score = commandA[commandA.Length - 1];
                        result.CommandType = OrderType.冠亚和;
                        return result;
                    }
                }
            }

            if (string.IsNullOrEmpty(result.OrderContent))
            {
                result.OrderContent = order;
                result.CommandType = OrderType.指令格式错误;
            }
            return result;
        }
    }
}
