using System;
using System.Threading;

namespace Kernel.Core
{
    public class FtpData
    {
        public bool success;
        public string errorMsg;
        public bool isFinished;
        public Action onFinish;

        public string ftpUrl;
        public string userName;
        public string passward;

        public FtpUploadSourceBase uploadSource;
        public FtpDownloadDestBase downloadDest;

        public long totalLength;
        public long processedLength;

        public Thread thread;

        public string UploadText
        {
            get
            {
                var s = uploadSource as FtpUploadSourceString;
                return s != null ? s.text : string.Empty;
            }
        }

        public byte[] UploadBytes
        {
            get
            {
                var s = uploadSource as FtpUploadSourceBytpes;
                return s?.bytes;
            }
        }

        public string DownloadText
        {
            get
            {
                var s = downloadDest as FtpDownloadDestString;
                return s != null ? s.text : string.Empty;
            }
        }

        public byte[] DownloadBytes
        {
            get
            {
                var s = downloadDest as FtpDownloadDestBytes;
                return s?.bytes;
            }
        }

        public FtpData(string ftpUrl, string userName, string password, FtpUploadSourceBase souce)
        {
            this.ftpUrl = ftpUrl;
            this.userName = userName;
            this.passward = password;
            uploadSource = souce;
        }

        public FtpData(string ftpUrl, string userName, string password, FtpDownloadDestBase dest)
        {
            this.ftpUrl = ftpUrl;
            this.userName = userName;
            this.passward = password;
            downloadDest = dest;
        }

        public FtpData Clone(FtpDownloadDestBase dest)
        {
            return new FtpData(ftpUrl, userName, passward, dest);
        }
    }
}