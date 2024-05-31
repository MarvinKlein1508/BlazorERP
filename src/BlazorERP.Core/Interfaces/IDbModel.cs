namespace BlazorERP.Core.Interfaces;

public interface IDbModel<T> : IDbParameterizable
{
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? LastModifiedBy { get; set; }
    public DateTime? LastModified { get; set; }


    public string CreatedByName { get; set; }
    public string LastModifiedName { get; set; }
    T GetIdentifier();
}

public interface IDbModelWithName<T> : IDbModel<T>
{
    string GetName();
}
