using System.IO;

namespace GameCore.Core.Storage
{
    //以文件的方式保存和读取数据
    //Save and load data using files.
    public class StorageFileLoaderSaver : IStorageLoaderSaver
    {
        private string filePath;

        public StorageFileLoaderSaver(string filePath)
        {
            this.filePath = filePath;
        }

        public byte[] Load()
        {
            if(File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            return null;
        }

        public void Save(byte[] data)
        {
            File.WriteAllBytes(filePath, data);
        }
    }
}