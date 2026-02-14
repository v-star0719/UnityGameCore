using System;
using System.Collections.Generic;
using System.Text;

namespace GameCore.Core
{
    public static class StringUtils
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

        public static void SortByNumberInString(IList<string> list)
        {
            var tmpList = new List<StringSortInfo>(list.Count);
            foreach (var l in list)
            {
                tmpList.Add(new StringSortInfo(l));
            }
            tmpList.Sort((a, b) => a.CompareTo(b));
            list.Clear();
            foreach (var info in tmpList)
            {
                list.Add(info.Str);
            }
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

        //两个字符串之间的字符串。比如123|13333|323，("|", "|")返回13333
        public static string Substring(this string s, string start, string end)
        {
            var idx1 = 0;
            if(!string.IsNullOrEmpty(start))
            {
                idx1 = s.IndexOf(start);
                if(idx1 < 0)
                {
                    return string.Empty;
                }
                idx1 = idx1 + start.Length;
            }

            var idx2 = s.Length - 1;
            if(!string.IsNullOrEmpty(end))
            {
                idx2 = s.IndexOf(end, idx1 + 1);
                if(idx2 < 0)
                {
                    return string.Empty;
                }
            }
            return s.Substring(idx1, idx2 - idx1);
        }

        /// <summary>
        /// 将字符串首字母大写，其余字符保持原样
        /// </summary>
        /// <param name="input">输入字符串（可null/空/空白）</param>
        /// <returns>首字母大写的字符串，空值返回空字符串</returns>
        public static string FirstLetterToUpper(string input)
        {
            // 处理null、空字符串、全空白字符串
            if(string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            // 单个字符直接转大写
            if(input.Length == 1)
            {
                return input.ToUpper();
            }

            // 首字母大写 + 剩余字符拼接
            return char.ToUpper(input[0]) + input.Substring(1);
        }
        private class StringSortInfo : IComparable
        {
            public string Str { get; private set; }
            private uint[] numbers;
            private int numberIndex;

            public StringSortInfo(string s)
            {
                Str = s;
                numbers = new uint[s.Length];
                uint number = 0;
                bool findNumber = false;
                for(int i = 0; i < s.Length; i++)
                {
                    var c = s[i];
                    if(c >= '0' && c <= '9')
                    {
                        findNumber = true;
                        number = number * 10 + c - '0';
                    }
                    else
                    {
                        if (findNumber)
                        {
                            findNumber = false;
                            numbers[numberIndex++] = number;
                            number = 0;
                        }
                        numbers[numberIndex++] = c;
                    }
                }
            }

            public int CompareTo(object obj)
            {
                if (obj is StringSortInfo s)
                {
                    for (var i = 0; i < numbers.Length && i < s.numbers.Length; i++)
                    {
                        if (numbers[i] != s.numbers[i])
                        {
                            return numbers[i].CompareTo(s.numbers[i]);
                        }
                    }
                    if (numberIndex == s.numberIndex)
                    {
                        return 0;
                    }
                    if (numberIndex < s.numberIndex)
                    {
                        return 1;
                    }
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}
