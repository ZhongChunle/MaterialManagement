using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialManagement.Entity
{
    /// <summary>
    /// 分页查询返回的实体
    /// </summary>
    public class PageResult
    {
        public long total { get; set; }
        public List<selectData> rows { get; set; }
    }
}