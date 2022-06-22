using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialManagement.Entity
{
    /// <summary>
    /// 模糊分页查询条件
    /// </summary>
    public class queryData
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int currentPage { get; set; }
        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 物质名称模糊查询
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 物质编号模糊查询
        /// </summary>
        public string code { get; set; }
    }
}