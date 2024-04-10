namespace BlazorERP.Core.Interfaces;

public interface IDbModel<T>
{
    T GetIdentifier();
}

public interface IDbModelWithName<T> : IDbModel<T>
{
    string GetName();
}
