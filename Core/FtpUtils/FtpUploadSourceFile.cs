using System;
using System.IO;

namespace GameCore.Core
{
    public class FtpUploadSourceFile : FtpUploadSourceBase
    {
        public string filePath;
        private FileStream fs;

        public FtpUploadSourceFile(string filePath)
        {
            this.filePath = filePath;
        }

        public override IDisposable Open()
        {
            try
            {
                fs = File.Open(filePath, FileMode.Open);
            }
            catch (Exception e)
            {
                LoggerX.Error(e.ToString());
            }

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

        public override long GetTotalLength()
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = fs.Read(buffer, offset, count);
            return count;
        }
    }
}