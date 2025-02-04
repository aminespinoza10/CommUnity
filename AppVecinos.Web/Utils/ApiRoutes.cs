public static class ApiRoutes
{
    public const string BaseUrl = "https://neighborapi.blackbeach-49468bd1.eastus2.azurecontainerapps.io/";
    
    public static class Auth
    {
        public const string Login = "login";
    }

    public static class Neighbors
    {
        public const string GetAll = BaseUrl + "neighbors";
        public const string Create = BaseUrl + "neighbors";
        public const string Edit = BaseUrl + "neighbors";
        public const string GetNeighborById = BaseUrl + "neighbors";
    }

}
