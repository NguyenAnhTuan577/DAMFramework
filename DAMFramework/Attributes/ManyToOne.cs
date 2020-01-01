using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Attribute
{
    public class ManyToOne:System.Attribute
    {
        public string ReferID { get; set; }
        public string TableName { get; set; }

        public ManyToOne(string referId, string tablename)
        {
            ReferID = referId;
            TableName = tablename;
        }
    }
}
