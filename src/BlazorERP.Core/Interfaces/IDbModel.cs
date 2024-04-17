namespace BlazorERP.Core.Interfaces;

public interface IDbModelWithName<T> : IDbParameterizable
{
    T GetIdentifier();
}

public interface IDbModelWithName<T> : IDbModelWithName<T>
{
    string GetName();
}
