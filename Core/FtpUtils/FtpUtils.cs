using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Kernel.Core
{
    public class FtpUtils
    {
        private static int buffSize = 30720;
        private const string PRE_FIX = "ftp://";

        public static void FtpThread(object arg)
        {
            var data = arg as FtpData;
            LoggerX.Info(System.DateTime.Now + " FtpThread start");
            if (data.uploadSource != null)
            {
                FtpUpload(data);
            }
            else
            {
                FtpDownload(data);
            }

            LoggerX.Info(System.DateTime.Now + " FtpThread exist");
            Thread.Sleep(1000);
        }

        public static FtpData FtpUploadStringAsync(string ftpUrl, string userName, string password, string text,
            Action onFinish)
        {
            var source = new FtpUploadSourceString(text);
            var data = new FtpData(ftpUrl, userName, password, source);
            data.onFinish = onFinish;
            FtpUploadAsync(data);
            return data;
        }

        public static FtpData FtpUploadFileAsync(string ftpUrl, string userName, string password, string filePath,
            Action onFinish)
        {
            var source = new FtpUploadSourceFile(filePath);
            var data = new FtpData(ftpUrl, userName, password, source);
            data.onFinish = onFinish;
            FtpUploadAsync(data);
            return data;
        }

        public static bool FtpUploadString(string ftpUrl, string userName, string password, string text)
        {
            var source = new FtpUploadSourceString(text);
            var data = new FtpData(ftpUrl, userName, password, source);
            return FtpUpload(data);
        }

        public static FtpData FtpDownloadStringAsync(string ftpUrl, string userName, string password, Action onFinish)
        {
            var dest = new FtpDownloadDestString();
            var data = new FtpData(ftpUrl, userName, password, dest);
            data.onFinish = onFinish;
            FtpDownloadAsync(data);
            return data;
        }

        public static FtpData FtpDownloadFileAsync(string ftpUrl, string userName, string password, string filePath,
            Action onFinish)
        {
            var dest = new FtpDownloadDestFile(filePath);
            var data = new FtpData(ftpUrl, userName, password, dest);
            data.onFinish = onFinish;
            FtpDownloadAsync(data);
            return data;
        }

        public static string FtpDownloadString(string ftpUrl, string userName, string password)
        {
            var dest = new FtpDownloadDestString();
            var data = new FtpData(ftpUrl, userName, password, dest);
            if (FtpDownload(data))
            {
                return dest.text;
            }

            return null;
        }

        public static bool FtpMakeDir(string ftpUrl, string userName, string password)
        {
            var dest = new FtpDownloadDestBase();
            var data = new FtpData(ftpUrl, userName, password, dest);
            if (FtpIsDirExist(data))
            {
                return true;
            }

            data.ftpUrl = GetDirPath(data.ftpUrl);
            LoggerX.Info("FtpMakeDir: " + data.ftpUrl);
            if (FtpMakeDir(data))
            {
                return true;
            }

            return false;
        }

        public static long FtpGetFileSize(string ftpUrl, string userName, string password)
        {
            var dest = new FtpDownloadDestString();
            var data = new FtpData(ftpUrl, userName, password, dest);
            var l = FtpGetFileSize(data);
            return l;
        }


        //url: ftp://1.2.3/a/b/c/a/ or ftp://1.2.3/a/b/c/a
        //返回：ftp://1.2.3/a/b/c/
        public static string GetParentDirPath(string url)
        {
            var first = url.IndexOf("/", PRE_FIX.Length);
            var index = url.LastIndexOf("/");
            if (index == first)
            {
                return url;
            }

            if (index == url.Length - 1)
            {
                index = url.LastIndexOf("/", index - 1);
            }

            return url.Substring(0, index);
        }

        public static string GetDirName(string url)
        {
            var first = url.IndexOf("/", PRE_FIX.Length);
            var index = url.LastIndexOf("/");
            if (index == first)
            {
                return "";
            }

            var index2 = url.LastIndexOf("/", index - 1);

            return url.Substring(index2 + 1, index - 1 - index2);
        }

        //如果就是目录，原封不动返回
        public static string GetDirPath(string url)
        {
            var first = url.IndexOf("/", PRE_FIX.Length);
            var index = url.LastIndexOf("/");
            if (index == first)
            {
                return url;
            }

            return url.Substring(0, index);
        }

        public static bool FtpStopThread(FtpData data)
        {
            if (data.thread != null && data.thread.IsAlive)
            {
                data.thread.Abort();
                data.thread = null;
                return true;
            }

            return false;
        }

        private static void FtpUploadAsync(FtpData data)
        {
            var thread = new Thread(new ParameterizedThreadStart(FtpThread));
            thread.IsBackground = true;
            thread.Start(data);
            data.thread = thread;
        }

        private static void FtpDownloadAsync(FtpData data)
        {
            var thread = new Thread(new ParameterizedThreadStart(FtpThread));
            thread.IsBackground = true;
            thread.Start(data);
            data.thread = thread;
        }

        private static bool FtpUpload(FtpData data)
        {
            LoggerX.Info("FtpUpload: " + data.ftpUrl);
            var reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(data.ftpUrl));
            reqFtp.UseBinary = true; //代表可以发送图片
            reqFtp.Credentials = new NetworkCredential(data.userName, data.passward);
            reqFtp.KeepAlive = true;
            reqFtp.Method = WebRequestMethods.Ftp.UploadFile; //表示将文件上载到 FTP 服务器的 FTP STOR 协议方法
            reqFtp.Timeout = 15000;
            reqFtp.ReadWriteTimeout = 15000;

            var buffer = new byte[buffSize];
            try
            {
                var source = data.uploadSource;
                data.totalLength = source.GetTotalLength();
                reqFtp.ContentLength = data.totalLength; //本地上传文件的长度
                data.processedLength = 0;
                using (source.Open())
                {
                    using (var strm = reqFtp.GetRequestStream())
                    {
                        var len = source.Read(buffer, 0, buffSize);

                        while (len > 0)
                        {
                            strm.Write(buffer, 0, len);
                            data.processedLength += len;
                            LoggerX.Info("upload progress: " + data.processedLength + "/" + data.totalLength);

                            len = source.Read(buffer, 0, buffSize);
                        }

                        strm.Flush();
                    }
                }

                data.success = true;
            }
            catch (Exception ex)
            {
                var msg = "FTP 上传错误：" + ex.Message;
                LoggerX.Error(msg);
                data.success = false;
                data.errorMsg = msg;
            }

            //校验一下
            if (data.success && data.totalLength != FtpGetFileSize(data))
            {
                data.success = false;
                data.errorMsg = "上传失败，文件大小不匹配";
            }

            data.isFinished = true;
            data.onFinish?.Invoke();
            return data.success;
        }

        private static bool FtpDownload(FtpData data)
        {
            LoggerX.Info("FtpDownLoad: " + data.ftpUrl);
            Uri uri = new Uri(data.ftpUrl);
            try
            {
                var dest = data.downloadDest;
                var request = (FtpWebRequest)WebRequest.Create(uri);
                request.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(data.userName, data.passward);
                request.Timeout = 15000;
                request.ReadWriteTimeout = 15000;
                request.UseBinary = true;

                using (var response = (FtpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (dest.Open())
                        {
                            byte[] buff = new byte[buffSize];
                            int readCnt = stream.Read(buff, 0, buff.Length);
                            data.processedLength = 0;
                            data.totalLength = response.ContentLength;
                            while (readCnt > 0)
                            {
                                data.processedLength += readCnt;
                                LoggerX.Info("download progress: " + data.processedLength + "/" + data.totalLength);

                                dest.Write(buff, 0, readCnt);
                                readCnt = stream.Read(buff, 0, buff.Length);
                            }
                        }
                    }
                }

                data.success = true;
            }
            catch (Exception e)
            {
                var msg = "FTP 下载错误：" + e.Message;
                LoggerX.Error(msg);
                data.success = false;
                data.errorMsg = msg;
            }

            //校验一下
            if (data.success && data.processedLength != FtpGetFileSize(data))
            {
                data.success = false;
                data.errorMsg = "下载失败，文件大小不匹配";
            }

            data.isFinished = true;
            data.onFinish?.Invoke();
            return data.success;
        }

        private static bool FtpMakeDir(FtpData data)
        {
            try
            {
                FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(new Uri(data.ftpUrl));
                reqFtp.UseBinary = true;
                reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFtp.Credentials = new NetworkCredential(data.userName, data.passward);
                reqFtp.KeepAlive = true;
                reqFtp.Timeout = 15000;

                using ((FtpWebResponse)reqFtp.GetResponse())
                {
                }

                data.success = true;
            }
            catch (Exception ex)
            {
                data.errorMsg = "创建目录失败：" + ex.Message;
                data.success = false;
                LoggerX.Error(data.errorMsg);
            }

            data.isFinished = true;
            return data.success;
        }

        private static string FtpGetFileList(FtpData data)
        {
            FtpDownloadDestString dest = null;
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(data.ftpUrl);
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(data.userName, data.passward);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Timeout = 15000;
                //todo 判断是文件还是文件夹

                using (var response = reqFTP.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        dest = (FtpDownloadDestString)data.downloadDest;
                        dest.text = reader.ReadToEnd();
                    }
                }

                data.success = true;
                LoggerX.Info("GetFileList： " + dest.text);
            }
            catch (Exception ex)
            {
                data.errorMsg = "获取文件列表失败：" + ex.Message;
                data.success = false;
                LoggerX.Error(data.errorMsg);
            }

            data.isFinished = true;
            return dest?.text;
        }

        private static bool FtpIsDirExist(FtpData data)
        {
            var dirName = GetDirName(data.ftpUrl);
            LoggerX.Info("dirName：" + dirName);
            if (string.IsNullOrEmpty(dirName)) //根目录
            {
                return true;
            }

            var newData = data.Clone(new FtpDownloadDestString());
            newData.ftpUrl = GetParentDirPath(data.ftpUrl);
            LoggerX.Info("newData.ftpUrl " + newData.ftpUrl);
            var text = FtpGetFileList(newData);
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            var files = text.Split('\n');
            foreach (var file in files)
            {
                if (file == dirName)
                {
                    return true;
                }
            }

            return false;
        }

        private static long FtpGetFileSize(FtpData data)
        {
            FtpDownloadDestString dest = null;
            long ret = 0;
            try
            {
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(data.ftpUrl);
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(data.userName, data.passward);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.Timeout = 15000;

                using (var response = reqFTP.GetResponse())
                {
                    ret = response.ContentLength;
                }

                data.success = true;
                LoggerX.Info("FtpGetFileSize： " + ret);
            }
            catch (Exception ex)
            {
                data.errorMsg = "获取文件列文件大小失败：" + ex.Message;
                data.success = false;
                LoggerX.Error(data.errorMsg);
            }

            data.isFinished = true;
            return ret;
        }
    }
}