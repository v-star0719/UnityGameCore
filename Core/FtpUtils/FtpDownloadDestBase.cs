using System;

namespace GameCore.Core
{
    public class FtpDownloadDestBase : IDisposable
    {
        public int writeCount = 0; //已经写入的数量

        public FtpDownloadDestBase()
        {
        }

        public virtual IDisposable Open()
        {
            return this;
        }

        public virtual void Close()
        {

        }

        public virtual void Write(byte[] buffer, int offset, int count)
        {
        }

        public void ResetWriteCount()
        {
            writeCount = 0;
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}