using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Attribute
{
    public class Column:System.Attribute
    {
        public string Name { get; set; }
        public DataType Type { get; set; }

        public Column(string name, DataType type)
        {
            Name = name;
            Type = type;
        }
    }
}
