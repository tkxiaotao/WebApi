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
    
    public partial class Logs
    {
        public int ID { get; set; }
        public string Operator { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
        public Nullable<int> Style { get; set; }
        public Nullable<int> FormClient { get; set; }
        public string Ip { get; set; }
        public string IpAddress { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
