using System.Configuration;

namespace ProxyParser
{
    public class AppConfiguration
    {
        private static AppConfiguration instance = null;
        private static object lockObj = new object();
        private string connectionString;

        public static AppConfiguration Instance 
        {
            get 
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if(instance == null)
                            instance = new AppConfiguration();
                    }
                }
                return instance;
            }
        }

        private AppConfiguration()
        {
            readConfigurations();
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        private void readConfigurations()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
    }
}

