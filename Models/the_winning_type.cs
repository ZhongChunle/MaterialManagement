//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MaterialManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class the_winning_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public the_winning_type()
        {
            this.matter_table = new HashSet<matter_table>();
        }
    
        public int the_winning_type_id { get; set; }
        public string the_winning_type_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<matter_table> matter_table { get; set; }
    }
}
