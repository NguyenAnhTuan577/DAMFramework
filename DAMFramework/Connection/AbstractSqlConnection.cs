using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAM_ORMFramework.Query;

namespace DAM_ORMFramework.Mapping
{
    abstract class AbstractSqlConnection
    {
        protected string cnnString { get; set; }

        public abstract void Open();
        public abstract void Close();
        public abstract WhereClause<T> Select<T>() where T : new();
        public abstract int Insert<T>(T obj) where T : new();
        public abstract int Update<T>(T obj) where T : new();
        public abstract int Delete<T>(T obj) where T : new();
        public abstract List<T> ExecuteQuery<T>(string query) where T : new();
        public abstract int ExecuteNonQuery(string query);
        public abstract List<T> ExecuteQueryNotRelationship<T>(string query) where T : new();

    }
}
