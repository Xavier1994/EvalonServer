//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EvalonServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class 学生信息表
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public 学生信息表()
        {
            this.选课信息表 = new HashSet<选课信息表>();
        }
    
        public string 学号 { get; set; }
        public string 姓名 { get; set; }
        public string 性别 { get; set; }
        public Nullable<int> 年龄 { get; set; }
        public Nullable<int> 系号 { get; set; }
        public string 籍贯 { get; set; }
        public string 民族 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<选课信息表> 选课信息表 { get; set; }
    }
}
