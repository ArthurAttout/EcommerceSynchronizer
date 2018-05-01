namespace EcommerceSynchronizer.Utilities
{
    public interface IConfigurationProvider
    {
        string GetFirebaseServerToken();
        string GetDatabaseConnectionString();

    }
}