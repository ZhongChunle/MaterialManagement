using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialManagement.Entity
{
    /// <summary>
    /// 业务返回的数据
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 业务返回状态
        /// </summary>
        public Boolean flag { set; get; }
        /// <summary>
        /// 返回的状态信息
        /// </summary>
        public String message { set; get; }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object data { set; get; }
    }
}