using System.IO;
using GameCore.Core.Logger;

namespace GameCore.Core.Misc
{
    public class DirUtils
    {
        public static void ClearDir(string p)
        {
            DirectoryInfo dir = new DirectoryInfo(p);
            FileInfo[] fs = dir.GetFiles("*", SearchOption.TopDirectoryOnly);
            for(int index = 0; index < fs.Length; index++)
            {
                try
                {
                    File.Delete(fs[index].FullName);
                }
                catch
                {
                    LoggerX.Warn($"exception thrown deleting {fs[index].FullName}");
                }
            }

            DirectoryInfo[] subs = dir.GetDirectories("*", SearchOption.TopDirectoryOnly);
            for(int index = 0; index < subs.Length; index++)
            {
                try
                {
                    Directory.Delete(subs[index].FullName, true);
                }
                catch
                {
                    LoggerX.Warn($"exception thrown deleting {subs[index].FullName}");
                }
            }
        }

    }
}

