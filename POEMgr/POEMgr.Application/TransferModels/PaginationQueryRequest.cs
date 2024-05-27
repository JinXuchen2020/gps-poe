using POEMgr.Repository.CommonQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application.TransferModels
{
    public class PaginationQueryRequest
    {
        public List<ColumnQueryParamRequest> columnQueryParams { get; set; }

        public int? pageIndex { get; set; } = 1;

        public int? pageSize { get; set; } = 9999;
    }

    public class ColumnQueryParamRequest
    {
        public string columnName { get; set; }
        public string value { get; set; }
    }    
}
