//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MvcApi.YLDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public int ID { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public int ShopID { get; set; }
        public int ParentID { get; set; }
        public System.DateTime CreateTime { get; set; }
        public string ParentID1 { get; set; }
        public string NickName { get; set; }
    }
}