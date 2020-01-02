using DAM_ORMFramework.Attribute;
using DAM_ORMFramework.Mapping;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public class SelectQuery<T> : SqlQuery, WhereClause<T>, HavingClause<T>, GroupByClause<T>, CompletedQuery<T> where T : new()
    {
        private SelectQuery(SqlConnection cnn, string connectionString) : base(cnn, connectionString)
        {
            SqlMapper mapper = new SqlMapper();
            query += "SELECT";
            foreach (Column column in mapper.GetColumns<T>())
                query = string.Format("{0} {1},", query, column.Name);

            query = query.Substring(0, query.Length - 1);

            query = string.Format("{0} FROM {1}", query, mapper.GetTable<T>());
        }

        public static WhereClause<T> Create(SqlConnection cnn, string connectionString)
        {
            return new SelectQuery<T>(cnn, connectionString);
        }

        public HavingClause<T> Where(string condition)
        {
            query = string.Format("{0} WHERE {1}", query, condition);
            return this;
        }

        public HavingClause<T> Rows()
        {
            return this;
        }

        public GroupByClause<T> Having(string condition)
        {
            query = string.Format("{0} HAVING {1}", query, condition);
            return this;
        }

        public CompletedQuery<T> GroupBy(string column)
        {
            query = string.Format("{0} GROUP BY {1}", query, column);
            return this;
        }

        public List<T> Execute()
        {
            return ExecuteQuery<T>();
        }
    }
}
