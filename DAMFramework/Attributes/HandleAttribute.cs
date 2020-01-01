using DAM_ORMFramework.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMFramework.Attributes
{
    public class HandleAttribute
    {
        public static string GetTable<T>() where T : new()
        {
            var attributes = typeof(T).GetCustomAttributes(typeof(Table), true);
            var tableAttribute = FirstOrDefault(attributes, typeof(Table)) as Table;
            if (tableAttribute != null)
                return tableAttribute.Name;
            return string.Empty;
        }

        public static List<PrimaryKey> GetPrimaryKeys<T>() where T : new()
        {
            List<PrimaryKey> primaryKeys = new List<PrimaryKey>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var primaryKeyAttribute = FirstOrDefault(attributes, typeof(PrimaryKey));
                if (primaryKeyAttribute != null)
                    primaryKeys.Add(primaryKeyAttribute as PrimaryKey);
            }

            if (primaryKeys.Count > 0)
                return primaryKeys;
            else
                return null;
        }

        public static List<ForeignKey> GetForeignKeys<T>(string referID) where T : new()
        {
            List<ForeignKey> foreignKeys = new List<ForeignKey>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var foreignKeyAttribute = FirstOrDefault(attributes, typeof(ForeignKey));
                if (foreignKeyAttribute != null && (foreignKeyAttribute as ForeignKey).ReferID == referID)
                    foreignKeys.Add(foreignKeyAttribute as ForeignKey);
            }

            if (foreignKeys.Count > 0)
                return foreignKeys;
            else
                return null;
        }

        public static List<Column> GetColumns<T>() where T : new()
        {
            List<Column> columnAttributes = new List<Column>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var columnMapping = FirstOrDefault(attributes, typeof(Column));

                if (columnMapping != null)
                {
                    var mapsTo = columnMapping as Column;
                    columnAttributes.Add(mapsTo);
                }
            }

            if (columnAttributes.Count > 0)
                return columnAttributes;
            else
                return null;
        }

        public static Dictionary<Column, object> GetColumnValues<T>(T obj)
        {
            Dictionary<Column, object> listColumnValues = new Dictionary<Column, object>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var columnMapping = FirstOrDefault(attributes, typeof(Column));

                if (columnMapping != null)
                {
                    var mapsTo = columnMapping as Column;
                    listColumnValues.Add(mapsTo, property.GetValue(obj, null));
                }
            }

            if (listColumnValues.Count > 0)
                return listColumnValues;
            else
                return null;
        }

        public static Column FindColumn(string name, Dictionary<Column, object> listColumValues)
        {
            foreach (Column column in listColumValues.Keys)
                if (column.Name == name)
                    return column;
            return null;
        }

        public static Column FindColumn(string name, List<Column> listColumAttributes)
        {
            foreach (Column column in listColumAttributes)
                if (column.Name == name)
                    return column;
            return null;
        }

        public static object FirstOrDefault(object[] attributes, Type type)
        {
            foreach (var a in attributes)
            {
                if (a.GetType() == type)
                    return a;
            }
            return null;
        }

        public static object[] GetAll(object[] attributes, Type type)
        {
            object[] objArray = new object[0];
            foreach (var a in attributes)
            {
                if (a.GetType() == type)
                {
                    Array.Resize(ref objArray, objArray.Length + 1);
                    objArray[objArray.Length - 1] = a;
                }
            }
            return objArray;
        }

        public static object GetFirst(IEnumerable source)
        {
            IEnumerator iter = source.GetEnumerator();

            if (iter.MoveNext())
            {
                return iter.Current;
            }
            return null;
        }
    }
}
