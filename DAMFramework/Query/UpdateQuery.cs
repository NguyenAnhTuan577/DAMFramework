﻿using DAM_ORMFramework.Attribute;
using DAM_ORMFramework.Mapping;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public class UpdateQuery<T> : SqlQuery where T : new()
    {
        public UpdateQuery(SqlConnection cnn, string cnnString, T obj) : base(cnn, cnnString)
        {
            SqlMapper mapper = new SqlMapper();

            string tableName = mapper.GetTableName<T>();
            List<PrimaryKey> primaryKeys = mapper.GetPrimaryKeys<T>();
            Dictionary<Column, object> listColumnValues = mapper.GetColumnValues<T>(obj);

            if (listColumnValues != null && primaryKeys != null)
            {
                string setStr = string.Empty;
                string whereStr = string.Empty;

                foreach (Column column in listColumnValues.Keys)
                {
                    string format = "{0} = {1}, ";
                    if (column.Type == DataType.NCHAR || column.Type == DataType.NVARCHAR)
                        format = "{0} = N'{1}', ";
                    else if (column.Type == DataType.CHAR || column.Type == DataType.VARCHAR)
                        format = "{0} = '{1}', ";

                    setStr += string.Format(format, column.Name, listColumnValues[column]);
                }
                if (!string.IsNullOrEmpty(setStr))
                    setStr = setStr.Substring(0, setStr.Length - 2);

                foreach (PrimaryKey primaryKey in primaryKeys)
                {
                    Column column = mapper.FindColumn(primaryKey.Name, listColumnValues);
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
                    query = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, setStr, whereStr);
                }
            }
        }
    }
}
