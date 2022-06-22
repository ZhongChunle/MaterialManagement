using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialManagement.Entity
{
    /// <summary>
    /// 查询数据库返回的字段信息
    /// </summary>
    public class selectData
    {
        /// <summary>
        /// 物资id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 物资名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 物资编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal? purchasePrice { get; set; }
        /// <summary>
        /// 售卖价格
        /// </summary>
        public decimal? SellingPrice { get; set; }
        /// <summary>
        /// 生产商名称
        /// </summary>
        public string producer { get; set; }
        /// <summary>
        /// 经销商名称
        /// </summary>
        public string franchiser { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string productType { get; set; }
        /// <summary>
        /// 中标类型
        /// </summary>
        public string TheWinningType { get; set; }
    }
}