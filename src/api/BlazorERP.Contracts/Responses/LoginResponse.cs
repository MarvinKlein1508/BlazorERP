namespace BlazorERP.Contracts.Responses;

public class LoginResponse
{
    public required string? Token { get; init; }
    public required bool Success { get; init; }
    public required string Message { get; init; }
}
