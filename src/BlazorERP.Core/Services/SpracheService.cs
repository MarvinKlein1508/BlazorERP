using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;

namespace BlazorERP.Core.Services;

public class SpracheService
{

    public static Task<List<Sprache>> GetAsync(IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = "SELECT * FROM SPRACHEN";

        return dbController.SelectDataAsync<Sprache>(sql, null, cancellationToken);
    }
}
