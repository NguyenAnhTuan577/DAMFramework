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
                var columnMapping = HandleAttribute.FirstOrDefault(attributes, typeof(T)); 

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
                var columnMapping = HandleAttribute.FirstOrDefault(attr, typeof(Column));
                if (columnMapping != null)
                {
                    var mapsTo = columnMapping as Column;
                    props[i].SetValue(obj, dr[mapsTo.Name]);
                }
            }

            return obj;
        }


      
    }
}
