using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.Tests
{
    [TestClass()]
    public class OrderManagerTests
    {
        [TestMethod()]
        public void ToOrderTest()
        {
            #region 上下查分
            ////上分指令
            //Order order=OrderManager.Instance.ToOrder("上分100");
            //Assert.IsTrue(order.CommandType == OrderType.上下查);
            //Assert.IsTrue(order.Score == "100");
            //Assert.IsTrue(order.CommandOne == "上");
            //Assert.IsTrue(order.OrderContent == "上分100");

            ////下分指令
            //Order order2 = OrderManager.Instance.ToOrder("下分100");
            //Assert.IsTrue(order2.CommandType == OrderType.上下查);
            //Assert.IsTrue(order2.Score == "100");
            //Assert.IsTrue(order2.CommandOne == "下");
            //Assert.IsTrue(order2.OrderContent == "下分100");

            ////查分指令
            //Order order3 = OrderManager.Instance.ToOrder("查");
            //Assert.IsTrue(order3.CommandType == OrderType.上下查);
            //Assert.IsTrue(order3.Score == "");
            //Assert.IsTrue(order3.CommandOne == "查");
            //Assert.IsTrue(order3.OrderContent == "查");
            #endregion

            //#region 名次大小单双龙虎
            //Order order4 = OrderManager.Instance.ToOrder("5虎1");
            //Assert.IsTrue(order4.CommandOne == "3");
            //Assert.IsTrue(order4.CommandTwo == "虎");
            //Assert.IsTrue(order4.CommandType == OrderType.名次大小单双龙虎);
            //Assert.IsTrue(order4.OrderContent == "3虎100");
            //Assert.IsTrue(order4.Score == "100");

            //Order order5 = OrderManager.Instance.ToOrder("五双100");
            //Assert.IsTrue(order5.CommandOne == "五");
            //Assert.IsTrue(order5.CommandTwo == "双");
            //Assert.IsTrue(order5.CommandType == OrderType.名次大小单双龙虎);
            //Assert.IsTrue(order5.OrderContent == "五双100");
            //Assert.IsTrue(order5.Score == "100");
            //#endregion

            //#region 买名次
            ////类型一
            //Order order6 = OrderManager.Instance.ToOrder("123/12670/100");
            //Assert.IsTrue(order6.CommandOne == "123");
            //Assert.IsTrue(order6.CommandTwo == "12670");
            //Assert.IsTrue(order6.CommandType == OrderType.买名次);
            //Assert.IsTrue(order6.OrderContent == "123/12670/100");
            //Assert.IsTrue(order6.Score == "100");

            ////类型二
            //Order order7 = OrderManager.Instance.ToOrder("冠1230/200");
            //Assert.IsTrue(order7.CommandOne == "冠");
            //Assert.IsTrue(order7.CommandTwo == "1230");
            //Assert.IsTrue(order7.CommandType == OrderType.买名次);
            //Assert.IsTrue(order7.OrderContent == "冠1230/200");
            //Assert.IsTrue(order7.Score == "200");

            ////类型三
            //Order order8 = OrderManager.Instance.ToOrder("12345.200");
            //Assert.IsTrue(order8.CommandOne == "冠");
            //Assert.IsTrue(order8.CommandTwo == "12345");
            //Assert.IsTrue(order8.CommandType == OrderType.买名次);
            //Assert.IsTrue(order8.OrderContent == "12345.200");
            //Assert.IsTrue(order8.Score == "200");
            //#endregion

            //#region 买冠亚和
            //Order order9 = OrderManager.Instance.ToOrder("和19/200");
            //Assert.IsTrue(order9.CommandOne == "和");
            //Assert.IsTrue(order9.CommandTwo == "19/");
            //Assert.IsTrue(order9.CommandType == OrderType.冠亚和);
            //Assert.IsTrue(order9.OrderContent == "和19/200");
            //Assert.IsTrue(order9.Score == "200");

            //Order order10 = OrderManager.Instance.ToOrder("和13/19/200");
            //Assert.IsTrue(order10.CommandOne == "和");
            //Assert.IsTrue(order10.CommandTwo == "13/19/");
            //Assert.IsTrue(order10.CommandType == OrderType.冠亚和);
            //Assert.IsTrue(order10.OrderContent == "和13/19/200");
            //Assert.IsTrue(order10.Score == "200");

            //#endregion

            //#region 非法指令
            //Order order11 = OrderManager.Instance.ToOrder("和");
            //Assert.IsTrue(order11.CommandType == OrderType.指令格式错误);

            //Order order12 = OrderManager.Instance.ToOrder("七八200");
            //Assert.IsTrue(order12.CommandType == OrderType.指令格式错误);
            //#endregion

            Order order4 = OrderManager.Instance.ToOrder("和345678910111213141516171819/10");
            Assert.IsTrue(order4.CommandOne == "3");
            Assert.IsTrue(order4.CommandTwo == "虎");
            Assert.IsTrue(order4.CommandType == OrderType.名次大小单双龙虎);
            Assert.IsTrue(order4.OrderContent == "3虎100");
            Assert.IsTrue(order4.Score == "100");

            Order order11 = OrderManager.Instance.ToOrder("123龙1");
            Assert.IsTrue(order11.CommandType == OrderType.指令格式错误);

        }
    }
}