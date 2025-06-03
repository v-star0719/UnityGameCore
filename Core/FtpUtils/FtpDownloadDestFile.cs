using System;
using System.IO;

namespace GameCore.Core
{
    public class FtpDownloadDestFile : FtpDownloadDestBase
    {
        public string filePath;
        private FileStream fs;

        public FtpDownloadDestFile(string filePath)
        {
            this.filePath = filePath;
        }

        public override IDisposable Open()
        {
            fs = File.Open(filePath, FileMode.Create);
            return this;
        }

        public override void Close()
        {
            if (fs != null)
            {
                fs.Close();
                fs = null;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            fs.Write(buffer, offset, count);
        }
    }
}