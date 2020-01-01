using DAM_ORMFramework.Attribute;
using DAMFramework.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Mapping
{
    abstract class Mapper
    {
        protected abstract void MapOneToMany<T>(SqlServerConnection cnn, DataRow dr, T obj) where T : new();
        protected abstract void MapToOne<T>(SqlServerConnection cnn, DataRow dr, T obj) where T : new();

        public T MapRelationship<T>(SqlServerConnection cnn, DataRow dr) where T : new()
        {
            T obj = new T();
            var props = typeof(T).GetProperties();

            int length = props.Length;
            for(int i=0; i < length; i++)
            {
                var attributes = props[i].GetCustomAttributes(false);
                var columnMapping = getFirstAttribute(attributes, typeof(T)); 

                if (columnMapping != null)
                {
                    var mapsTo = columnMapping as Column;
                    props[i].SetValue(obj, dr[mapsTo.Name]);
                }

            }

            MapOneToMany(cnn, dr, obj);
            MapToOne(cnn, dr, obj);

            return obj;
        }

        public T MapNotRelationship<T>(SqlServerConnection cnn, DataRow dr) where T : new()
        {
            T obj = new T();
            var props = typeof(T).GetProperties();
            int length = props.Length;

            for(int i = 0; i < length; i++)
            {
                var attr = props[i].GetCustomAttributes(false);
                var columnMapping = getFirstAttribute(attr, typeof(Column));
                if (columnMapping != null)
                {
                    var mapsTo = columnMapping as Column;
                    props[i].SetValue(obj, dr[mapsTo.Name]);
                }
            }

            return obj;
        }

        public string GetTable<T>() where T : new()
        {
            var attributes = typeof(T).GetCustomAttributes(typeof(Table), true);
            var attr = getFirstAttribute(attributes, typeof(Table)) as Table;
            if (attr != null)
                return attr.Name;
            return string.Empty;
        }

        public List<PrimaryKey> GetPK<T>() where T : new()
        {
            List<PrimaryKey> pks = new List<PrimaryKey>();

            var props = typeof(T).GetProperties();
            for(int i = 0; i < props.Length; i++)
            {
                var attr = props[i].GetCustomAttributes(false);
                var pk = getFirstAttribute(attr, typeof(PrimaryKey));
                if (pk != null)
                    pks.Add(pk as PrimaryKey);
            }

            if (pks.Count > 0)
                return pks;
            else
                return null;
        }

        public List<ForeignKey> GetFK<T>(string refID) where T : new()
        {
            List<ForeignKey> fks = new List<ForeignKey>();

            var props = typeof(T).GetProperties();
            int length = props.Length;
            for(int i=0;i<length; i++)
            {
                var attr = props[i].GetCustomAttributes(false);
                var fk = getFirstAttribute(attr, typeof(ForeignKey)); ;
                if (fk && (fk as ForeignKey).ReferID == refID)
                    fks.Add(fk as ForeignKey);
            }


            if (fks.Count > 0)
                return fks;
            else
                return null;
        }

        public  List<Column> GetColumns<T>() where T : new()
        {
            List<Column> attrs = new List<Column>();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var attributes = prop.GetCustomAttributes(false);
                var columnMapping = getFirstAttribute(attributes, typeof(Column));

                if (columnMapping != null)
                {
                    var mapsTo = columnMapping as Column;
                    attrs.Add(mapsTo);
                }
            }

            if (attrs.Count > 0)
                return attrs;
            else
                return null;
        }

        public  Dictionary<Column, object> GetValuesOfColumn<T>(T obj)
        {
            Dictionary<Column, object> values = new Dictionary<Column, object>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);
                var firstAttr =  getFirstAttribute(attributes, typeof(Column));

                if (firstAttr != null)
                {
                    var mapsTo = firstAttr as Column;
                    values.Add(mapsTo, property.GetValue(obj, null));
                }
            }

            if (values.Count > 0)
                return values;
            else
                return null;
        }

        public  Column GetColumn(string columnName, Dictionary<Column, object> listColum)
        {
           
            foreach (Column column in listColum.Keys)
                if (column.Name == columnName)
                    return column;
            return null;
        }

        public  Column GetColumn(string name, List<Column> columAttributes)
        {
            foreach (Column column in columAttributes)
                if (column.Name == name)
                    return column;
            return null;
        }

        protected object getFirstAttribute(object[] attributes, Type type)
        {
            int lenght = attributes.Length;
            for(int i=0;i< lenght; i++)
            {
                if (attributes[i].GetType() == type)
                    return attributes[i];
            }

            return null;
        }

        protected object[] getAllAttributes(object[] attributes, Type type)
        {
            List<object> attrs = new List<object>();
            int length = attributes.Length;
            for(int i=0; i < length; i++)
            {
                if(attributes[i]== type)
                {
                    attrs.Add(attributes[i]);
                }
            }

            object[] objArray = attrs.ToArray();

            return objArray;
        }

        public object GetFirst(IEnumerable source)
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
