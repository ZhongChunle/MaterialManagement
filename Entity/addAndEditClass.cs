using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialManagement.Entity
{
    /// <summary>
    /// 接收添加数据和修改数据的实体类
    /// </summary>
    public class addAndEditClass
    {
        /// <summary>
        /// 物质名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 物质编号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public int productType { set; get; }
        /// <summary>
        /// 中标类型
        /// </summary>
        public int ThewinningType { get; set; }
        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal PurchasePrice { get; set; }
        /// <summary>
        /// 售卖价格
        /// </summary>
        public decimal SellingPrice { get; set; }
        /// <summary>
        /// 生成商
        /// </summary>
        public int manufacturer { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public int franchiser { get; set; }
    }
}