using System;
using System.Collections.Generic;

namespace GameCore.Core
{
    public class FtpDownloadDestBytes : FtpDownloadDestBase
    {
        public byte[] bytes;
        private List<byte> list = new List<byte>();

        public FtpDownloadDestBytes()
        {
        }

        public override void Close()
        {
            bytes = list.ToArray();
            list.Clear();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(buffer[offset + i]);
            }

            writeCount++;
        }
    }
}