using MaterialManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MaterialManagement.Entity
{
    /// <summary>
    /// 修改回填数据的实体类
    /// </summary>
    public class EntityClass : matter_table
    {
        public string manufacturer_name { get; set; }
        public string franchiser_name { get; set; }
        public string product_type_name { get; set; }
        public string the_winning_type_name { get; set; }
    }
}