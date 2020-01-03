using DAM_ORMFramework.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public class SqlServerQuery : IQuery
    {
        protected string cnnString;
        protected SqlCommand command;
        protected string query;

        public SqlServerQuery(SqlConnection cnn, string connecttring)
        {
            cnnString = connecttring;
            command = new SqlCommand();
            command.Connection = cnn;
            command.CommandType = CommandType.Text;
        }

        public SqlServerQuery(SqlConnection cnn, string connectString, string query)
        {
            cnnString = connectString;
            command = new SqlCommand();
            command.Connection = cnn;
            command.CommandType = CommandType.Text;
            this.query = query;
        }

        public List<T> ExecuteQuery<T>() where T : new()
        {
            command.CommandText = query;

            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);

            List<T> res = new List<T>();
            SqlServerConnection cnn = new SqlServerConnection(cnnString);
            SqlServerMapper mapper = new SqlServerMapper();

            foreach (DataRow row in dataTable.Rows)
                res.Add(mapper.MapRelationship<T>(cnn, row));

            return res;
        }

        public List<T> ExecuteQueryNotRelationship<T>() where T : new()
        {
            command.CommandText = query;

            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);

            List<T> res = new List<T>();
            SqlServerConnection cnn = new SqlServerConnection(cnnString);
            SqlServerMapper mapper = new SqlServerMapper();

            foreach (DataRow row in dataTable.Rows)
                res.Add(mapper.MapNotRelationship<T>(cnn, row));

            return res;
        }

        public int ExecuteNonQuery()
        {
            command.CommandText = query;
            return command.ExecuteNonQuery();
        }

        
    }
}
