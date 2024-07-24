using System.Collections;
using System.IO;

namespace GameCore.Core
{
    /// 方式保存数据，本地文件，远程文件、内存文件？
    public interface IStorageLoaderSaver
    {
        public byte[] Load();
        public void Save(byte[] data);
    }
}