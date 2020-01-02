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
    public class DeleteQuery<T> : SqlQuery where T : new()
    {
        public DeleteQuery(SqlConnection cnn, string cnnString, T obj) : base(cnn, cnnString)
        {
            SqlMapper mapper = new SqlMapper();
            string tableName = mapper.GetTable<T>();
            List<PrimaryKey> primaryKeys = mapper.GetPK<T>();
            Dictionary<Column, object> listColumnValues = mapper.GetValuesOfColumn<T>(obj);

            string whereStr = string.Empty;
            foreach (PrimaryKey primaryKey in primaryKeys)
            {
                Column column = mapper.GetColumn(primaryKey.Name, listColumnValues);
                if (column != null)
                {
                    string format = "{0} = {1}, ";
                    if (column.Type == DataType.NCHAR || column.Type == DataType.NVARCHAR)
                        format = "{0} = N'{1}', ";
                    else if (column.Type == DataType.CHAR || column.Type == DataType.VARCHAR)
                        format = "{0} = '{1}', ";

                    whereStr += string.Format(format, primaryKey.Name, listColumnValues[column]);
                }
            }
            if (!string.IsNullOrEmpty(whereStr))
            {
                whereStr = whereStr.Substring(0, whereStr.Length - 2);
                query = string.Format("DELETE {0} WHERE {1}", tableName, whereStr);
            }
        }
    }
}
