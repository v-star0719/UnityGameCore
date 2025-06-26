using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace GameCore.Core
{
    public class FileUtils
    {
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

        //通过通配符匹配文件，删除掉
        public static void DeleteFileWithMath(string dir, string pattern, SearchOption option)
        {
            foreach(string filePath in Directory.GetFiles(dir, pattern, option))
            {
                File.Delete(filePath);
            }
        }

        // 转换通配符为正则表达式
        public static string WildCardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                           .Replace(@"\*", ".*")       // 处理*（匹配任意字符）
                           .Replace(@"\?", ".")        // 处理?（匹配单个字符）
                       //.Replace(@"\[!", "[^")      // 处理[!（否定字符集）
                       //.Replace(@"\[", "[")        // 处理[（字符集开始）
                       //.Replace(@"\]", "]")        // 处理]（字符集结束）
                       + "$";
        }

        public bool IsMatch(string fileName, string pattern)
        {
            return Regex.IsMatch(fileName, WildCardToRegex(pattern), RegexOptions.IgnoreCase);
        }

        public static int GetFileCountRecursive(string path)
        {
            int count = 0;
            try
            {
                // 统计当前目录的文件
                count += Directory.EnumerateFiles(path).Count();

                // 递归统计子目录中的文件
                foreach(var subDir in Directory.EnumerateDirectories(path))
                {
                    count += GetFileCountRecursive(subDir);
                }
            }
            catch(UnauthorizedAccessException)
            {
                Debug.Log($"警告：无法访问目录 {path}，已跳过。");
            }
            catch(DirectoryNotFoundException)
            {
                Debug.Log($"错误：目录 {path} 不存在。");
            }
            return count;
        }

        public enum FileCompareType
        {
            lastWriteTime,
            binary,
            hash, //todo 未实现。需要同时缓存目录和文件的
        }

        public static bool IsFileDiff(string source, string target, FileCompareType compareType)
        {
            var sourceFileInfo = new FileInfo(source);
            var targetFileInfo = new FileInfo(target);
            if(sourceFileInfo.Length != targetFileInfo.Length)
            {
                return true;
            }

            switch(compareType)
            {
                case FileCompareType.lastWriteTime:
                    if(File.GetLastWriteTime(source) != File.GetLastWriteTime(target))
                    {
                        return true;
                    }
                    break;
                case FileCompareType.binary:
                    FileStream sourceFileStream = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    FileStream targetFileStream = new FileStream(target, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using(sourceFileStream)
                    {
                        using(targetFileStream)
                        {
                            if(sourceFileStream.Length != targetFileStream.Length)
                            {
                                return true;
                            }

                            var sourceBytes = new byte[1024];
                            var targetBytes = new byte[sourceBytes.Length];
                            while(true)
                            {
                                var n = sourceFileStream.Read(sourceBytes);
                                targetFileStream.Read(targetBytes);
                                for(int i = 0; i < n; i++)
                                {
                                    if(sourceBytes[i] != targetBytes[i])
                                    {
                                        return true;
                                    }
                                }

                                if(n == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case FileCompareType.hash:
                    Debug.Log("Todo FileSyncCompareType.hash");
                    break;
            }
            return false;
        }
    }
}