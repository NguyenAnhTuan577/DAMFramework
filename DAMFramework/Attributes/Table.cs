using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Attribute
{
    class Table:System.Attribute
    {
        public string Name { get; set; }

        public Table(string name)
        {
            Name = name;
        }
    }
}
