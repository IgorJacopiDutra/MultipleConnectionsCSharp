using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using FirebirdSql.Data.FirebirdClient;
using Oracle.ManagedDataAccess.Client;

namespace MultipleConnectionsCSharp.DAL
{
    public class Query
    {
        private string error;
        private readonly IDbCommand dbCommand;
        public IDataReader Reader { get; private set; }
        private readonly DataBase database;

        public List<string> SQL { get; set; }
        public int RowsAffected { get; set; }

        public Query(DataBase database)
        {
            this.database = database;
            this.dbCommand = CreateDbCommand();
            this.SQL = new List<string>();
        }

        private IDbCommand CreateDbCommand()
        {
            IDbCommand command = null;

            if (database.IsFirebird())
            {
                command = new FbCommand();
            }
            else if (database.IsMSSQL())
            {
                command = new SqlCommand();
            }
            else if (database.IsOracle())
            {
                command = new OracleCommand();
            }
            else
            {
                throw new InvalidOperationException("Tipo de banco de dados não suportado.");
            }

            return command;
        }

        public string Error
        {
            get { return error; }
            private set { error = value; }
        }

        public void Prepare()
        {
            using (var connection = database.GetConnection())
            {

                ReplaceParameterPrefix();
                connection.Open();
                string sqlText = string.Join(Environment.NewLine, SQL);

                if (dbCommand != null)
                {
                    dbCommand.CommandText = sqlText;
                    dbCommand.Connection = connection;
                }
                else
                {
                    Error = "Comando não inicializado.";
                }
            }
        }

        public void ParamByName(string name, string value)
        {
            ParamByName(name, DbType.String, value);
        }

        public void ParamByName(string name, int value)
        {
            ParamByName(name, DbType.Int32, value);
        }

        public void ParamByName(string name, double value)
        {
            ParamByName(name, DbType.Double, value);
        }

        public void ParamByName(string name, float value)
        {
            ParamByName(name, DbType.Single, value);
        }

        public void ParamByName(string name, DbType dbType, object value)
        {
            if (dbCommand != null)
            {
                IDbDataParameter parameter = dbCommand.CreateParameter();
                parameter.ParameterName = GetPrefixParam() + name;
                parameter.DbType = dbType;
                parameter.Value = value;

                dbCommand.Parameters.Add(parameter);
            }
        }

        public bool ExecuteNonQuery()
        {
            RowsAffected = 0;
            try
            {
                using (dbCommand.Connection)
                {
                    if (dbCommand != null)
                    {
                        if (dbCommand.Connection.State != ConnectionState.Open)
                        {
                            dbCommand.Connection.Open();
                        }

                        RowsAffected = dbCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        Error = "Comando não inicializado.";
                    }
                }
            }
            catch (Exception ex)
            {
                Error = $"Erro ao executar o comando: {ex.Message}";
            }

            return RowsAffected > 0;
        }

        public void ExecuteReader()
        {
            try
            {
                using (dbCommand.Connection)
                {
                    if (dbCommand != null)
                    {
                        if (dbCommand.Connection.State != ConnectionState.Open)
                        {
                            dbCommand.Connection.Open();
                        }

                        Reader = dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    }
                    else
                    {
                        Error = "Comando não inicializado.";
                    }
                }
            }
            catch (Exception ex)
            {
                Error = $"Erro ao executar o comando de leitura: {ex.Message}";
            }
        }

        private string GetPrefixParam()
        {
            if (database.IsFirebird() || database.IsMSSQL())
            {
                return "@";
            }
            else if (database.IsOracle())
            {
                return ":";
            }
            else
            {
                return "@";
            }
        }

        private string ChangePrefixParam(string text)
        {
            return text.Replace("@", GetPrefixParam());
        }

        private void ReplaceParameterPrefix()
        {
            List<string> newSQLList = new List<string>();

            foreach (var sqlStatement in SQL)
            {
                string newSQL = ChangePrefixParam(sqlStatement.ToString());
                newSQLList.Add(newSQL);
            }

            SQL.Clear();
            SQL.AddRange(newSQLList);
        }

        public void Close()
        {
            dbCommand.Parameters.Clear();
            Error = "";
        }
    }
}
