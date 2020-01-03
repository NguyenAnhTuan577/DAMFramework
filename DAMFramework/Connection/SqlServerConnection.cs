using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DAM_ORMFramework.Query;

namespace DAM_ORMFramework.Mapping
{
    class SqlServerConnection : AbstractSqlConnection
    {

        private SqlConnection cnn;

        public SqlServerConnection(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        public SqlServerConnection(string cnnString)
        {
            this.cnnString = cnnString;
            cnn = new SqlConnection(cnnString);
        }

        public override void Close()
        {
            if (cnn.State == System.Data.ConnectionState.Open)
                cnn.Close();
        }

        public override int Delete<T>(T obj)
        {
            SqlServerDeleteQuery<T> SqlServerDeleteQuery = new SqlServerDeleteQuery<T>(cnn, cnnString, obj);
            return SqlServerDeleteQuery.ExecuteNonQuery();
        }

        public override int ExecuteNonQuery(string query)
        {
            SqlServerQuery execute = new SqlServerQuery(cnn, cnnString, query);
            return execute.ExecuteNonQuery();
        }

        public override List<T> ExecuteQuery<T>(string query)
        {
            SqlServerQuery execute = new SqlServerQuery(cnn, cnnString, query);
            return execute.ExecuteQuery<T>();
        }

        public override List<T> ExecuteQueryNotRelationship<T>(string query)
        {
            SqlServerQuery execute = new SqlServerQuery(cnn, cnnString, query);
            return execute.ExecuteQueryNotRelationship<T>();
        }

        public override int Insert<T>(T obj)
        {
            SqlServerInsertQuery<T> SqlServerInsertQuery = new SqlServerInsertQuery<T>(cnn, cnnString, obj);
            return SqlServerInsertQuery.ExecuteNonQuery();
        }

        public override void Open()
        {
            if (cnn.State != System.Data.ConnectionState.Open)
                cnn.Open();
        }

        public override Query.WhereClause<T> Select<T>()
        {
            return SqlServerSelectQuery<T>.Create(cnn, cnnString);
        }

        public override int Update<T>(T obj)
        {
            SqlServerUpdateQuery<T> SqlServerUpdateQuery = new SqlServerUpdateQuery<T>(cnn, cnnString, obj);
            return SqlServerUpdateQuery.ExecuteNonQuery();
        }
    }
}
