namespace GameCore.Unity.Localization
{
    public static class L10nExtension
    {
        public static string L10n(this string str, params object[] args)
        {
            return Localization.L10n.Inst.Get(str, args);
        }
    }
}