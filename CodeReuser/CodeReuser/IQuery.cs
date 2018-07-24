using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReuser
{
    interface IQuery
    {
        CodeSearchResponse RunTextQuery(SearchItem item);

        CodeSearchResponse RunCodeElementQuery(SearchItem item);
    }
}
