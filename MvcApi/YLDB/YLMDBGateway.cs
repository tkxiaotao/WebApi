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
    
    public partial class YLMDBGateway
    {
        public int ID { get; set; }
        public Nullable<int> MallID { get; set; }
        public string GatewayCode { get; set; }
        public string GatewayName { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
    
        public virtual YLMDBGateway YLMDBGateway1 { get; set; }
        public virtual YLMDBGateway YLMDBGateway2 { get; set; }
    }
}
