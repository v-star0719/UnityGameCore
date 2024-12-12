namespace GameCore.Storage
{
    /// 以什么格式保存数据，二进制、json、xml等等？
    public interface IStorageSerializer
    {
        byte[] Serialize<T>(T data);
        T Deserialize<T>(byte[] bytes) where T : new();//没有数据时，会传入null
    }
}
