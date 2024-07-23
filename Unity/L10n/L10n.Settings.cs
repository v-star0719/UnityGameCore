using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public class L10nLanguageInfo
    {
        public SystemLanguage language;
        public bool isSupported;
        public SystemLanguage replaceLanguage;//改语言不受支持时，用哪个语言替代
        public string nativeName;
        public string sortName;

        public L10nLanguageInfo(SystemLanguage lan, string nativeName, bool isSupported, SystemLanguage replaceLan)
        {
            language = lan;
            sortName = lan.ToString();
            this.isSupported = isSupported;
            replaceLanguage = replaceLan;
            this.nativeName = nativeName;
        }
    }

    public partial class L10n
    {
        public Dictionary<SystemLanguage, L10nLanguageInfo> languages = new Dictionary<SystemLanguage, L10nLanguageInfo>();

        public void InitLanguageInfo()
        {
            AddLanguageInfo(SystemLanguage.Afrikaans, "", false, SystemLanguage.English);//南非的公用荷兰语
            AddLanguageInfo(SystemLanguage.Arabic, "العربية", false, SystemLanguage.English);//阿拉伯语
            AddLanguageInfo(SystemLanguage.Basque, "", false, SystemLanguage.English);//巴斯克语
            AddLanguageInfo(SystemLanguage.Belarusian, "", false, SystemLanguage.English);//白俄罗斯语
            AddLanguageInfo(SystemLanguage.Bulgarian, "", false, SystemLanguage.English);//保加利亚语
            AddLanguageInfo(SystemLanguage.Catalan, "", false, SystemLanguage.English);//加泰罗尼亚语
            AddLanguageInfo(SystemLanguage.Chinese, "", false, SystemLanguage.ChineseSimplified);//中文
            AddLanguageInfo(SystemLanguage.ChineseSimplified, "简体中文", true, SystemLanguage.English);//简体中文
            AddLanguageInfo(SystemLanguage.ChineseTraditional, "繁體中文", true, SystemLanguage.English);//繁体中文
            AddLanguageInfo(SystemLanguage.Czech, "", false, SystemLanguage.English);//捷克语
            AddLanguageInfo(SystemLanguage.Danish, "", false, SystemLanguage.English);//丹麦语
            AddLanguageInfo(SystemLanguage.Dutch, "Nederlands", false, SystemLanguage.English);//荷兰语
            AddLanguageInfo(SystemLanguage.English, "English", true, SystemLanguage.English);//英语
            AddLanguageInfo(SystemLanguage.Estonian, "", false, SystemLanguage.English);//爱沙尼亚语
            AddLanguageInfo(SystemLanguage.Faroese, "", false, SystemLanguage.English);//法罗语
            AddLanguageInfo(SystemLanguage.Finnish, "Suomi", false, SystemLanguage.English);//芬兰语
            AddLanguageInfo(SystemLanguage.French, "Français", false, SystemLanguage.English);//法语
            AddLanguageInfo(SystemLanguage.German, "Deutsch", false, SystemLanguage.English);//德语
            AddLanguageInfo(SystemLanguage.Greek, "", false, SystemLanguage.English);//希腊语
            AddLanguageInfo(SystemLanguage.Hebrew, "עברית", false, SystemLanguage.English);//希伯来语
            AddLanguageInfo(SystemLanguage.Hungarian, "", false, SystemLanguage.English);//匈牙利语
            AddLanguageInfo(SystemLanguage.Hindi, "", false, SystemLanguage.English);//北印度语 //हिन्दी游戏里的字体不支持
            AddLanguageInfo(SystemLanguage.Icelandic, "", false, SystemLanguage.English);//冰岛语
            AddLanguageInfo(SystemLanguage.Indonesian, "Bahasa Indonesia", false, SystemLanguage.English);//印尼语
            AddLanguageInfo(SystemLanguage.Italian, "Italiano", false, SystemLanguage.English);//意大利语
            AddLanguageInfo(SystemLanguage.Japanese, "日本語", false, SystemLanguage.English);//日语
            AddLanguageInfo(SystemLanguage.Korean, "한국어", false, SystemLanguage.English);//韩语
            AddLanguageInfo(SystemLanguage.Latvian, "", false, SystemLanguage.English);//拉脱维亚语
            AddLanguageInfo(SystemLanguage.Lithuanian, "", false, SystemLanguage.English);//立陶宛语
            AddLanguageInfo(SystemLanguage.Norwegian, "", false, SystemLanguage.English);//挪威语
            AddLanguageInfo(SystemLanguage.Polish, "Polski", false, SystemLanguage.English);//波兰语
            AddLanguageInfo(SystemLanguage.Portuguese, "Português", false, SystemLanguage.English);//葡萄牙语
            AddLanguageInfo(SystemLanguage.Romanian, "", false, SystemLanguage.English);//罗马尼亚语
            AddLanguageInfo(SystemLanguage.Russian, "Pусский", false, SystemLanguage.English);//俄语
            AddLanguageInfo(SystemLanguage.SerboCroatian, "", false, SystemLanguage.English);//塞尔维亚克罗地亚
            AddLanguageInfo(SystemLanguage.Slovak, "", false, SystemLanguage.English);//斯洛伐克语
            AddLanguageInfo(SystemLanguage.Slovenian, "", false, SystemLanguage.English);//斯洛维尼亚语
            AddLanguageInfo(SystemLanguage.Spanish, "Español", false, SystemLanguage.English);//西班牙语
            AddLanguageInfo(SystemLanguage.Swedish, "Svenska", false, SystemLanguage.English);//瑞典语
            AddLanguageInfo(SystemLanguage.Thai, "ภาษาไทย", false, SystemLanguage.English);//泰语
            AddLanguageInfo(SystemLanguage.Turkish, "Türkçe", false, SystemLanguage.English);//土耳其语
            AddLanguageInfo(SystemLanguage.Ukrainian, "", false, SystemLanguage.English);//乌克兰语
            AddLanguageInfo(SystemLanguage.Vietnamese, "tiếng việt", false, SystemLanguage.English);//越南
            //"Bahasa Malaysia", 马来西亚
        }

        private void AddLanguageInfo(SystemLanguage lan, string nativeName, bool isSupported, SystemLanguage replaceLanguage)
        {
            languages[lan] = new L10nLanguageInfo(lan, nativeName, isSupported, replaceLanguage);
        }

        //返回默认显示的语言
        public SystemLanguage GetDefaultLanguage(SystemLanguage lan)
        {
            if (languages.TryGetValue(lan, out var l))
            {
                return l.isSupported ? l.language : l.replaceLanguage;
            }
            return SystemLanguage.English;
        }
    }
}
