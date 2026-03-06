namespace GameCore.Core.Storage
{
    /// 以什么方式保存和读取数据：本地文件，远程文件、内存文件？
    /// What method is used to save and read data: local files, remote files, or in-memory files?
    /// 实现这个接口，定义自己的保存和读取方式
    /// Implement this interface and define your own methods for saving and loading data.
    public interface IStorageLoaderSaver
    {
        public byte[] Load();
        public void Save(byte[] data);
    }
}