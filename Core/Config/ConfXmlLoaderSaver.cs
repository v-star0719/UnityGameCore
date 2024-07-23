using System;
using System.IO;
using Kernel.Storage;

namespace Kernel.Core
{
    public class ConfXmlLoaderSaver
    {
        private Func<string, string> getFilePath;

        public ConfXmlLoaderSaver(Func<string, string> getFilePath)
        {
            this.getFilePath = getFilePath;
        }

        public void Save<TData>(string fileName, TData data) where TData : ConfBase
        {
            var bytes = new StorageXmlSerializer().Serialize(data);
            File.WriteAllBytes(getFilePath(fileName), bytes);
        }

        public TData Load<TData>(string fileName) where TData : ConfBase, new()
        {
            var bytes = File.ReadAllBytes(getFilePath(fileName));
            return new StorageXmlSerializer().Deserialize<TData>(bytes);
        }
    }
}