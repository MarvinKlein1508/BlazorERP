namespace BlazorERP.Api;

public static class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Users
    {
        private const string Base = $"{ApiBase}/movies";
        public const string Get = $"{Base}/{{userId:int}}";
    }

    public static class Auth
    {
        private const string Base = $"{ApiBase}/login";

        public const string PerformLogin = Base;
    }

}
