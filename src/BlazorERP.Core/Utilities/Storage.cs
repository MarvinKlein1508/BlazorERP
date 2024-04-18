using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using BlazorERP.Core.Services;
using Microsoft.Extensions.Configuration;

namespace BlazorERP.Core.Utilities;

public static class Storage
{
    private static IConfiguration? _configuration;
    private readonly static Dictionary<Type, object> _storage = [];

    public static Währung EmptyWährung => new()
    {
        Code = string.Empty
    };
    public static Sprache EmptySprache => new()
    {
         SprachId = -1,
         Name = "--- Auswählen ---"
    };

    public static Zahlungsbedingung EmptyZahlungsbedingung => new()
    {
        ZahlungsbedingungId = -1,
        Name = "--- Auswählen ---"
    };
    public static Lieferbedingung EmptyLieferbedingung => new()
    {
        LieferbedingungId = -1,
        Name = "--- Auswählen ---"
    };
    public static Land EmptyLand => new()
    {
        LandId = -1,
        Name = "--- Auswählen ---"
    };

    public static Anrede EmptyAnrede => new()
    {
        AnredeId = -1,
        Name = "--- Auswählen ---"
    };

    public static async Task InitAsync(IConfiguration configuration)
    {
        _configuration = configuration;

       using IDbController dbController = new FbController();
        _storage.Add(typeof(Anrede), await AnredeService.GetAsync(dbController));
        _storage.Add(typeof(Lieferbedingung), await LieferbedingungService.GetAsync(dbController));
        _storage.Add(typeof(Zahlungsbedingung), await ZahlungsbedingungService.GetAsync(dbController));
        _storage.Add(typeof(Währung), await WährungService.GetAsync(dbController));
        _storage.Add(typeof(Sprache), await SpracheService.GetAsync(dbController));
        _storage.Add(typeof(Kostenstelle), await KostenstelleService.GetAsync(dbController));
        _storage.Add(typeof(Land), await LandService.GetAsync(dbController));
    }

    /// <summary>
    /// Creates or updates an object in the corresponding list fro the type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    public static void UpdateStorage<T, TIdentifier>(T input) where T : class, IDbModel<TIdentifier>
    {
        if (!_storage.ContainsKey(typeof(T)))
        {
            return;
        }

        var list = _storage[typeof(T)] as List<T>;

        if(list is null)
        {
            return;
        }

        var existingItem = list.FirstOrDefault(x => x?.GetIdentifier()?.Equals(input.GetIdentifier()) ?? false);

        if (existingItem == null)
        {
            list.Add(input);
        }
        else
        {
            int index = list.IndexOf(existingItem);
            list[index] = input;
        }
    }

    /// <summary>
    /// Deletes an item from the corresponding object list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    public static void DeleteFromStorage<T, TIdentifier>(T input) where T : class, IDbModel<TIdentifier>
    {
        var storage = _storage.GetValueOrDefault(typeof(T)) as List<T>;

        var item = storage?.Cast<T>().FirstOrDefault(x => x.GetIdentifier()?.Equals(input.GetIdentifier()) ?? false);

        if (item is not null)
        {
            storage!.Remove(item);
        }
    }

    /// <summary>
    /// Gets the corresponding list for the type <see cref="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>This method never returns null. When no list for <see cref="T"/> is specified, it returns a new empty list</returns>
    public static IEnumerable<T> Get<T>() where T : class
    {
        if (_storage.ContainsKey(typeof(T)))
        {
            var storage = _storage.GetValueOrDefault(typeof(T)) as List<T> ?? [];

            foreach (var item in storage)
            {
                yield return (T)item;
            }
        }
    }

    /// <summary>
    /// Gets the corresponding item for the specified identifier.
    /// </summary>
    /// <typeparam name="T">The type of item.</typeparam>
    /// <typeparam name="TIdentifier">The type of identifier.</typeparam>
    /// <param name="identifier">The identifier of the item to retrieve.</param>
    /// <returns>When found, this method returns an item of type <see cref="T"/>, otherwise it returns null.</returns>
    public static T? Get<T, TIdentifier>(TIdentifier identifier) where T : class, IDbModel<TIdentifier>
    {
        if (identifier is null)
        {
            return null;
        }

        foreach (var item in Get<T>())
        {
            if (item.GetIdentifier()?.Equals(identifier) ?? false)
            {
                return item;
            }
        }

        return null;
    }


    /// <summary>
    /// Ruft den Namen für eine generalisierte Klasse ab.
    /// </summary>
    /// <typeparam name="T">Der Typ des Objekts.</typeparam>
    /// <typeparam name="TIdentifier">Der Typ des Bezeichners.</typeparam>
    /// <param name="identifier">Der Bezeichner des Objekts.</param>
    /// <returns>Wenn kein Name gefunden wird, gibt die Funktion "Unbekannt" zurück.</returns>
    public static string GetName<T, TIdentifier>(TIdentifier? identifier) where T : class, IDbModelWithName<TIdentifier>, new()
    {
        if (identifier is 0 or null)
        {
            return string.Empty;
        }

        T? item = Get<T, TIdentifier>(identifier);

        return item is null ? "Unbekannt" : item.GetName();
    }

}
