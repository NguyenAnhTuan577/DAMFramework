using DAM_ORMFramework.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Mapping
{
    class SqlMapper : Mapper
    {
        protected override void MapOneToMany<T>(SqlServerConnection cnn, DataRow dr, T obj)
        {
            var props = typeof(T).GetProperties();
            for(int i =0;i<props.Length; i++)
            {
                var attributes = props[i].GetCustomAttributes(false);

                var relationshipAtrr = getAllAttributes(attributes, typeof(OneToMany));
                if (relationshipAtrr != null && relationshipAtrr.Length != 0)
                {
                    for(int j =0; j< relationshipAtrr.Length; j++)
                    {
                        Type type = props[i].PropertyType;
                        if (type.IsGenericType)
                        {
                            SqlMapper sqlServerMapper = new SqlMapper();
                            Type itemType = type.GetGenericArguments()[0];
                            MethodInfo getTableNameMethod = sqlServerMapper.GetType().GetMethod("GetTable")
                               .MakeGenericMethod(new Type[] { itemType });
                            string tableName = getTableNameMethod.Invoke(sqlServerMapper, null) as string;

                            MethodInfo getForeignKeyAttributeMethod = sqlServerMapper.GetType().GetMethod("GetFK")
                                .MakeGenericMethod(new Type[] { itemType });
                            List<ForeignKey> foreignKeyAttributes = getForeignKeyAttributeMethod.Invoke(sqlServerMapper, new object[] { relationshipAtrr.ReferID }) as List<ForeignKey>;

                            MethodInfo getColumnAttributeMethod = sqlServerMapper.GetType().GetMethod("GetColumns")
                                .MakeGenericMethod(typeof(T));
                            List<Column> columnAttributes = getColumnAttributeMethod.Invoke(sqlServerMapper, null) as List<Column>;

                            string whereStr = string.Empty;
                            if (foreignKeyAttributes != null)
                            {
                                foreach (ForeignKey foreignKeyAttribute in foreignKeyAttributes)
                                {
                                    Column column = GetColumn(foreignKeyAttribute.ReferID, columnAttributes);
                                    if (column != null)
                                    {
                                        string format = "{0} = {1}, ";
                                        if (column.Type == DataType.NCHAR || column.Type == DataType.NVARCHAR)
                                            format = "{0} = N'{1}', ";
                                        else if (column.Type == DataType.CHAR || column.Type == DataType.VARCHAR)
                                            format = "{0} = '{1}', ";

                                        whereStr += string.Format(format, foreignKeyAttribute.Name, dr[foreignKeyAttribute.ReferID]);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(whereStr))
                            {
                                whereStr = whereStr.Substring(0, whereStr.Length - 2);
                                string query = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereStr);

                                cnn.Open();
                                MethodInfo method = cnn.GetType().GetMethod("ExecuteQueryNotRelationship")
                                .MakeGenericMethod(new Type[] { itemType });
                                props[i].SetValue(obj, method.Invoke(cnn, new object[] { query }));
                                cnn.Close();
                            }
                        }
                    }

                }
            }

        }

        protected override void MapToOne<T>(SqlServerConnection cnn, DataRow dr, T obj)
        {
            throw new NotImplementedException();
        }
    }
}
