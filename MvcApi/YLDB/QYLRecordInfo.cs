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
    
    public partial class QYLRecordInfo
    {
        public int ID { get; set; }
        public Nullable<int> GatewayID { get; set; }
        public int UserID { get; set; }
        public string strTime { get; set; }
        public string StrDevSn { get; set; }
        public Nullable<int> PhotoLen { get; set; }
        public string strBase64PhotoData { get; set; }
        public Nullable<int> SignType { get; set; }
        public string KqStatus { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    }
}
