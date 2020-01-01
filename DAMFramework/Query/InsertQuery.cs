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
    public class InsertQuery<T> : SqlQuery where T : new()
    {
        public InsertQuery(SqlConnection cnn, string cnnString, T obj) : base(cnn, cnnString)
        {
            SqlMapper mapper = new SqlMapper();

            string tableName = mapper.GetTable<T>();
            List<PrimaryKey> primaryKeys = mapper.GetPK<T>();
            Dictionary<Column, object> listColumnNameValues = mapper.GetColumnValues<T>(obj);

            if (listColumnNameValues.Count != 0)
            {
                string columnStr = string.Empty;
                string valueStr = string.Empty;

                foreach (Column column in listColumnNameValues.Keys)
                {
                    bool isAutoID = false;
                    foreach (PrimaryKey primaryKey in primaryKeys)
                    {
                        if (column.Name == primaryKey.Name && primaryKey.GenerateID)
                        {
                            isAutoID = true;
                            break;
                        }
                    }

                    if (!isAutoID)
                    {
                        string format = "{0}, ";
                        if (column.Type == DataType.NCHAR || column.Type == DataType.NVARCHAR)
                            format = "N'{0}', ";
                        else if (column.Type == DataType.CHAR || column.Type == DataType.VARCHAR)
                            format = "'{0}', ";
                        columnStr += string.Format("{0}, ", column.Name);
                        valueStr += string.Format(format, listColumnNameValues[column]);
                    }
                }
                if (!string.IsNullOrEmpty(columnStr) && !string.IsNullOrEmpty(valueStr))
                {
                    columnStr = columnStr.Substring(0, columnStr.Length - 2);
                    valueStr = valueStr.Substring(0, valueStr.Length - 2);
                    query = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, columnStr, valueStr);
                }
            }
        }
    }
}
