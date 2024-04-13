namespace BlazorERP.Core.Interfaces;

public interface IDbModel<T> : IDbParameterizable
{
    T GetIdentifier();
}

public interface IDbModelWithName<T> : IDbModel<T>
{
    string GetName();
}
