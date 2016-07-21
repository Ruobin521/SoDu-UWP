using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Util
{
    public class ChineseGBKConverter
    {

        /// <summary>
        /// 从汉字转换到16进制
        /// </summary>
        /// <param name="argStrUtf8"></param>
        /// <param name="argUseSeparator">是否每中文字符间用%分隔</param>
        /// <returns></returns>
        public static string Utf8ToGb2312(string argStrUtf8, bool argUseSeparator = true)
        {
            if ((argStrUtf8.Length % 2) != 0)
            {
                argStrUtf8 += " ";//空格 
            }
            System.Text.Encoding chs = Encoding.GetEncoding("gb2312");//System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = chs.GetBytes(argStrUtf8);
            string str = "%";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if (argUseSeparator && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", "%");
                }
            }
            return str.ToUpper();
        }

        /// <summary>
        /// 传入一个字符串，将字符串中的汉字转换为GBK编码,然后输出
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertChinestStringtoGBKString(string str)
        {
            string tmp = string.Empty;
            try
            {
                char[] uil = str.ToArray();
                for (int j = 0; j < uil.Length; j++)
                {
                    string t = uil[j].ToString();
                    if (CheckStringChineseReg(t))
                    {
                        t = ConvertChineseToGBK(uil[j].ToString());
                    }
                    else if ("(".Equals(t) || ")".Equals(t) || "（".Equals(t) || "）".Equals(t) || "《".Equals(t) || "》".Equals(t) || ">".Equals(t) || "<".Equals(t) || "、".Equals(t))
                    {
                        t = ConvertChineseToGBK(uil[j].ToString());
                    }
                    tmp += t;

                }
            }
            catch (Exception)
            {
                tmp = str;
            }
            return tmp;
        }
        /// <summary>
        /// 判断字符串中字符是否为汉字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool CheckStringChineseReg(string text)
        {
            bool res = false;
            try
            {
                char[] c = text.ToArray();

                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] >= 0x4e00 && c[i] <= 0x9fbb)
                    {
                        res = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                res = true;

            }
            return res;
        }

        /// <summary>
        /// 将字符转换为GBK
        /// </summary>
        /// <param name="m_ChineseString"></param>
        /// <returns></returns>
        public static string ConvertChineseToGBK(string m_ChineseString)
        {
            byte[] gbk = Encoding.GetEncoding("GBK").GetBytes(m_ChineseString);
            string GBKString = "%";
            foreach (byte b in gbk)
            {
                GBKString += string.Format("{0:X2}", b) + "%";
            }
            return GBKString.Remove(GBKString.Length - 1);
        }

        /// <summary>
        /// 将GBK转换为汉字
        /// </summary>
        /// <param name="m_GBKString"></param>
        /// <returns></returns>
        public static string ConvertGBKToChinese(string m_GBKString)
        {
            string[] gbkLists = m_GBKString.Split(new string[] { "%" }, StringSplitOptions.RemoveEmptyEntries);
            byte[] gbkOne = new byte[2];
            string Chinese = string.Empty;
            for (int i = 0; i < gbkLists.Length / 2; i++)
            {
                gbkOne[0] = (byte)Convert.ToByte(gbkLists[2 * i], 16);
                gbkOne[1] = (byte)Convert.ToByte(gbkLists[2 * i + 1], 16);
                // Chinese += Encoding.GetEncoding("GBK").GetString(gbkOne);
            }
            return Chinese;
        }
    }
}
