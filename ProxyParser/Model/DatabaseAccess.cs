using Dapper;
using System.Data;
using System.Data.SQLite;

namespace ProxyParser.Model
{
    public class DatabaseAccess
    {
        private string connectionString;
        private IDbConnection dbConnection;
        public DatabaseAccess(string _connectionString)
        {
            connectionString = _connectionString;
            dbConnection = new SQLiteConnection(connectionString);
        }

        public List<Proxy> GetProxyList()
        {
            List<Proxy> result = new List<Proxy>();
            string sqlQuery = "SELECT * FROM Proxy";
            result = dbConnection.Query<Proxy>(sqlQuery).ToList();

            return result;
        }

        public void AddProxyRecord(Proxy record)
        {
            string sqlQuery = "INSERT INTO Proxy(Type, IpAddress, Port, Country, Anonymity) VALUES (@Type, @IpAddress, @Port, @Country, @Anonymity)";
            dbConnection.Execute(sqlQuery, record);
        }

        public void DeleteProxyRecord(int Id)
        {
            string sqlQuery = "DELETE FROM Proxy WHERE id=" + Id.ToString() + ";";
            dbConnection.Execute(sqlQuery);
        }
    }
}
