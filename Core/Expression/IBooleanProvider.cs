namespace GameCore.Core.Expression
{
    public interface IBooleanProvider : IProvider
    {
        bool GetVariableValue(int key);
    }
}
