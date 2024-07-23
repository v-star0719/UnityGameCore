using System;
using System.Collections.Generic;
using System.Text;
using Kernel.Core;
using Kernel.Lang.Extension;
using UnityEngine;

namespace Misc
{
    public interface IL10nMsgReceiver
    {
        int Id { get; set;}
        void OnL10nChange();
    }

    public partial class L10n : Singleton<L10n>
    {
        private static string Separator = " = ";
        private Dictionary<string, string> dict = new Dictionary<string, string>(1024);
        private List<IL10nMsgReceiver> receivers = new List<IL10nMsgReceiver>();
        private int id;

        public L10n()
        {
            InitLanguageInfo();
        }

        public void Load(SystemLanguage language)
        {
            dict.Clear();
            var file = GetLanguageFileName(language);
            var textAsset = ResManager.Instance.GetTextAsset(file);
            if (textAsset == null)
            {
                Debug.LogError($"l10n file is not exist {file}");
                return;
            }

            var text = textAsset.text;
            var lineBegin = 0;
            var lineIndex = 0;
            while (lineBegin < text.Length)
            {
                lineIndex++;
                var lineEnd = text.IndexOf('\n', lineBegin);
                if (lineEnd < 0)
                {
                    lineEnd = text.Length;
                }

                if (!AnalysisLine(text, lineBegin, lineEnd))
                {
                    Debug.LogError("L10n AnalysisLine failed, at line : " + lineIndex);
                }
                lineBegin = lineEnd + 1;
            }

            //foreach (var kv in dict)
            //{
            //    Debug.Log($"{kv.Key} = {kv.Value}");
            //}

            foreach (var receiver in receivers)
            {
                receiver.OnL10nChange();
            }
        }

        public void AddReceiver(IL10nMsgReceiver r)
        {
            if (r.Id > 0)
            {
                Debug.LogError("IL10nMsgReceiver is registered: " + r.Id);
                return;
            }
            r.Id = ++id;
            receivers.Add(r);
        }

        public void RemoveReceiver(IL10nMsgReceiver r)
        {
            if (r.Id <= 0)
            {
                Debug.LogError("IL10nMsgReceiver is not registered: " + r.Id);
                return;
            }

            r.Id = 0;
            receivers.FastRemove(receivers.IndexOf(r));
        }

        public string Get(string key, params object[] args)
        {
            if (dict.TryGetValue(key, out var value))
            {
                try
                {
                    return string.Format(value, args);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return e.ToString();
                }
            }

            StringBuilder sb = new StringBuilder("Missing: ");
            sb.Append(key);
            foreach (var t in args)
            {
                sb.Append($" [{t}]");
            }
            return sb.ToString();
        }

        private bool AnalysisLine(string text, int lineBegin, int lineEnd)
        {
            var keyEnd = text.IndexOf(Separator, lineBegin);
            if (keyEnd >= lineEnd || keyEnd < 0)
            {
                return false;
            }

            var key = text.Substring(lineBegin, keyEnd - lineBegin);
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, text.Substring(keyEnd + Separator.Length, lineEnd - keyEnd - Separator.Length));
            }
            else
            {
                Debug.LogError($"Duplicated key [{key}]");
            }

            return true;
        }

        private string GetLanguageFileName(SystemLanguage lan)
        {
            return $"l10n_{lan}";
        }
    }
}
