namespace BlazorERP.Core.Interfaces;

/// <summary>
/// Provides generalized CUD operations for an object Service.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IModelService<TObject> : ICreateOperation<TObject>, IUpdateOperation<TObject>, IDeleteOperation<TObject>
{

}

/// <summary>
/// <para>
/// Provides generalized CRUD operations for an object Service.
/// </para>
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <typeparam name="TIdentifier"></typeparam>
public interface IModelService<TObject, TIdentifier> : IModelService<TObject>, IGetOperation<TObject, TIdentifier>
{

}


/// <summary>
/// Provides a generalized CREATE function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface ICreateOperation<TObject>
{
    /// <summary>
    /// Saves the object as new entry in the database.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="dbController"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CreateAsync(TObject input, IDbController dbController, CancellationToken cancellationToken = default);
}

/// <summary>
/// Provides a generalized UPDATE function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IUpdateOperation<TObject>
{
    /// <summary>
    /// Updates an existing entry of the object in the database.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="dbController"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UpdateAsync(TObject input, IDbController dbController, CancellationToken cancellationToken = default);
}
/// <summary>
/// Provides a generalized DELETE function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
public interface IDeleteOperation<TObject>
{
    /// <summary>
    /// Deletes the object from the database.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="dbController"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(TObject input, IDbController dbController, CancellationToken cancellationToken = default);
}
/// <summary>
/// Provides a generalized GET function for a specific type.
/// </summary>
/// <typeparam name="TObject"></typeparam>
/// <typeparam name="TIdentifier"></typeparam>
public interface IGetOperation<TObject, TIdentifier>
{
    /// <summary>
    /// Gets the objects from the database
    /// </summary>
    /// <param name="identifier">The unique identifer for the object.</param>
    /// <param name="dbController"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// If the object does not exist than this method will return NULL.
    /// </returns>
    Task<TObject?> GetAsync(TIdentifier identifier, IDbController dbController, CancellationToken cancellationToken = default);
}