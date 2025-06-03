using System;

namespace GameCore.Core
{
    public class FtpUploadSourceBase : IDisposable
    {
        public int readCount = 0; //已经读取的数量

        public FtpUploadSourceBase()
        {
        }

        public virtual IDisposable Open()
        {
            return this;
        }

        public virtual void Close()
        {

        }

        public virtual int Read(byte[] buffer, int offset, int count)
        {
            return 0;
        }

        public virtual long GetTotalLength()
        {
            return 0;
        }

        public void ResetReadCount()
        {
            readCount = 0;
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}