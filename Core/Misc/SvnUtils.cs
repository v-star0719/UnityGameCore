using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Kernel.Unity
{
    public class SvnUtils
    {
        private static readonly string _GET_REVERSION = "export SVN_HOME=/usr/local/Cellar/subversion/1.14.2/\nexport PATH=${PATH}:${SVN_HOME}bin/\nexport LANG=en_US\nsvn info";
        private static readonly string _REGEXP_REVERSION = "Last Changed Rev: ([0-9]+)";
        private static readonly Regex _REG_REVERSION = new Regex(_REGEXP_REVERSION);

        private static readonly string _SVN_FORMAT = "svn diff -r {{{0:yyyy-MM-dd}}}:HEAD --summarize {1}";
        private static readonly string _HOTRES_PATH = "./Assets/HotRes/";

        private static readonly string _REGEXP_FILECHANGE = "[MAU][ ]+[\\/a-zA-Z0-9\\-_\\.\\:]+(Assets\\/HotRes[\\/a-zA-Z0-9\\-_\\.]+\\.prefab)";
        private static readonly Regex _REG_FILECHANGE = new Regex(_REGEXP_FILECHANGE);

        private static string _reversionPath;
        private static int _reversion = -1;

        /// 获取修改日期更新的文件列表
        public static List<string> GetModifiedAssets(DateTime afterOrOn)
        {
            string cmd = string.Format(_SVN_FORMAT, afterOrOn, Path.GetFullPath(_HOTRES_PATH));
            string[] lines = ShellUtils.ExecuteShellCommand(cmd);
            List<string> result = new List<string>();
            Match m;
            string l;
            for(int index = 0; index < lines.Length; index++)
            {
                m = _REG_FILECHANGE.Match(lines[index]);
                if(m.Success && (0 < m.Groups.Count) && (0 < m.Groups[1].Value.Length))
                {
                    l = m.Groups[1].Value;
                    if(!result.Contains(l))
                    {
                        result.Add(l);
                    }
                }
            }

            return result;
        }

        public static int GetReversion(string path)
        {
            if(0 > _reversion || !string.Equals(_reversionPath, path))
            {
                string[] lines = ShellUtils.ExecuteShellCommand($"{_GET_REVERSION} {path}");
                Match m;
                for(int index = 0; index < lines.Length; index++)
                {
                    m = _REG_REVERSION.Match(lines[index]);
                    if(m.Success && (0 < m.Groups.Count) && (0 < m.Groups[1].Value.Length))
                    {
                        _reversionPath = path;
                        _reversion = Convert.ToInt32(m.Groups[1].Value);
                        Debug.LogFormat($"{_reversionPath} Reversion: {_reversion}");
                    }
                }
            }

            return _reversion;
        }
    }
}
