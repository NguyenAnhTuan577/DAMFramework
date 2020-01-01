using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public interface CompletedQuery<T> where T : new()
    {
        List<T> Execute();
    }
}
