using System;
using System.Collections.Generic;

namespace Kernel.Core
{
    public class FtpUploadSourceBytpes : FtpUploadSourceBase
    {
        public byte[] bytes;

        public FtpUploadSourceBytpes()
        {
        }

        public FtpUploadSourceBytpes(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public override long GetTotalLength()
        {
            return bytes.Length;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var left = bytes.Length - readCount;
            if (count > left)
            {
                count = left;
            }

            for (int i = 0; i < count; i++)
            {
                buffer[offset] = bytes[readCount];
                offset++;
                readCount++;
            }

            return count;
        }
    }
}