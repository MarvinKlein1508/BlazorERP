using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class DeliveryConditionService : IModelService<DeliveryCondition, int?, DeliveryConditionFilter>, ITranslationCode
{
    private readonly TranslationService _translationService;

    public DeliveryConditionService(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task CreateAsync(DeliveryCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO DELIVERY_CONDITIONS
            (
                SHIPPING_BY_CARRIER,
                IS_PICKUP,
                AVAILABLE_FOR_CUSTOMER,
                AVAILABLE_FOR_SUPPLIER,
                IS_ACTIVE,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @SHIPPING_BY_CARRIER,
                @IS_PICKUP,
                @AVAILABLE_FOR_CUSTOMER,
                @AVAILABLE_FOR_SUPPLIER,
                @IS_ACTIVE,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING DELIVERY_CONDITION_ID;
            """;



        input.DeliveryConditionId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.DeliveryConditionId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(DeliveryCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM DELIVERY_CONDITIONS WHERE LIEFERBEDINGUNG_ID = @LIEFERBEDINGUNG_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public static async Task<List<DeliveryCondition>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM DELIVERY_CONDITIONS";

        var results = await dbController.SelectDataAsync<DeliveryCondition>(sql);


        var translations = await TranslationService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Translations = translations.Where(x => x.ParentId == item.DeliveryConditionId).ToList();
        }

        return results;
    }
    public async Task<DeliveryCondition?> GetAsync(int? deliveryConditionId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (deliveryConditionId is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                DC.*,
                UC.DISPLAY_NAME AS CreatedByName,
                U.DISPLAY_NAME AS LastModifiedName
            FROM DELIVERY_CONDITIONS DC
            LEFT JOIN USERS UC ON (U.USER_ID = DC.CREATED_BY)
            LEFT JOIN USERS UL ON (U.USER_ID = DC.LAST_MODIFIED_BY)
            WHERE 
                DELIVERY_CONDITION_ID = @DELIVERY_CONDITION_ID
            """;

        var result = await dbController.GetFirstAsync<DeliveryCondition>(sql, new
        {
            DELIVERY_CONDITION_ID = deliveryConditionId
        }, cancellationToken);

        if (result is not null)
        {
            result.Translations = await _translationService.GetAsync(GetTranslationCode(), result.DeliveryConditionId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<DeliveryCondition>> GetAsync(DeliveryConditionFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                DC.*,
                UC.DISPLAY_NAME AS CreatedByName,
                UL.DISPLAY_NAME AS LastModifiedName 
            FROM DELIVERY_CONDITIONS DC 
            LEFT JOIN USERS UC ON (U.USER_ID = DC.CREATED_BY)
            LEFT JOIN USERS UL ON (U.USER_ID = DC.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY DELIVERY_CONDITION_ID DESC
        """;

        var results = await dbController.SelectDataAsync<DeliveryCondition>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var deliveryConditionIds = results.Select(x => x.DeliveryConditionId).ToArray();
            var tranlations = await _translationService.GetAsync(GetTranslationCode(), deliveryConditionIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Translations = tranlations.Where(x => x.ParentId == item.DeliveryConditionId).ToList();
            }
        }

        return results;
    }

 

    public string GetFilterWhere(DeliveryConditionFilter filter)
    {
        StringBuilder sb = new();

        if (!string.IsNullOrWhiteSpace(filter.SearchPhrase))
        {
            sb.AppendLine(@" AND 
(
        UPPER(NAME) LIKE @SEARCH_PHRASE
)");
        }



        string sql = sb.ToString();
        return sql;
    }

    public Task<int> GetTotalAsync(DeliveryConditionFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM DELIVERY_CONDITIONS
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public static string GetTranslationCode() => "DELIVERY_CONDITION";

    public async Task UpdateAsync(DeliveryCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE DELIVERY_CONDITIONS SET 
                SHIPPING_BY_CARRIER = @SHIPPING_BY_CARRIER,
                IS_PICKUP = @IS_PICKUP,
                AVAILABLE_FOR_CUSTOMER = @AVAILABLE_FOR_CUSTOMER,
                AVAILABLE_FOR_SUPPLIER = @AVAILABLE_FOR_SUPPLIER,
                IS_ACTIVE = @IS_ACTIVE,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED
            WHERE
                DELIVERY_CONDITION_ID = @DELIVERY_CONDITION_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _translationService.ClearAsync(GetTranslationCode(), input.DeliveryConditionId, dbController, cancellationToken);
        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.DeliveryConditionId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }
    }

    public Task<List<DeliveryCondition>> GetAsync(int?[] identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
