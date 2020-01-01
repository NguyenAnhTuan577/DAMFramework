using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Attribute
{
    class PrimaryKey:System.Attribute
    {
        public string Name { get; set; }
        public bool GenerateID { get; set; }
        
        public PrimaryKey(string name, string generateid)
        {
            Name = name;
            GenerateID = generateid;
        }
    }
}
