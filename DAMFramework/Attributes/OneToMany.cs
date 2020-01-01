using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Attribute
{
    class OneToMany:System.Attribute
    {
        public string ReferID { get; set; }
        public string TableName { get; set; }

        public OneToMany(string referId, string tablename)
        {
            ReferID = referId;
            TableName = tablename;
        }
    }
}
