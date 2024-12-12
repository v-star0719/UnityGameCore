namespace GameCore.Scripts
{
    public interface IBooleanProvider : IProvider
    {
        bool GetVariableValue(int key);
    }
}
