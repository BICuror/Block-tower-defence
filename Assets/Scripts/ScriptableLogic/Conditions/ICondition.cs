public interface ICondition<T>
{
    public bool GetValue(T value);
}

public interface ICondition
{
    public bool GetValue();
}