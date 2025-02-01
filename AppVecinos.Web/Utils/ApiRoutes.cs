public static class ApiRoutes
{
    public const string BaseUrl = "http://localhost:5081/";
    
    public static class Auth
    {
        public const string Login = "login";
    }

    public static class Neighbors
    {
        public const string GetAll = BaseUrl + "neighbors";
        public const string Create = BaseUrl + "neighbors";
    }

}
