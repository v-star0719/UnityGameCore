using System;
using System.IO;
using UnityEngine;

namespace Kernel.Core
{
    public class FileUtils
    {
        //如果目标目录不存在，则失败
        //同步的对象可以是文件，也可以是目录
        //todo 缓存文件和目录的哈希值
        public static void Sync(string source, string target)
        {
            if (File.Exists(source))
            {
                if (!File.Exists(target) || IsFileDiff(source, target))
                {
                    File.Copy(source, target, true);
                    Debug.Log($"copy file: {source} => {target}");
                }
            }
            else if(Directory.Exists(source))
            {
                if (!Directory.Exists(target))
                {
                    CopyDirectory(source, target, true);
                    Debug.Log($"Copy directory: {source} => {target}");
                }
                else
                {
                    var sourceDirectInfo = new DirectoryInfo(source);
                    var targetDirectInfo = new DirectoryInfo(target);
                    SyncDirectories(source, target, sourceDirectInfo.GetDirectories(), targetDirectInfo.GetDirectories());
                    SyncFiles(source, target, sourceDirectInfo.GetFiles(), targetDirectInfo.GetFiles());
                }
            }
            else
            {
                Debug.Log($"unknown source: {source}");
            }
        }

        private static void SyncDirectories(string sourceRoot, string targetRoot, DirectoryInfo[] sourceDirectories, DirectoryInfo[] targetDirectories)
        {
            //删除target中多余的目录，两边都有的进行同步
            for(var i = 0; i < targetDirectories.Length; i++)
            {
                var td = targetDirectories[i];
                bool find = false;
                for(var j = 0; j < sourceDirectories.Length; j++)
                {
                    var sd = sourceDirectories[j];
                    if(sd.Name == td.Name)
                    {
                        find = true;
                        sourceDirectories[j] = null;
                        Sync(sd.FullName, td.FullName);
                        break;
                    }
                }

                if(!find)
                {
                    Directory.Delete(td.FullName);
                    Debug.Log($"Delete {td.FullName}");
                }
            }
            //target中没有的目录复制过去，有的进行同步
            foreach(var sd in sourceDirectories)
            {
                if(sd != null)
                {
                    Sync(sd.FullName, Path.Combine(targetRoot, sd.Name));
                }
            }
        }

        private static void SyncFiles(string sourceRoot, string targetRoot, FileInfo[] sourceFiles, FileInfo[] targetFiles)
        {
            //删除target中多余的文件，两边都有的进行同步
            for(var i = 0; i < targetFiles.Length; i++)
            {
                var tf = targetFiles[i];
                bool find = false;
                for(var j = 0; j < sourceFiles.Length; j++)
                {
                    var sd = sourceFiles[j];
                    if(sd.Name == tf.Name)
                    {
                        find = true;
                        sourceFiles[j] = null;
                        if(IsFileDiff(sd.FullName, tf.FullName))
                        {
                            File.Copy(sd.FullName, tf.FullName);
                            Debug.Log($"copy file: {sd.FullName} => {tf.FullName}");
                        }
                        break;
                    }
                }

                if(!find)
                {
                    File.Delete(tf.FullName);
                    Debug.Log($"Delete {tf.FullName}");
                }
            }
            //target中没有的目录复制过去，有的进行同步
            foreach(var sf in sourceFiles)
            {
                if(sf != null)
                {
                    File.Copy(sf.FullName, Path.Combine(targetRoot, sf.Name));
                    Debug.Log($"copy file: {sf.FullName} => {Path.Combine(targetRoot, sf.Name)}");
                }
            }
        }

        public static bool IsFileDiff(string source, string target)
        {
            if (File.GetLastWriteTime(source) != File.GetLastWriteTime(target))
            {
                return true;
            }

            FileStream sourceFileStream = new FileStream(source, FileMode.Open);
            FileStream targetFileStream = new FileStream(source, FileMode.Open);
            using (sourceFileStream)
            {
                using (targetFileStream)
                {
                    if(sourceFileStream.Length != targetFileStream.Length)
                    {
                        return true;
                    }

                    var sourceBytes = new byte[1024];
                    var targetBytes = new byte[sourceBytes.Length];
                    while(sourceFileStream.CanRead)
                    {
                        var n = sourceFileStream.Read(sourceBytes);
                        targetFileStream.Read(targetBytes);
                        for(int i = 0; i < n; i++)
                        {
                            if(sourceBytes[i] != targetBytes[i])
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if(!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach(FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if(recursive)
            {
                // Cache directories before we start copying
                DirectoryInfo[] dirs = dir.GetDirectories();

                foreach(DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}