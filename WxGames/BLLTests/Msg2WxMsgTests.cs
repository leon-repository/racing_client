using Microsoft.VisualStudio.TestTools.UnitTesting;
using WxGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace WxGames.Tests
{
    [TestClass()]
    public class Msg2WxMsgTests
    {
        [TestMethod()]
        public void GetMsgTest()
        {
            NowMsg msg = new NowMsg();

            WXMsg wxMsg = Msg2WxMsg.Instance.GetMsg(msg);
            Assert.IsNull(wxMsg);

            //msg = new NowMsg();
            //msg.MsgId = "8923210472891488273";
            //msg.MsgFromId = "2328346900";
            //msg.MsgFromName = "厚德载物";
            //msg.ReciveId = "@a35e875ec635c11a43d86835d24189c1609d73c936588dcbe6e13d7d2b3573a7";
            //msg.ReciveName = "@a35e875ec635c11a43d86835d24189c1609d73c936588dcbe6e13d7d2b3573a7";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "买名次";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "6/4/20";
            //msg.CommandTwo = "4";
            //msg.Score = "20";
            //msg.CommandType = "买名次";
            //msg.Period = null;
            //msg.CommandOne = "6";

            //1，无积分 买名次
            //WXMsg wxMsg2 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg2.From == null);
            //Assert.IsTrue(wxMsg2.Type == 1);
            //Assert.IsTrue(wxMsg2.Msg == "@厚德载物 积分不足<br/>当前积分：0");

            //2，有积分 买名次
            //WXMsg wxMsg3= Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg3.From == null);
            //Assert.IsTrue(wxMsg3.Type == 1);
            //Assert.IsTrue(wxMsg3.Msg.ToString() == "@厚德载物  下注成功<br/>六名[4]20<br/>当前积分：760".ToString());

            //3,无积分 买冠军
            //msg = new NowMsg();
            //msg.MsgId = "7825310389402061516";
            //msg.MsgFromId = "2328346900";
            //msg.MsgFromName = "厚德载物";
            //msg.ReciveId = "@51735995a2af400b7e8bc5bbc54bb06951bc4ba61e6ca537119d2a44ae1dc182";
            //msg.ReciveName = "@51735995a2af400b7e8bc5bbc54bb06951bc4ba61e6ca537119d2a44ae1dc182";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "买名次";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "2/6";
            //msg.CommandTwo = "2";
            //msg.Score = "6";
            //msg.CommandType = "买名次";
            //msg.Period = null;
            //msg.CommandOne = "冠";

            //WXMsg wxMsg3 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg3.Msg.ToString() == "@厚德载物 积分不足<br/>当前积分：0".ToString());

            //4,有积分 买冠军
            //msg = new NowMsg();
            //msg.MsgId = "7825310389402061516";
            //msg.MsgFromId = "2328346900";
            //msg.MsgFromName = "厚德载物";
            //msg.ReciveId = "@51735995a2af400b7e8bc5bbc54bb06951bc4ba61e6ca537119d2a44ae1dc182";
            //msg.ReciveName = "@51735995a2af400b7e8bc5bbc54bb06951bc4ba61e6ca537119d2a44ae1dc182";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "买名次";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "2/6";
            //msg.CommandTwo = "2";
            //msg.Score = "6";
            //msg.CommandType = "买名次";
            //msg.Period = null;
            //msg.CommandOne = "冠";

            //WXMsg wxMsg4 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg4.Msg.ToString() == "@厚德载物  下注成功<br/>冠军[2]6 <br/>当前积分：488".ToString());


            //5,无积分 买名次大小单双龙虎
            //msg = new NowMsg();
            //msg.MsgId = "8564475898164854124";
            //msg.MsgFromId = "437364022";
            //msg.MsgFromName = "浅雪沁心";
            //msg.ReciveId = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.ReciveName = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "名次大小单双龙虎";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "五双100";
            //msg.CommandTwo = "双";
            //msg.Score = "100";
            //msg.CommandType = "名次大小单双龙虎";
            //msg.Period = null;
            //msg.CommandOne = "五";
            //WXMsg wxMsg4 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg4.Msg.ToString() == "@浅雪沁心 积分不足<br/>当前积分：0".ToString());

            //6,有积分 买名次大小单双龙虎

            //msg = new NowMsg();
            //msg.MsgId = "8564475898164854124";
            //msg.MsgFromId = "437364022";
            //msg.MsgFromName = "浅雪沁心";
            //msg.ReciveId = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.ReciveName = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "买名次大小单双龙虎";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "五双100";
            //msg.CommandTwo = "双";
            //msg.Score = "100";
            //msg.CommandType = "买名次大小单双龙虎";
            //msg.Period = null;
            //msg.CommandOne = "五";
            //WXMsg wxMsg4 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg4.Msg.ToString() == "@浅雪沁心  下注成功<br/>五名 双 100<br/>当前积分：200".ToString());

            //7,无积分 冠亚和
            //msg = new NowMsg();
            //msg.MsgId = "8564475898164854124";
            //msg.MsgFromId = "437364022";
            //msg.MsgFromName = "浅雪沁心";
            //msg.ReciveId = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.ReciveName = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "冠亚和";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "和13/19/200";
            //msg.CommandTwo = "3/19/";
            //msg.Score = "200";
            //msg.CommandType = "冠亚和";
            //msg.Period = null;
            //msg.CommandOne = "和";
            //WXMsg wxMsg7 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg7.Msg.ToString() == "@浅雪沁心 积分不足<br/>当前积分：0".ToString());

            //8,有积分 冠亚和
            //msg = new NowMsg();
            //msg.MsgId = "8564475898164854124";
            //msg.MsgFromId = "437364022";
            //msg.MsgFromName = "浅雪沁心";
            //msg.ReciveId = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.ReciveName = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "冠亚和";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "和13/19/50";
            //msg.CommandTwo = "13/19/";
            //msg.Score = "50";
            //msg.CommandType = "冠亚和";
            //msg.Period = null;
            //msg.CommandOne = "和";
            //WXMsg wxMsg8 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg8.Msg.ToString() == "@浅雪沁心  下注成功<br/>和 13 50<br/>和 19 50<br/>当前积分：350".ToString());

            //9,取消
            msg = new NowMsg();
            msg.MsgId = "8564475898164854124";
            msg.MsgFromId = "437364022";
            msg.MsgFromName = "浅雪沁心";
            msg.ReciveId = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            msg.ReciveName = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            msg.MsgContent = "";
            msg.CreateDate = "1475163056";
            msg.OpDate = "星期四 2016-9-29 23:58:49";
            msg.IsSucc = 0;
            msg.IsDelete = "0";
            msg.IsMsg = "取消";
            msg.IsDeal = "0";
            msg.Result = null;
            msg.OrderContect = "和13/19/50";
            msg.CommandTwo = "13/19/";
            msg.Score = "50";
            msg.CommandType = "取消";
            msg.Period = null;
            msg.CommandOne = "和";
            WXMsg wxMsg8 = Msg2WxMsg.Instance.GetMsg(msg);
            Assert.IsTrue(wxMsg8.Msg.ToString() == "@浅雪沁心 暂不支持取消指令".ToString());


            //10,指令格式错误
            //msg = new NowMsg();
            //msg.MsgId = "8564475898164854124";
            //msg.MsgFromId = "437364022";
            //msg.MsgFromName = "浅雪沁心";
            //msg.ReciveId = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.ReciveName = "@2b0c0c0c27c7075e3c092b6d159bcacb126bd42afb8d2c5ec877439c2ced00fd";
            //msg.MsgContent = "";
            //msg.CreateDate = "1475163056";
            //msg.OpDate = "星期四 2016-9-29 23:58:49";
            //msg.IsSucc = 0;
            //msg.IsDelete = "0";
            //msg.IsMsg = "指令格式错误";
            //msg.IsDeal = "0";
            //msg.Result = null;
            //msg.OrderContect = "和13/19/50";
            //msg.CommandTwo = "13/19/";
            //msg.Score = "50";
            //msg.CommandType = "指令格式错误";
            //msg.Period = null;
            //msg.CommandOne = "和";
            //WXMsg wxMsg10 = Msg2WxMsg.Instance.GetMsg(msg);
            //Assert.IsTrue(wxMsg10.Msg.ToString() == "@浅雪沁心 指令格式错误".ToString());


            //11,暂不支持此指令，异常情况
        }

        [TestMethod()]
        public void GetMsg2Test()
        {
            NowMsg msg = new NowMsg();
            msg = new NowMsg();
            msg.MsgId = "7825310389402061516";
            msg.MsgFromId = "2328346900";
            msg.MsgFromName = "厚德载物";
            msg.ReciveId = "@51735995a2af400b7e8bc5bbc54bb06951bc4ba61e6ca537119d2a44ae1dc182";
            msg.ReciveName = "@51735995a2af400b7e8bc5bbc54bb06951bc4ba61e6ca537119d2a44ae1dc182";
            msg.MsgContent = "";
            msg.CreateDate = "1475163056";
            msg.OpDate = "星期四 2016-9-29 23:58:49";
            msg.IsSucc = 0;
            msg.IsDelete = "0";
            msg.IsMsg = "买名次";
            msg.IsDeal = "0";
            msg.Result = null;
            msg.OrderContect = "2/6";
            msg.CommandTwo = "2";
            msg.Score = "6";
            msg.CommandType = "买名次";
            msg.Period = null;
            msg.CommandOne = "冠";

            //WXMsg wxMsg4 = Msg2WxMsg.Instance.GetMsg2(msg);
            //Assert.IsTrue(wxMsg4.Msg.ToString() == "@厚德载物  下注成功<br/>冠军[2]6 <br/>当前积分：488".ToString());

            //Assert.Fail();
        }
    }
}