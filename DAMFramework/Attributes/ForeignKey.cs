using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Attribute
{
    class ForeignKey:System.Attribute
    {
        public string ReferID { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }

        public ForeignKey(string referID, string name, string reference)
        {
            ReferID = referID;
            Name = name;
            Reference = reference;
        }
    }
}
