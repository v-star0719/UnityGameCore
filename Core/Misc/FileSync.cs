using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameCore.Core
{
    public enum FileSyncLogType
    {
        None,
        simple,
    }

    //同步的对象可以是文件，也可以是目录
    public class FileSync
    {
        //[MenuItem("Tools/Test")]
        //public static void Test()
        //{
        //    FileSync.Sync("D:/1", "D:/2", FileUtils.FileCompareType.binary, FileSyncLogType.simple, new[] { "*.json", "json.*", "3.txt" }, (i, j) => Debug.Log($"{i}/{j}"));
        //}

        private enum FileExistStatus
        {
            Unknown,
            Exist,
            NotExist,
        }

        private string source;
        private string target;
        private FileUtils.FileCompareType compareType;
        private FileSyncLogType logType;
        private Action<int, int> progress;
        private Regex[] regexes;
        private int fileCount;
        private int counter;

        public FileSync(string source, string target, FileUtils.FileCompareType compareType, FileSyncLogType logType, string[] ignoreFiles = null, Action<int, int> progress = null)
        {
            this.source = source;
            this.target = target;
            this.compareType = compareType;
            this.logType = logType;
            this.progress = progress;
            if (ignoreFiles != null)
            {
                regexes = new Regex[ignoreFiles.Length];
                for (int i = 0; i < ignoreFiles.Length; i++)
                {
                    regexes[i] = new Regex(FileUtils.WildCardToRegex(ignoreFiles[i]));
                }
            }
        }

        public void Run()
        {
            if(File.Exists(source))
            {
                SyncOneFile(source, target, FileExistStatus.Unknown);
            }
            else if(Directory.Exists(source))
            {
                fileCount = FileUtils.GetFileCountRecursive(source);
                SyncOneDirectory(source, target, FileExistStatus.Unknown);
            }
            else
            {
                Debug.Log($"unknown source: {source}");
            }
        }
        
        private void SyncDirectories(string sourceRoot, string targetRoot, string[] sourceDirectories, string[] targetDirectories)
        {
            //删除target中多余的目录
            for(var i = 0; i < targetDirectories.Length; i++)
            {
                var td = targetDirectories[i];
                bool find = false;
                for(var j = 0; j < sourceDirectories.Length; j++)
                {
                    var sd = sourceDirectories[j];
                    if(sd != null && Path.GetFileName(sd) == Path.GetFileName(td))
                    {
                        find = true;
                        sourceDirectories[j] = null;
                        SyncOneDirectory(sd, Path.Combine(targetRoot, Path.GetFileName(sd)), FileExistStatus.Exist);
                        break;
                    }
                }

                if(!find)
                {
                    Directory.Delete(td, true);
                    if (logType >= FileSyncLogType.simple)
                    {
                        Debug.Log($"Delete {td}");
                    }
                }
            }

            //target中没有的目录复制过去
            foreach(var sd in sourceDirectories)
            {
                if (sd != null)
                {
                    SyncOneDirectory(sd, Path.Combine(targetRoot, Path.GetFileName(sd)), FileExistStatus.NotExist);
                }
            }
        }

        private void SyncOneDirectory(string source, string target, FileExistStatus targetStatus)
        {
            if (targetStatus == FileExistStatus.Unknown)
            {
                targetStatus = Directory.Exists(target) ? FileExistStatus.Exist : FileExistStatus.NotExist;
            }

            //源是一个目录
            if(targetStatus == FileExistStatus.NotExist)
            {
                var info = Directory.CreateDirectory(target);
                if(info.Exists)
                {
                    if(logType >= FileSyncLogType.simple)
                    {
                        Debug.Log($"create target directory {target}");
                    }
                }
                else
                {
                    Debug.Log($"create target directory failed: {target}");
                    return;
                }
            }
            SyncDirectories(source, target, Directory.GetDirectories(source), Directory.GetDirectories(target));
            SyncFiles(source, target, Directory.GetFiles(source), Directory.GetFiles(target));
        }

        private void SyncFiles(string sourceRoot, string targetRoot, string[] sourceFiles, string[] targetFiles)
        {
            //删除target中多余的文件，两边都有的进行同步
            for(var i = 0; i < targetFiles.Length; i++)
            {
                var tf = targetFiles[i];
                bool find = false;
                for(var j = 0; j < sourceFiles.Length; j++)
                {
                    var sf = sourceFiles[j];
                    if(sf != null && Path.GetFileName(sf) == Path.GetFileName(tf))
                    {
                        find = true;
                        sourceFiles[j] = null;
                        SyncOneFile(sf, tf, FileExistStatus.Exist);
                        break;
                    }
                }

                if(!find)
                {
                    File.Delete(tf);
                    if(logType >= FileSyncLogType.simple)
                    {
                        Debug.Log($"Delete {tf}");
                    }
                }
            }
            //target中没有的复制过去
            foreach(var sf in sourceFiles)
            {
                if(sf != null)
                {
                    SyncOneFile(sf, Path.Combine(targetRoot, Path.GetFileName(sf)), FileExistStatus.NotExist);
                }
            }
        }

        ///targetStatus: 0=未知，1=存在，2=不存在
        private void SyncOneFile(string source, string target, FileExistStatus targetStatus)
        {
            OnFinishOne();

            if (IsIgnored(source))
            {
                return;
            }

            if(targetStatus == FileExistStatus.Unknown)
            {
                targetStatus = File.Exists(target) ? FileExistStatus.Exist : FileExistStatus.NotExist;
            }

            //源是一个文件
            if( targetStatus == FileExistStatus.NotExist || FileUtils.IsFileDiff(source, target, compareType))
            {
                File.Copy(source, target, true);
                if(logType >= FileSyncLogType.simple)
                {
                    Debug.Log($"copy file: {source} => {target}");
                }
            }
        }

        private bool IsIgnored(string filePath)
        {
            if (regexes != null)
            {
                foreach (var r in regexes)
                {
                    if (r.IsMatch(Path.GetFileName(filePath)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void OnFinishOne()
        {
            counter++;
            progress?.Invoke(counter, fileCount);
        }

        public static void Sync(string source, string target, FileUtils.FileCompareType compareType = FileUtils.FileCompareType.lastWriteTime, FileSyncLogType logType = FileSyncLogType.None, string[] ignoreFiles = null, Action<int, int> progress = null)
        {
            new FileSync(source, target, compareType, logType, ignoreFiles, progress).Run();
        }
    }
}