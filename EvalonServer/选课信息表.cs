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
    
    public partial class 选课信息表
    {
        public long 选课编号 { get; set; }
        public string 学号 { get; set; }
        public string 课程号 { get; set; }
        public int 课程成绩 { get; set; }
    
        public virtual 课程信息表 课程信息表 { get; set; }
        public virtual 学生信息表 学生信息表 { get; set; }
    }
}
