﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDu.Core.Util
{
    public class LevenshteinDistance
    {
        /// <summary>
        /// 取最小的一位数
        /// </summary>
        /// <param name=”first”></param>
        /// <param name=”second”></param>
        /// <param name=”third”></param>
        /// <returns></returns>
        public static int LowerOfThree(int first, int second, int third)
        {
            int min = Math.Min(first, second);
            return Math.Min(min, third);
        }
        /// <summary>
        /// www.it165.net
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static int Levenshtein_Distance(string str1, string str2)
        {
            int[,] Matrix;
            int n = str1.Length;
            int m = str2.Length;

            int temp = 0;
            char ch1;
            char ch2;
            int i = 0;
            int j = 0;
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {

                return n;
            }
            Matrix = new int[n + 1, m + 1];

            for (i = 0; i <= n; i++)
            {
                //初始化第一列
                Matrix[i, 0] = i;
            }

            for (j = 0; j <= m; j++)
            {
                //初始化第一行
                Matrix[0, j] = j;
            }

            for (i = 1; i <= n; i++)
            {
                ch1 = str1[i - 1];
                for (j = 1; j <= m; j++)
                {
                    ch2 = str2[j - 1];
                    if (ch1.Equals(ch2))
                    {
                        temp = 0;
                    }
                    else
                    {
                        temp = 1;
                    }
                    Matrix[i, j] = LowerOfThree(Matrix[i - 1, j] + 1, Matrix[i, j - 1] + 1, Matrix[i - 1, j - 1] + temp);

                }
            }

            return Matrix[n, m];
        }

        /// <summary>
        /// 计算字符串相似度
        /// </summary>
        /// <param name=”str1″></param>
        /// <param name=”str2″></param>
        /// <returns></returns>
        public static decimal LevenshteinDistancePercent(string str1, string str2)
        {

            str1 = str1.ToLower().Replace(" ", "").Replace("　", "");
            str2 = str2.ToLower().Replace(" ", "").Replace("　", "");

            if (str1.Equals(str2) || str1.Contains(str2) || str2.Contains(str1))
            {
                return 100;
            }

            int val = Levenshtein_Distance(str1, str2);
            return 1 - (decimal)val / Math.Max(str1.Length, str2.Length);
        }
    }
}
