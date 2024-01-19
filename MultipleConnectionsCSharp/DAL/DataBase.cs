using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using FirebirdSql.Data.FirebirdClient;
using Oracle.ManagedDataAccess.Client;

namespace MultipleConnectionsCSharp.DAL
{
    public class DataBase
    {
        private readonly string connectionString;

        public DataBase(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                this.connectionString = connectionString;
            }
            else
            {
                // Adjust the key as per your configuration
                this.connectionString = ConfigurationManager.ConnectionStrings["Entities"]?.ConnectionString;
            }
        }

        public IDbConnection GetConnection()
        {
            if (IsFirebird())
            {
                return FirebirdConnection;
            }
            else if (IsMSSQL())
            {
                return SqlConnection;
            }
            else if (IsOracle())
            {
                return OracleConnection;
            }

            return null;
        }

        private IDbConnection FirebirdConnection => new FbConnection(connectionString);
        private IDbConnection SqlConnection => new SqlConnection(connectionString);
        private IDbConnection OracleConnection => new OracleConnection(connectionString);

        public string GetConnectionString()
        {
            return connectionString;
        }

        public bool IsFirebird()
        {
            return connectionString.Contains("Firebird") || connectionString.Contains("SYSDBA");
        }

        public bool IsOracle()
        {
            return connectionString.Contains("Oracle");
        }

        public bool IsMSSQL()
        {
            return connectionString.Contains("SqlServer") || connectionString.Contains("SQL Server");
        }
    }
}
