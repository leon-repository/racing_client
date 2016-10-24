using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace DrawTool
{
    /// <summary>
    /// 文本位置模型
    /// </summary>
    public class PositionContent
    {
        private string _content;

        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
        private Rectangle rect;

        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }
        public PositionContent()
        { }
        public PositionContent(string c, Rectangle rect)
        {
            Content = c;
            Rect = rect;
        }
    }

    //绘图工具
    public class DrawImage
    {
        //画板
        private Bitmap _map;
        //画刷
        private Graphics _gr;

        //行高:0,title行高；1，数据行行高
        private int[] _rowHeight = new int[2];
        //列宽
        private int[] _colWidth = new int[10];
        //行高比例
        public double[] rowHeightRatio = { 2, 1 };
        //列宽比例
        public double[] colHeightRatio = { 4, 6, 2, 2, 2, 1, 1, 1, 1, 1 };

        //文件保存路径,绝对路径
        private string _savePath;
        //设置文件保存路径(绝对路径)
        public void SetSavePath(string path)
        {
            _savePath = path;
        }
        //获取文件保存路径
        public string GetSavePath()
        {
            return _savePath;
        }

        //画笔
        private Pen _framePen;
        //设置主体边框颜色
        public void SetFramePen(Color color)
        {
            if(color == null)
                color = Color.DarkGray;
            _framePen = new Pen(color);
        }

        //数字背景颜色
        private Dictionary<int, Brush> _numBrush;
        //设置开奖号码背景色
        public void SetNumberBackgroundColor(int num, Color color)
        {
            if (color == null)
                color = Color.White;
            if (_numBrush.ContainsKey(num))
            {
                _numBrush[num] = new SolidBrush(color);
            }
            else
            {
                _numBrush.Add(num, new SolidBrush(color));
            }
        }


        //初始化画板
        public DrawImage(int width, int height)
        {
            _map = new Bitmap(width, height);
            //计算基础行高、列宽
            initRowAndWidth();
            //初始化画笔
            _framePen = Pens.DarkGray;
            //初始化文件保存路径
            _savePath = @"C:\drawImage.png";
            //初始化开奖号码背景色
            _numBrush = new Dictionary<int, Brush>();
            _numBrush.Add(1, Brushes.White);
            _numBrush.Add(2, Brushes.Orange);
            _numBrush.Add(3, Brushes.Blue);
            _numBrush.Add(4, Brushes.Yellow);
            _numBrush.Add(5, Brushes.DimGray);
            _numBrush.Add(6, Brushes.Green);
            _numBrush.Add(7, Brushes.Silver);
            _numBrush.Add(8, Brushes.SkyBlue);
            _numBrush.Add(9, Brushes.Red);
            _numBrush.Add(10, Brushes.Purple);
        }

        //计算整体框架行高,列宽
        private void initRowAndWidth()
        {
            double baseRowHeight = (_map.Height - 20) * 1.0 / (rowHeightRatio[0] + 20 * rowHeightRatio[1]);
            for (int i = 0; i < _rowHeight.Length; i++)
            {
                _rowHeight[i] = (int)(baseRowHeight * rowHeightRatio[i]);
            }

            double baseColWidth = (_map.Width - 9) * 1.0 / colHeightRatio.Sum();
            for (int i = 0; i < _colWidth.Length; i++)
            {
                _colWidth[i] = (int)(baseColWidth * colHeightRatio[i]);
            }
        }

        //Main函数
        public void Draw(JArray jData)
        {
            _gr = Graphics.FromImage(_map);
            drawFrame();
            drawTitle();
            drawData(jData);
            _gr.Dispose();
        }

        //绘制主体框架
        private void drawFrame()
        {
            //横线
            _gr.DrawLine(_framePen, new Point(0, _rowHeight[0]), new Point(_map.Width, _rowHeight[0]));
            for (int i = 1; i <= 19; i++)
            {
                int tempHeight = _rowHeight[0] + i + i * _rowHeight[1];
                _gr.DrawLine(_framePen, new Point(0, tempHeight), new Point(_map.Width, tempHeight));
            }


            //竖线
            for (int i = 0; i < 9; i++)
            {
                int tempWidth = _colWidth.Take<int>(i + 1).Sum();
                _gr.DrawLine(_framePen, new Point(tempWidth + i, _rowHeight[0]), new Point(tempWidth + i, _map.Height));
            }


        }

        //绘制标题
        private void drawTitle()
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
            format.Alignment = StringAlignment.Center;      // 水平居中
            Font font = new Font("宋体", 12, FontStyle.Regular);
            Brush brush = Brushes.Black;
            _gr.FillRectangle(Brushes.WhiteSmoke, 0, 0, _map.Width, _rowHeight[0]);
            PositionContent[] title = new PositionContent[4];
            title[0] = new PositionContent("时间", new Rectangle(_colWidth.Take<int>(0).Sum() + 0, 0, _colWidth[0], _rowHeight[0]));
            title[1] = new PositionContent("开奖号码", new Rectangle(_colWidth.Take<int>(1).Sum() + 1, 0, _colWidth[1], _rowHeight[0]));
            title[2] = new PositionContent("冠亚军和", new Rectangle(_colWidth.Take<int>(2).Sum() + 2, 0, _colWidth[2] + _colWidth[3] + _colWidth[4], _rowHeight[0]));
            title[3] = new PositionContent("1-5龍虎", new Rectangle(_colWidth.Take<int>(5).Sum() + 5, 0, _colWidth[5] + _colWidth[6] + _colWidth[7] + _colWidth[8] + _colWidth[9], _rowHeight[0]));

            for (int i = 0; i < title.Length; i++)
            {
                _gr.DrawString(title[i].Content, font, brush, title[i].Rect, format);
            }

        }

        //绘制数据栏
        private void drawData(JArray jData)
        {
            if (jData == null)
            {
                return;
            }
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
            format.Alignment = StringAlignment.Center;      // 水平居中
            Font font = new Font("宋体", 12, FontStyle.Regular);
            Brush brush = Brushes.Black;

            int RowCount = jData.Count > 20 ? 20:jData.Count;
            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
                JObject jRow = (JObject)jData[rowIndex];
                PositionContent[] cols = new PositionContent[10];
                for (int i = 0; i < 10; i++)
                {
                    string content = string.Empty;
                    if (jRow["c" + (i + 1).ToString()] == null)
                    {
                        content = "";
                    }
                    else
                    {
                        content = jRow["c" + (i + 1).ToString()].ToString();
                    }
                    int tempHeight = _rowHeight[0] + rowIndex + 1 + rowIndex * _rowHeight[1];
                    Rectangle Rect = new Rectangle(_colWidth.Take<int>(i).Sum() + i, tempHeight, _colWidth[i], _rowHeight[1]);
                    PositionContent temp = new PositionContent(content, Rect);
                    cols[i] = temp;
                }
                for (int i = 0; i < cols.Length; i++)
                {
                    if (i != 1)
                        _gr.DrawString(cols[i].Content, font, brush, cols[i].Rect, format);
                    else
                    {
                        Rectangle codeRect = cols[i].Rect;
                        int padding = 20;
                        Bitmap numImage = drawNumber(cols[i].Content, cols[i].Rect.Width - padding * 2);
                        _gr.DrawImage(numImage, codeRect.Left + padding, codeRect.Top, cols[i].Rect.Width - padding * 2, _rowHeight[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制开奖号码区
        /// </summary>
        /// <param name="?"></param>
        private Bitmap drawNumber(string numText, int width)
        {
            StringFormat format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;  // 更正： 垂直居中
            format.Alignment = StringAlignment.Center;      // 水平居中
            Font font = new Font("宋体", 12, FontStyle.Bold);
            Brush brush = Brushes.Black;

            int quWidth = (int)(_rowHeight[1] * 0.8);
            int sepWidth = (int)((width - quWidth * 10) / 9);
            Bitmap numMap = new Bitmap(width, _rowHeight[1]);
            string[] numStrList = numText.Split(' ');
            Graphics _grNum = Graphics.FromImage(numMap);
            //_grNum.SmoothingMode= System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //_grNum.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //_grNum.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            quWidth = quWidth >= (int)(_rowHeight[1] * 0.9) ? (int)(_rowHeight[1] * 0.9) : quWidth;
            for (int i = 0; i < numStrList.Length; i++)
            {
                string numStr = numStrList[i];
                Rectangle curRect = new Rectangle(i * (sepWidth + quWidth), (_rowHeight[1] - quWidth) / 2, quWidth, quWidth);
                _grNum.FillRectangle(_numBrush[Int32.Parse(numStr)], curRect);
                _grNum.DrawString(numStr, font, brush, curRect, format);
            }
            _grNum.Dispose();
            return numMap;
        }

        //保存文件
        public void Save()
        {
            try
            {
                _map.Save(_savePath, ImageFormat.Png);
            }
            catch (Exception ex)
            {
                throw new Exception("文件路径异常,生成图片失败");
            }
            GC.Collect();
        }
    }

    //格式化工具
    public static class DataFormat
    {
        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static JArray FormatString(string jsonData)
        {
            JArray res = null;
            JObject jData = new JObject();
            try 
	        {
                jData = JObject.Parse(jsonData);
	        }
	        catch (Exception)
	        {
                throw new Exception("JSON字符串反序列化失败") ;
	        };
            try 
	        {	        
		        res = FormatJObject(jData);
	        }
	        catch (Exception ex)
	        {
		        throw new Exception("数据解析失败");
	        }
            return res;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        public static JArray FormatJObject(JObject jData)
        {
            JArray jres = null;
            try
            {
                if (jData["result"].ToString().Trim() == "SUCCESS")
                {
                    jres = new JArray();
                    JArray jDataArray = (JArray)jData["data"];
                    for (int i = 0; i < jDataArray.Count; i++)
                    {
                        JObject jtemp = (JObject)jDataArray[i];
                        JObject jresData = new JObject();
                        jresData["c1"] = jtemp["racingNum"].ToString() + " " + jtemp["racingTime"].ToString();
                        jresData["c2"] = String.Join(" ", jtemp["racingResult"]);
                        jresData["c3"] = jtemp["firstAddSecond"];
                        jresData["c4"] = (bool)jtemp["isFirstSecondOdd"] == true ? "单" : "双";
                        jresData["c5"] = (bool)jtemp["isFirstSecondBig"] == true ? "大" : "小";
                        jresData["c6"] = (bool)jtemp["isFirstUp"] == true ? "龍" : "虎";
                        jresData["c7"] = (bool)jtemp["isSecondUp"] == true ? "龍" : "虎";
                        jresData["c8"] = (bool)jtemp["isThirdUp"] == true ? "龍" : "虎";
                        jresData["c9"] = (bool)jtemp["isFourthUp"] == true ? "龍" : "虎";
                        jresData["c10"] = (bool)jtemp["isFifthUp"] == true ? "龍" : "虎";
                        jres.Add(jresData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("数据解析失败");
            }
            return jres;
        }
    }
}
