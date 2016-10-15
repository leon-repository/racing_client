using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace BLL
{
    public class PanKou
    {
        public static readonly PanKou Instance = new PanKou();

        public static string accessKey = "";

        public static string securityKey = "";

        public bool IsLogin()
        {
            //读取数据库

            return true;
        }

        public bool Login(string userName, string password,ref string msg)
        {
            //验证用户名密码
            string json = WebService.SendPostRequest2(ConfigHelper.GetXElementNodeValue("Client", "url")+"/user/client/login", "{\"userName\":\"" + userName + "\",\"password\":\"" + password + "\"}");

            JObject result = JsonConvert.DeserializeObject(json) as JObject;

            string succ = result["result"].ToString();
            if (succ == "SUCCESS")
            {
                accessKey = result["data"]["accessKey"].ToString();
                securityKey = result["data"]["securityKey"].ToString();
            }
            else
            {
                msg = result["message"].ToString();
                return false;
            }

            return true;
        }

        public string Check()
        {
            string json = WebService.SendGetRequest2(ConfigHelper.GetXElementNodeValue("Client", "url")+"/user/client/login/check","","");

            JObject result = JsonConvert.DeserializeObject(json) as JObject;

            if (result != null)
            {
                string succ = result["result"].ToString();
                string msg = result["message"].ToString();
                if (succ == "SUCCESS")
                {
                    return "";
                }
                else
                {
                    return msg;
                }
            }
            return null;
        }

        public string Test()
        {
            string msg = "";
            Login("usertest", "123456",ref msg);

            string body = "[{\"memberId\": 0,\"racingNum\": \"string\",\"stakeVo\": {\"appointStakeList\": [{\"carNum\": 0,\"eighth\": 0,\"fifth\": 0,\"first\": 0,\"fourth\": 0,\"id\": 0,\"ninth\": 0,\"second\": 0,\"seventh\": 0,\"sixth\": 0,\"tenth\": 0,\"third\": 0}],\"commonStake\": {\"fifthDowm\": 0,\"fifthUp\": 0,\"firstDowm\": 0,\"firstSecond10\": 0,\"firstSecond11\": 0,\"firstSecond12\": 0,\"firstSecond13\": 0,\"firstSecond14\": 0,\"firstSecond15\": 0,\"firstSecond16\": 0,\"firstSecond17\": 0,\"firstSecond18\": 0,\"firstSecond19\": 0,\"firstSecond3\": 0,\"firstSecond4\": 0,\"firstSecond5\": 0,\"firstSecond6\": 0,\"firstSecond7\": 0,\"firstSecond8\": 0,\"firstSecond9\": 0,\"firstSecondBig\": 0,\"firstSecondEven\": 0,\"firstSecondOdd\": 0,\"firstSecondSmall\": 0,\"firstUp\": 0,\"fourthDowm\": 0,\"fourthUp\": 0,\"id\": 0,\"secondDowm\": 0,\"secondUp\": 0,\"thirdDowm\": 0,\"thirdUp\": 0},\"racing_num\": \"string\",\"rankingStakeList\": [{\"big\": 0,\"even\": 0,\"id\": 0,\"odd\": 0,\"rankingNum\": 0,\"small\": 0}]}}]";
            body = body.Replace(" ", "");
            body = Regex.Replace(body, "\\s{2,}", ",");

            string auth = "/member/stake" + body + securityKey;
            byte[] data= Encoding.UTF8.GetBytes(auth);
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] result = sha.ComputeHash(data);
            string authion= BitConverter.ToString(result).Replace("-","");
            authion = authion.ToUpper();

            WebService.SendPostRequest2("http://60.205.163.65:8080/member/stake", body, authion,accessKey);

            return "";
        }

        public string GetSha1(string body, string url)
        {
            body = body.Replace(" ", "");
            body = Regex.Replace(body, "\\s{2,}", ",");

            string auth = url + body + securityKey;
            byte[] data = Encoding.UTF8.GetBytes(auth);
            SHA1 sha = new SHA1CryptoServiceProvider();

            byte[] result = sha.ComputeHash(data);
            string authion = BitConverter.ToString(result).Replace("-", "");
            authion = authion.ToUpper();

            return authion;
        }

        public List<string> PankouList()
        {
            List<string> list = new List<string>();
            list.Add("盘口：上海");
            list.Add("盘口：北京");

            return list;
        }
    }
}
