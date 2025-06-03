using System.IO;

namespace GameCore.Core
{
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