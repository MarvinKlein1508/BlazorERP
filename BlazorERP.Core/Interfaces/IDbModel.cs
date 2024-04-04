using System.Data;

namespace BlazorERP.Core.Interfaces;

public interface IDbModel
{
    int GetId();
}

public interface IDbModelWithName : IDbModel
{
    string GetName();
}
