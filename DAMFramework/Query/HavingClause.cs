using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public interface HavingClause<T> where T : new()
    {
        GroupByClause<T> Having(string condition);
        List<T> Execute();
    }
}
