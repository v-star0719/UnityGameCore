namespace GameCore.Core.Expression
{
    public interface IBooleanExpression : IExpression
    {
        bool GetValue(IBooleanProvider provider);
    }
}
