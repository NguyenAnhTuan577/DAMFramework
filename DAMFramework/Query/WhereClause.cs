using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public interface WhereClause<T> where T : new()
    {
        HavingClause<T> Where(string condition);
        HavingClause<T> Rows();
    }
}
