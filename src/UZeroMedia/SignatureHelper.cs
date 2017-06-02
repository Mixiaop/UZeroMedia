using System;

namespace UZeroMedia
{
    /// <summary>
    /// 优志愿密码簿V1版
    /// 24小时制，例：2015-12-24 23:55:12
    /// 密码对照表：0 1 2 3 4 5 6 7 8 9 - : 空格 随机符
    ///             Z 0 T t F f S s E N L D B    C
    /// 组合规则：随机符为随机位次穿插
    /// 样式结果：TZOfLOCTLTFBTtDffDOT
    /// 20位
    /// </summary>
    public class SignatureHelper
    {
        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="sign">签名Key</param>
        /// <param name="now">当前系统时间</param>
        /// <param name="expiresMinutes">过期时间（分钟）</param>
        /// <returns></returns>
        public static bool Validation(string sign, DateTime now, int expiresMinutes)
        {
            var result = false;
            sign = sign.Replace("C", "");
            if (sign.Length == 19)
            {
                var signTimeString = sign.Replace("Z", "0")
                                         .Replace("O", "1")
                                         .Replace("T", "2")
                                         .Replace("t", "3")
                                         .Replace("F", "4")
                                         .Replace("f", "5")
                                         .Replace("S", "6")
                                         .Replace("s", "7")
                                         .Replace("E", "8")
                                         .Replace("N", "9")
                                         .Replace("L", "-")
                                         .Replace("D", ":")
                                         .Replace("B", " ");
                try
                {
                    DateTime signTime = signTimeString.ToDateTime();
                    TimeSpan span = now - signTime;

                    //是否过期
                    if (span.TotalMinutes <= expiresMinutes)
                    {
                        result = true;
                    }
                }
                catch
                {

                }
            }

            return result;
        }

        /// <summary>
        /// 获取当前时间加密的签名
        /// </summary>
        /// <returns></returns>
        public static string Create() {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var signTimeString = now.Replace("0", "Z")
                                         .Replace("1", "O")
                                         .Replace("2", "T")
                                         .Replace("3", "t")
                                         .Replace("4", "F")
                                         .Replace("5", "f")
                                         .Replace("6", "S")
                                         .Replace("7", "s")
                                         .Replace("8", "E")
                                         .Replace("9", "N")
                                         .Replace("-", "L")
                                         .Replace(":", "D")
                                         .Replace(" ", "B");

            return signTimeString;
        }
    }
}
