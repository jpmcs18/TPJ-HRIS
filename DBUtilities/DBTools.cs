using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtilities
{
    public class DBTools : IDisposable
    {
        private readonly SqlConnection Connection;
        private readonly SqlCommand Command;
        private SqlDataAdapter Adapter { get; set; }
        private SqlTransaction Transaction { get; set; }

        public DBTools()
        {
            if (Connection?.State != ConnectionState.Open)
            {
                Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["TPJDB"].ConnectionString);
                Command = new SqlCommand();
            }
        }

        public DBTools(string ConnectionStringName)
        {
            if (Connection?.State != ConnectionState.Open)
            {
                Connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString);
                Command = new SqlCommand();
            }
        }

        public void OpenConnection()
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
        }

        public void CloseConnection()
        {
            if (Connection.State != ConnectionState.Closed && !HasTransaction()) Connection.Close();
        }

        public void StartTransaction()
        {
            if (!HasTransaction())
            {
                OpenConnection();
                Transaction = Connection.BeginTransaction(IsolationLevel.Serializable);
            }
        }

        public void CommitTransaction()
        {
            if (HasTransaction()) Transaction.Commit();

            CloseConnection();
        }

        public void RollBackTransaction()
        {
            if (HasTransaction()) Transaction.Rollback();

            CloseConnection();
        }

        public bool HasTransaction()
        {
            return Transaction != null;
        }

        public DataSet ExecuteReader(string proc, Dictionary<string, object> parameters = null)
        {
            OpenConnection();

            DataSet ds = new DataSet();
            Command.Connection = Connection;
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = proc;
            Command.Parameters.Clear();

            if (HasTransaction())
            {
                Command.Transaction = Transaction;
            }

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    Command.Parameters.AddWithValue(pair.Key, pair.Value ?? DBNull.Value);
                }
            }

            Adapter = new SqlDataAdapter(Command);
            Adapter.Fill(ds);

            CloseConnection();

            return ds;
        }

        public DataSet ExecuteReader(string proc, ref List<OutParameters> outParameters, Dictionary<string, object> parameters = null)
        {
            OpenConnection();

            DataSet ds = new DataSet();
            Command.Connection = Connection;
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = proc;
            Command.Parameters.Clear();

            if (HasTransaction())
            {
                Command.Transaction = Transaction;
            }
            
            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    Command.Parameters.AddWithValue(pair.Key, pair.Value ?? DBNull.Value);
                }
            }

            foreach (OutParameters p in outParameters)
            {
                var parameter = new SqlParameter
                {
                    ParameterName = p.ParameterName,
                    SqlDbType = p.Type,
                    SqlValue = p.Value ?? DBNull.Value
                };

                Command.Parameters.Add(parameter).Direction = ParameterDirection.InputOutput;
            }

            Adapter = new SqlDataAdapter(Command);
            Adapter.Fill(ds);

            foreach (OutParameters p in outParameters)
            {
                p.Value = Command.Parameters[p.ParameterName].Value;
            }

            CloseConnection();

            return ds;
        }

        public object ExecuteScalar(string proc, Dictionary<string, object> parameters = null)
        {
            OpenConnection();

            Command.Connection = Connection;
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = proc;
            Command.Parameters.Clear();

            if (HasTransaction())
            {
                Command.Transaction = Transaction;
            }

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    Command.Parameters.AddWithValue(pair.Key, pair.Value ?? DBNull.Value);
                }
            }

            var retVal = Command.ExecuteScalar();

            CloseConnection();

            return retVal;

        }

        public void ExecuteNonQuery(string proc, Dictionary<string, object> parameters = null)
        {
            Command.Connection = Connection;
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = proc;
            Command.Parameters.Clear();

            if (HasTransaction())
            {
                Command.Transaction = Transaction;
            }

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    Command.Parameters.AddWithValue(pair.Key, pair.Value ?? DBNull.Value);
                }
            }

            OpenConnection();

            Command.ExecuteNonQuery();

            CloseConnection();
        }

        public void ExecuteNonQuery(string proc, ref List<OutParameters> outParameters, Dictionary<string, object> parameters = null)
        {
            Command.Connection = Connection;
            Command.CommandType = CommandType.StoredProcedure;
            Command.CommandText = proc;
            Command.Parameters.Clear();

            if (HasTransaction())
            {
                Command.Transaction = Transaction;
            }

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> pair in parameters)
                {
                    Command.Parameters.AddWithValue(pair.Key, pair.Value ?? DBNull.Value);
                }
            }

            foreach (OutParameters p in outParameters)
            {
                var parameter = new SqlParameter
                {
                    ParameterName = p.ParameterName,
                    SqlDbType = p.Type,
                    SqlValue = p.Value ?? DBNull.Value
                };

                Command.Parameters.Add(parameter).Direction = ParameterDirection.InputOutput;
            }

            OpenConnection();

            Command.ExecuteNonQuery();

            foreach (OutParameters p in outParameters)
            {
                p.Value = Command.Parameters[p.ParameterName].Value;
            }

            CloseConnection();
         }

        public void Dispose()
        {
            if (!HasTransaction())
            {
                if (Connection != null) Connection.Dispose();
                if (Command != null) Command.Dispose();
                if (Adapter != null) Adapter.Dispose();
                if (Transaction != null) Transaction.Dispose();
            }
        }
    }

    public class OutParameters
    {
        public string ParameterName { get; set; }
        public SqlDbType Type { get; set; }
        public object Value { get; set; }
    }

}
