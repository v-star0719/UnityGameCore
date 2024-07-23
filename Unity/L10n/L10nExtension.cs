using System.Collections;
using UnityEngine;

namespace Localize
{
    public static class L10nExtension
    {
        public static string L10n(this string str, params object[] args)
        {
            return Misc.L10n.Inst.Get(str, args);
        }
    }
}