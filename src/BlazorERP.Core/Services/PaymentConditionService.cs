using BlazorERP.Core.Filters;
using BlazorERP.Core.Interfaces;
using BlazorERP.Core.Models;
using System.Text;

namespace BlazorERP.Core.Services;

public class PaymentConditionService : IModelService<PaymentCondition, int?, PaymentConditionFilter>, ITranslationCode
{
    private readonly TranslationService _translationService;

    public PaymentConditionService(TranslationService translationService)
    {
        _translationService = translationService;
    }

    public async Task CreateAsync(PaymentCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            """
            INSERT INTO PAYMENT_CONDITIONS
            (
                NET_DAYS,
                DISCOUNT1_DAYS,
                DISCOUNT1_PERCENT,
                DISCOUNT2_DAYS,
                DISCOUNT2_PERCENT,
                IS_PREPAYMENT,
                IS_CASH_PAYMENT,
                IS_DIRECT_DEBIT,
                IS_INVOICE,
                IS_ACTIVE,
                AVAILABLE_FOR_CUSTOMER,
                AVAILABLE_FOR_SUPPLIER,
                LAST_MODIFIED_BY,
                LAST_MODIFIED
            )
            VALUES
            (
                @NET_DAYS,
                @DISCOUNT1_DAYS,
                @DISCOUNT1_PERCENT,
                @DISCOUNT2_DAYS,
                @DISCOUNT2_PERCENT,
                @IS_PREPAYMENT,
                @IS_CASH_PAYMENT,
                @IS_DIRECT_DEBIT,
                @IS_INVOICE,
                @IS_ACTIVE,
                @AVAILABLE_FOR_CUSTOMER,
                @AVAILABLE_FOR_SUPPLIER,
                @LAST_MODIFIED_BY,
                @LAST_MODIFIED
            ) RETURNING PAYMENT_CONDITION_ID;
            """;



        input.PaymentConditionId = await dbController.GetFirstAsync<int>(sql, input.GetParameters(), cancellationToken);

        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.PaymentConditionId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }

    }

    public Task DeleteAsync(PaymentCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql = "DELETE FROM PAYMENT_CONDITIONS WHERE PAYMENT_CONDITION_ID = @PAYMENT_CONDITION_ID";

        return dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);
    }

    public static async Task<List<PaymentCondition>> GetAsync(IDbController dbController)
    {
        string sql = "SELECT * FROM PAYMENT_CONDITIONS";

        var results = await dbController.SelectDataAsync<PaymentCondition>(sql);


        var translations = await TranslationService.GetAsync(GetTranslationCode(), dbController);

        foreach (var item in results)
        {
            item.Translations = translations.Where(x => x.ParentId == item.PaymentConditionId).ToList();
        }

        return results;
    }
    public async Task<PaymentCondition?> GetAsync(int? paymentConditionId, IDbController dbController, CancellationToken cancellationToken = default)
    {
        if (paymentConditionId is null)
        {
            return null;
        }

        string sql =
            """
            SELECT 
                PC.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME
            FROM PAYMENT_CONDITIONS PC
            LEFT JOIN USERS U ON (U.USER_ID = PC.LAST_MODIFIED_BY)
            WHERE 
                PAYMENT_CONDITION_ID = @PAYMENT_CONDITION_ID
            """;

        var result = await dbController.GetFirstAsync<PaymentCondition>(sql, new
        {
            PAYMENT_CONDITION_ID = paymentConditionId
        }, cancellationToken);

        if (result is not null)
        {
            result.Translations = await _translationService.GetAsync(GetTranslationCode(), result.PaymentConditionId, dbController, cancellationToken);
        }

        return result;
    }

    public async Task<List<PaymentCondition>> GetAsync(PaymentConditionFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
        $"""
        SELECT 
            FIRST {filter.Limit} SKIP {(filter.PageNumber - 1) * filter.Limit}
                PC.*,
                U.DISPLAY_NAME AS BEARBEITER_NAME 
            FROM PAYMENT_CONDITIONS PC 
            LEFT JOIN USERS U ON (U.USER_ID = PC.LAST_MODIFIED_BY)
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            ORDER BY PAYMENT_CONDITION_ID DESC
        """;

        var results = await dbController.SelectDataAsync<PaymentCondition>(sql, filter.GetParameters(), cancellationToken);
        if (results.Count > 0)
        {
            var paymentConditionIds = results.Select(x => x.PaymentConditionId).ToArray();
            var translations = await _translationService.GetAsync(GetTranslationCode(), paymentConditionIds, dbController, cancellationToken);
            foreach (var item in results)
            {
                item.Translations = translations.Where(x => x.ParentId == item.PaymentConditionId).ToList();
            }
        }

        return results;
    }

 

    public string GetFilterWhere(PaymentConditionFilter filter)
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

    public Task<int> GetTotalAsync(PaymentConditionFilter filter, IDbController dbController, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string sql =
            $"""
            SELECT 
                COUNT(*)
            FROM PAYMENT_CONDITIONS PC
            WHERE 1 = 1
            {GetFilterWhere(filter)}
            """;


        return dbController.GetFirstAsync<int>(sql, filter.GetParameters(), cancellationToken);
    }

    public static string GetTranslationCode() => "PAYMENT_CONDITION";

    public async Task UpdateAsync(PaymentCondition input, IDbController dbController, CancellationToken cancellationToken = default)
    {
        string sql =
           """
            UPDATE PAYMENT_CONDITIONS SET 
                NET_DAYS = @NET_DAYS,
                DISCOUNT1_DAYS = @DISCOUNT1_DAYS,
                DISCOUNT1_PERCENT = @DISCOUNT1_PERCENT,
                DISCOUNT2_DAYS = @DISCOUNT2_DAYS,
                DISCOUNT2_PERCENT = @DISCOUNT2_PERCENT,
                IS_PREPAYMENT = @IS_PREPAYMENT,
                IS_CASH_PAYMENT = @IS_CASH_PAYMENT,
                IS_DIRECT_DEBIT = @IS_DIRECT_DEBIT,
                IS_INVOICE = @IS_INVOICE,
                IS_ACTIVE = @IS_ACTIVE,
                AVAILABLE_FOR_CUSTOMER = @AVAILABLE_FOR_CUSTOMER,
                AVAILABLE_FOR_SUPPLIER = @AVAILABLE_FOR_SUPPLIER,
                LAST_MODIFIED_BY = @LAST_MODIFIED_BY,
                LAST_MODIFIED = @LAST_MODIFIED           
            WHERE
                PAYMENT_CONDITION_ID = @PAYMENT_CONDITION_ID
            """;


        await dbController.QueryAsync(sql, input.GetParameters(), cancellationToken);


        await _translationService.ClearAsync(GetTranslationCode(), input.PaymentConditionId, dbController, cancellationToken);
        foreach (var item in input.Translations)
        {
            item.Code = GetTranslationCode();
            item.ParentId = input.PaymentConditionId;

            await _translationService.CreateAsync(item, dbController, cancellationToken);
        }
    }

    public Task<List<PaymentCondition>> GetAsync(IEnumerable<int?> identifiers, IDbController dbController, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
