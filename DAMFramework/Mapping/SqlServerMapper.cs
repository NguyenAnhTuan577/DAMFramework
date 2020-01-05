using DAM_ORMFramework.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Mapping
{
    public class SqlServerMapper : Mapper
    {
        protected override void MapOneToMany<T>(AbstractSqlConnection cnn, DataRow dr, T obj)
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
                            SqlServerMapper sqlServerMapper = new SqlServerMapper();
                            Type typeArguments = type.GetGenericArguments()[0];
                            MethodInfo getTableMethod = sqlServerMapper
                                                        .GetType()
                                                        .GetMethod("GetTable")
                                                        .MakeGenericMethod(new Type[] { typeArguments });
                            string tbName = getTableMethod.Invoke(sqlServerMapper, null) as string;


                            MethodInfo getColumnMethod = sqlServerMapper
                                                        .GetType()
                                                        .GetMethod("GetColumns")
                                                        .MakeGenericMethod(typeof(T));
                            List<Column> columnAttributes = getColumnMethod.Invoke(sqlServerMapper, null) as List<Column>;


                            MethodInfo getFKMethod = sqlServerMapper
                                                    .GetType()
                                                    .GetMethod("GetFK")
                                                    .MakeGenericMethod(new Type[] { typeArguments });
                            OneToMany attribute = (OneToMany)relationshipAtrr[j];
                            List<ForeignKey> fkAttributes = getFKMethod.Invoke(sqlServerMapper, new object[] { attribute.ReferID }) as List<ForeignKey>;


                            string whereClause = string.Empty;
                            if (fkAttributes !=null)
                            {
                                foreach (ForeignKey fk in fkAttributes)
                                {
                                    Column column = GetColumn(fk.ReferID, columnAttributes);
                                    if (column!=null)
                                    {
                                        string format = "{0} = {1} and ";
                                        if (column.Type == DataType.NCHAR || column.Type == DataType.NVARCHAR)
                                            format = "{0} = N'{1}' and ";
                                        else if (column.Type == DataType.CHAR || column.Type == DataType.VARCHAR)
                                            format = "{0} = '{1}' and ";

                                        whereClause += string.Format(format, fk.Name, dr[fk.ReferID]);
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(whereClause))
                            {
                                cnn.Open();
                                int whereLength = whereClause.Length;
                                whereClause = whereClause.Substring(0, whereLength - 4);
                                string query = string.Format("SELECT * FROM {0} WHERE {1}", tbName, whereClause);
                                MethodInfo method = cnn
                                                    .GetType()
                                                    .GetMethod("ExecuteQueryNotRelationship")
                                                    .MakeGenericMethod(new Type[] { typeArguments });
                                props[i].SetValue(obj, method.Invoke(cnn, new object[] { query }));
                                cnn.Close();
                            }
                        }
                    }

                }
            }

        }

        protected override void MapToOne<T>(AbstractSqlConnection cnn, DataRow dr, T obj)
        {
            var props = typeof(T).GetProperties();
            for(int i=0;i<props.Length; i++)
            {
                var attributes = props[i].GetCustomAttributes(false);
                Type propType = props[i].PropertyType;

                var oneToOneArr = getAllAttributes(attributes, typeof(OneToOne));
                var manyToOneArr = getAllAttributes(attributes, typeof(ManyToOne));

                var toOneAttr = new object[oneToOneArr.Length + manyToOneArr.Length];
                if (toOneAttr.Length > 0)
                {
                    oneToOneArr.CopyTo(toOneAttr, 0);
                    manyToOneArr.CopyTo(toOneAttr, oneToOneArr.Length);
                }

                if (toOneAttr != null && toOneAttr.Length > 0)
                {
                    for(int j=0; j< toOneAttr.Length; j++)
                    {
                        SqlServerMapper sqlServerMapper = new SqlServerMapper();
                        string where = string.Empty;
                        string table = string.Empty;
                        string referID = string.Empty;

                        if (toOneAttr[j].GetType() == typeof(ManyToOne))
                        {

                            referID = (toOneAttr[j] as ManyToOne).ReferID;
                            table = (toOneAttr[j] as ManyToOne).TableName;
                        }
                        else
                        {
                            referID = (toOneAttr[j] as OneToOne).ReferID;
                            table = (toOneAttr[j] as OneToOne).TableName;
                        }

                        MethodInfo getColumnsMethod = sqlServerMapper
                              .GetType()
                              .GetMethod("GetColumns")
                              .MakeGenericMethod(new Type[] { propType });

                        List<Column> columnAttributes = getColumnsMethod.Invoke(sqlServerMapper, null) as List<Column>;


                        MethodInfo getFKMethod = sqlServerMapper
                                                .GetType()
                                                .GetMethod("GetFK")
                                                .MakeGenericMethod(typeof(T));
                        List<ForeignKey> fkAttributes = getFKMethod.Invoke(sqlServerMapper, new object[] { referID }) as List<ForeignKey>;


                        if (fkAttributes != null)
                        {
                            foreach (ForeignKey fk in fkAttributes)
                            {
                                Column column = GetColumn(fk.Reference, columnAttributes);
                                if (column != null)
                                {
                                    string format = "{0} = {1} and ";
                                    if (column.Type == DataType.NCHAR || column.Type == DataType.NVARCHAR)
                                        format = "{0} = N'{1}' and ";
                                    else if (column.Type == DataType.CHAR || column.Type == DataType.VARCHAR)
                                        format = "{0} = '{1}' and ";

                                    where += string.Format(format, fk.Reference, dr[fk.Name]);
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(where))
                        {

                            where = where.Substring(0, where.Length - 4);
                            string query = string.Format("SELECT * FROM {0} WHERE {1}", table, where);
                            cnn.Open();
                            MethodInfo executeMethod = cnn
                                                .GetType()
                                                .GetMethod("ExecuteQueryNotRelationship")
                                                .MakeGenericMethod(new Type[] { propType });
                            var ienumerable = (IEnumerable)executeMethod.Invoke(cnn, new object[] { query });
                            cnn.Close();

                            MethodInfo getFirstMethod = sqlServerMapper
                                                 .GetType()
                                                 .GetMethod("GetFirst");
                            var first = getFirstMethod.Invoke(sqlServerMapper, new object[] { ienumerable });

                            props[i].SetValue(obj, Convert.ChangeType(first, props[i].PropertyType) );
                        }

                    }

                }
            }

        }
    }
}
