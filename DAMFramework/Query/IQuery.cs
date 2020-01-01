using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM_ORMFramework.Query
{
    public interface IQuery
    {
        List<T> ExecuteQuery<T>() where T : new();
        int ExecuteNonQuery();
        List<T> ExecuteQueryNotRelationship<T>() where T : new();
    }
}
