using Model;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        public static List<Config> OrderConfig = null;

        private DataHelper data = new DataHelper(ConfigurationManager.AppSettings["conn"].ToString());

        private int McDi = 1;

        private int McGao = 10000;

        private int LongDi = 1;

        private int LongGao = 10000;

        private int DanDi = 1;

        private int DanGao = 10000;

        private int DaDi = 1;

        private int DaGao = 10000;

        public Order ToOrder(string order)
        {
            OrderConfig = data.GetList<Config>("type='ORDER'", "");

            Order result = new Order();
            //下注高低限额设置
            Config modelDi = OrderConfig.Find(p => p.Key == "MCDI");
            if (modelDi != null)
            {
                McDi = modelDi.Value.ToInt();
            }
            Config modelGao = OrderConfig.Find(p => p.Key == "MCGAO");
            if (modelGao != null)
            {
                McGao = modelGao.Value.ToInt();
            }

            Config modelLongDi = OrderConfig.Find(p => p.Key == "MCLONGDI");
            if (modelLongDi != null)
            {
                LongDi = modelLongDi.Value.ToInt();
            }

            Config modelLongGao = OrderConfig.Find(p => p.Key == "MCLONGGAO");
            if (modelLongGao != null)
            {
                LongGao = modelLongGao.Value.ToInt();
            }

            Config modelDanDi = OrderConfig.Find(p => p.Key == "MCDANDI");
            if (modelDanDi != null)
            {
                DanDi = modelDanDi.Value.ToInt();
            }

            Config modelDanGao = OrderConfig.Find(p => p.Key == "MCDANGAO");
            if (modelDanGao != null)
            {
                DanGao = modelDanGao.Value.ToInt();
            }

            Config modelDaDi = OrderConfig.Find(p => p.Key == "MCDADI");
            if (modelDaDi != null)
            {
                DaDi = modelDaDi.Value.ToInt();
            }
            Config modelDaGao = OrderConfig.Find(p => p.Key == "MCDAGAO");
            if (modelDaGao != null)
            {
                DaGao = modelDaGao.Value.ToInt();
            }

            List<KeyValuePair<string, string>> quKey = new List<KeyValuePair<string, string>>();
            Config modelQu = OrderConfig.Find(p => p.Key == "MCQU");
            if (modelQu == null)
            {
                quKey.Add(new KeyValuePair<string, string>("取消", "取消"));
            }
            else
            {
                quKey.Add(new KeyValuePair<string, string>("取消", modelQu.Value));
            }
            foreach (KeyValuePair<string, string> pair in quKey)
            {
                string[] key = pair.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < key.Length; i++)
                {
                    if (order.Contains(key[i]))
                    {
                        result.OrderContent = order;
                        result.CommandOne = key[i];
                        result.Score = order.GetNumber();
                        result.CommandType = OrderType.取消;
                        return result;
                    }
                }
            }

            ///上分，下分，查分
            List<KeyValuePair<string, string>> daKey = new List<KeyValuePair<string, string>>();
            Config modelShang = OrderConfig.Find(p => p.Key == "MCSHANG");
            if (modelShang != null)
            {
                daKey.Add(new KeyValuePair<string, string>("上", modelShang.Value));
            }
            else
            {
                daKey.Add(new KeyValuePair<string, string>("上", "上|上分"));
            }
            Config modelXia = OrderConfig.Find(p => p.Key == "MCXIA");
            if (modelXia != null)
            {
                daKey.Add(new KeyValuePair<string, string>("下", modelXia.Value));
            }
            else
            {
                daKey.Add(new KeyValuePair<string, string>("下", "下|下分"));
            }
            Config modelCha = OrderConfig.Find(p => p.Key == "MCCHA");
            if (modelCha != null)
            {
                daKey.Add(new KeyValuePair<string, string>("查", modelCha.Value));
            }
            else
            {
                daKey.Add(new KeyValuePair<string, string>("查", "查|查分"));
            }

            foreach (KeyValuePair<string, string> pair in daKey)
            {
                string[] key = pair.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

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

            //和大,和小,和单,和双
            List<KeyValuePair<string, string>> heDaKey = new List<KeyValuePair<string, string>>();
            heDaKey.Add(new KeyValuePair<string, string>("和大", "和大"));
            heDaKey.Add(new KeyValuePair<string, string>("和小", "和小"));
            heDaKey.Add(new KeyValuePair<string, string>("和单", "和单"));
            heDaKey.Add(new KeyValuePair<string, string>("和双", "和双"));

            foreach (KeyValuePair<string,string> item in heDaKey)
            {
                if(!order.Contains(item.Value))
                {
                    continue;
                }
                string[] arr = order.Split(new string[] { item.Value }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length != 1)
                {
                    result.CommandType = OrderType.指令格式错误;
                    return result;
                }
                else
                {
                    result.CommandOne = item.Value;
                    result.Score = arr[0].ToString();
                    if (result.Score.IsNum())
                    {
                        switch (item.Value)
                        {
                            case "和大":
                                result.CommandType = OrderType.和大;
                                if (result.Score.ToInt() < DaDi || result.Score.ToInt() > DaGao)
                                {
                                    result.CommandType = OrderType.下注积分范围错误;
                                }
                                break;
                            case "和小":
                                result.CommandType = OrderType.和小;
                                if (result.Score.ToInt() < DaDi || result.Score.ToInt() > DaGao)
                                {
                                    result.CommandType = OrderType.下注积分范围错误;
                                }
                                break;
                            case "和单":
                                result.CommandType = OrderType.和单;
                                if (result.Score.ToInt() < DanDi || result.Score.ToInt() > DanGao)
                                {
                                    result.CommandType = OrderType.下注积分范围错误;
                                }
                                break;
                            case "和双":
                                result.CommandType = OrderType.和双;
                                if (result.Score.ToInt() < DanDi || result.Score.ToInt() > DanGao)
                                {
                                    result.CommandType = OrderType.下注积分范围错误;
                                }
                                break;
                            default:
                                result.CommandType = OrderType.指令格式错误;
                                break;
                        }
                    }
                    else
                    {
                        result.CommandType = OrderType.指令格式错误;
                    }
                    return result;
                }
            }


            ///大小单双龙虎
             List<KeyValuePair<string, string>> longKey = new List<KeyValuePair<string, string>>();

            Config modelDa = OrderConfig.Find(p => p.Key == "MCDA");
            if (modelDa == null)
            {
                longKey.Add(new KeyValuePair<string, string>("大", "大"));
            }
            else
            {
                longKey.Add(new KeyValuePair<string, string>("大", modelDa.Value));
            }
            Config modelXiao = OrderConfig.Find(p => p.Key == "MCXIAO");
            if (modelXiao == null)
            {
                longKey.Add(new KeyValuePair<string, string>("小", "小"));
            }
            else
            {
                longKey.Add(new KeyValuePair<string, string>("小", modelXiao.Value));
            }
            Config modelDan = OrderConfig.Find(p => p.Key == "MCDAN");
            if (modelDan == null)
            {
                longKey.Add(new KeyValuePair<string, string>("单", "单"));
            }
            else
            {
                longKey.Add(new KeyValuePair<string, string>("单", modelDan.Value));
            }
            Config modelShuang = OrderConfig.Find(p => p.Key == "MCSHUANG");
            if (modelShuang == null)
            {
                longKey.Add(new KeyValuePair<string, string>("双", "双"));
            }
            else
            {
                longKey.Add(new KeyValuePair<string, string>("双", modelShuang.Value));
            }

            Config modelLong = OrderConfig.Find(p => p.Key == "MCLONG");
            if (modelLong == null)
            {
                longKey.Add(new KeyValuePair<string, string>("龙", "龙"));
            }
            else
            {
                longKey.Add(new KeyValuePair<string, string>("龙", modelLong.Value));
            }
            Config modelHu = OrderConfig.Find(p => p.Key == "MCHU");
            if (modelHu == null)
            {
                longKey.Add(new KeyValuePair<string, string>("虎", "虎"));
            }
            else
            {
                longKey.Add(new KeyValuePair<string, string>("虎", modelHu.Value));
            }

            foreach (KeyValuePair<string, string> pair in longKey)
            {
                string[] key = pair.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < key.Length; i++)
                {
                    if (order.Contains(key[i]))
                    {
                        result.OrderContent = order;

                        string[] command = order.Split(new string[] { key[i].ToString() }, StringSplitOptions.RemoveEmptyEntries);

                        if (command.Length == 1)
                        {
                            result.CommandOne = "1";
                        }
                        else if (command.Length == 2)
                        {
                            if (!command[0].IsNum())
                            {
                                continue;
                            }

                            result.CommandOne = command[0];
                            result.CommandTwo = key[i];
                            result.Score = command[1];
                            result.CommandType = OrderType.名次大小单双龙虎;
                        }
                        else
                        {
                            continue;
                        }
                        //判断积分范围是否正确
                        if (result.CommandTwo == "大" || result.CommandTwo == "小")
                        {
                            if (result.Score.ToInt() < DaDi || result.Score.ToInt() > DaGao)
                            {
                                result.CommandType = OrderType.下注积分范围错误;
                            }
                        }
                        else if (result.CommandTwo == "单" || result.CommandTwo == "双")
                        {
                            if (result.Score.ToInt() < DanDi || result.Score.ToInt() > DanGao)
                            {
                                result.CommandType = OrderType.下注积分范围错误;
                            }
                        }
                        else if (result.CommandTwo == "龙" || result.CommandTwo == "虎")
                        {
                            if (result.Score.ToInt() < LongDi || result.Score.ToInt() > LongGao)
                            {
                                result.CommandType = OrderType.下注积分范围错误;
                            }
                        }

                        if (result.CommandTwo == "龙" || result.CommandTwo == "虎")
                        {
                            if (result.CommandOne.ToInt() >= 6 || result.CommandOne.ToInt() <= 0)
                            {
                                result.CommandType = OrderType.指令格式错误;
                            }
                        }
                        return result;
                    }
                }
            }

            //买名次
            //指令类型一
            List<KeyValuePair<string, string>> guanKey = new List<KeyValuePair<string, string>>();
            guanKey.Add(new KeyValuePair<string, string>("/", "/"));
            foreach (KeyValuePair<string, string> key in guanKey)
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
                if (result.Score.ToInt() < McDi || result.Score.ToInt() > McGao)
                {
                    result.CommandType = OrderType.下注积分范围错误;
                }
                else
                {
                    result.CommandType = OrderType.买名次;
                }
                return result;
            }

            //指令类型二
            List<KeyValuePair<string, string>> guanKey2 = new List<KeyValuePair<string, string>>();

            Config model1 = OrderConfig.Find(p => p.Key == "MC1");
            if (model1 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("冠", "冠|冠军"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("冠", model1.Value));
            }
            Config model2 = OrderConfig.Find(p => p.Key == "MC2");
            if (model2 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("亚", "亚|亚军"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("亚", model2.Value));
            }

            Config model3 = OrderConfig.Find(p => p.Key == "MC3");
            if (model3 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("三", "三"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("三", model3.Value));
            }
            Config model4 = OrderConfig.Find(p => p.Key == "MC4");
            if (model4 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("四", "四"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("四", model4.Value));
            }
            Config model5 = OrderConfig.Find(p => p.Key == "MC5");
            if (model5 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("五", "五"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("五", model5.Value));
            }

            Config model6 = OrderConfig.Find(p => p.Key == "MC6");
            if (model6 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("六", "六"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("六", model6.Value));
            }

            Config model7 = OrderConfig.Find(p => p.Key == "MC7");
            if (model7 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("七", "七"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("七", model7.Value));
            }
            Config model8 = OrderConfig.Find(p => p.Key == "MC8");
            if (model8 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("八", "八"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("八", model8.Value));
            }
            Config model9 = OrderConfig.Find(p => p.Key == "MC9");
            if (model9 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("九", "九"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("九", model9.Value));
            }
            Config model10 = OrderConfig.Find(p => p.Key == "MC10");
            if (model10 == null)
            {
                guanKey2.Add(new KeyValuePair<string, string>("十", "十"));
            }
            else
            {
                guanKey2.Add(new KeyValuePair<string, string>("十", model10.Value));
            }
            foreach (KeyValuePair<string, string> key in guanKey2)
            {
                string[] keyA = key.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                keyA = keyA.OrderByDescending(p => p.Length).ToArray();

                for (int i = 0; i < keyA.Length; i++)
                {
                    if (order.Contains(keyA[i]))
                    {
                        string command = order.Substring(order.IndexOf(keyA[i]) + 1);
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

                        if (result.Score.ToInt() < McDi || result.Score.ToInt() > McGao)
                        {
                            result.CommandType = OrderType.下注积分范围错误;
                        }
                        else
                        {
                            result.CommandType = OrderType.买名次;
                        }

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
                    string[] command = order.Split(new string[] { keyA[i] }, StringSplitOptions.RemoveEmptyEntries);
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

                    if (result.Score.ToInt() < McDi || result.Score.ToInt() > McGao)
                    {
                        result.CommandType = OrderType.下注积分范围错误;
                    }
                    else
                    {
                        result.CommandType = OrderType.买名次;
                    }
                    result.OrderContent = order;
                    return result;
                }
            }

            //买冠亚和
            List<KeyValuePair<string, string>> heKey = new List<KeyValuePair<string, string>>();

            Config modelHe = OrderConfig.Find(p => p.Key == "MCHE");
            if (modelHe == null)
            {
                heKey.Add(new KeyValuePair<string, string>("和", "和|和值"));
            }
            else
            {
                heKey.Add(new KeyValuePair<string, string>("和", modelHe.Value));
            }
            foreach (KeyValuePair<string, string> key in heKey)
            {
                string[] keyA = key.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < keyA.Length; i++)
                {
                    if (order.Contains(keyA[i]))
                    {
                        string command = order.Substring(order.IndexOf(keyA[i]) + 1);
                        string[] commandA = command.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        if (commandA == null)
                        {
                            continue;
                        }
                        if (commandA.Length <= 0)
                        {
                            continue;
                        }
                        if (commandA.Length != 2)
                        {
                            result.CommandType = OrderType.指令格式错误;
                            return result;
                        }
                        result.OrderContent = order;
                        result.CommandOne = keyA[i];
                        result.CommandTwo = commandA[0];
                        result.Score = commandA[1];

                        if (result.Score.IsNum())
                        {
                            result.CommandType = OrderType.冠亚和;
                        }
                        else
                        {
                            result.CommandType = OrderType.指令格式错误;
                        }
                        return result;
                    }
                }
            }

            result.OrderContent = order;
            result.CommandType = OrderType.指令格式错误;
            return result;
        }
    }
}
