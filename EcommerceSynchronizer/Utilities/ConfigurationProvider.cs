namespace EcommerceSynchronizer.Utilities
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string FirebaseServerToken { get; set; }
        public string DatabaseConnectionString { get; set; }

        public string GetFirebaseServerToken()
        {
            return FirebaseServerToken;
        }

        public string GetDatabaseConnectionString()
        {
            return DatabaseConnectionString;
        }
    }
}