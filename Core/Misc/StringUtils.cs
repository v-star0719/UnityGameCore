using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Core
{
    class StringUtils
    {
        public static bool IsTelePhone(string str_handset)
        {
            return !string.IsNullOrEmpty(str_handset) && str_handset.Length == 11 && System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^[1]+\d{10}");
        }

        public static bool IsNumber(string str_number)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_number, @"^[0-9]*$");
        }

        public static bool IsChinese(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"[\u4e00-\u9fa5]+$");
        }

        public static string TruncateString(string orgText, int maxCharactorCount, string suffix)
        {
            int n = 0;
            const char c = (char)255;
            for (int i = 0; i < orgText.Length; i++)
            {
                if (orgText[i] < c)
                    n++;
                else
                    n += 2;
                if (n > maxCharactorCount)
                    return orgText.Substring(0, i) + suffix;//取子字符串
            }
            return orgText;//原来的字数没超，原样返回
        }

        /// <summary>
        /// Urlencode 如果是中文需要两次encode,服务器再主动调用一次decode,
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            return Uri.EscapeDataString(str);
        }

        public static string ToStr(object o)
        {
            return ToStr(o, new StringBuilder());
        }

        public static string ToStr(object o, StringBuilder sb)
        {
            foreach (var p in o.GetType().GetFields())
            {

                sb.Append($"{p.Name} = {p.GetValue(o)},");
            }

            foreach (var p in o.GetType().GetProperties())
            {
                sb.Append($"{p.Name} = {p.GetValue(o)},");
            }
            return sb.ToString();
        }

        public static string ArrayToString<T>(T[] arr)
        {
            return "[" + string.Join(",", arr) + "]";
        }


        public static string CollectionToString<T>(IEnumerable<T> arr)
        {
            return "[" + string.Join(",", arr) + "]";
        }


        public static string CollectionToString<TK, TV>(IDictionary<TK, TV> dic)
        {
            var sb = new StringBuilder('{');
            foreach (var e in dic)
            {
                sb.Append(e.Key).Append(':');
                sb.Append(e.Value).Append(',');
            }
            sb.Append('}');
            return sb.ToString();
        }

        //在Windows上运行Unity3D，WWW用file协议加载文件时，路径部分必需用"\"，否则也可能成功也可能不成功
        //eg: file://e:/a.txt  -> file://e:\a.txt
        public static string ConvertNativeUrlToWindowsPlatform(string url)
        {
#if UNITY_STANDALONE_WIN
            if (url.IndexOf("file://") > -1)
            {
                url = url.Replace(@"/", @"\");
                url = url.Replace(@"file:\\", @"file://");
            }
#endif
            return url;
        }
    }
}
