using System.IO;
using System;
using System.Text;

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
                Logger.LogYellow(string.Format("exception thrown deleting {0}", fs[index].FullName));
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
                Logger.LogYellow(string.Format("exception thrown deleting {0}", subs[index].FullName));
            }
        }
    }

}
