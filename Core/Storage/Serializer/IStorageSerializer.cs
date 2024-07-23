namespace Kernel.Storage
{
    public interface IStorageSerializer
    {
        byte[] Serialize<T>(T data);
        T Deserialize<T>(byte[] bytes) where T : new();//没有数据时，会传入null
    }
}
