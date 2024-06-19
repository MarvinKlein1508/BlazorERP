namespace BlazorERP.Core.Interfaces;

/// <summary>
/// Defines an interface used for filters to exclude blocked identifiers from search results.
/// </summary>
/// <typeparam name="TIdentifier"></typeparam>
public interface IBlockedList<TIdentifier>
{
    List<TIdentifier> Blocked { get; set; }
}
