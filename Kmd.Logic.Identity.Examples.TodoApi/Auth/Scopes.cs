namespace Kmd.Logic.Identity.Examples.TodoApi.Auth
{
    public static class Scopes
    {
        public const string ScopeClaimTypeName = "http://schemas.microsoft.com/identity/claims/scope";
        public const string Read = "todos.read";
        public const string Write = "todos.write";
        public const string Admin = "todos.admin";
    }
}