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
    
    public partial class YLMDBScoreInfo
    {
        public int ID { get; set; }
        public int Score { get; set; }
        public int Type { get; set; }
        public int MallID { get; set; }
        public int CheckTaskID { get; set; }
        public int CheckPeronID { get; set; }
        public Nullable<int> ParentPeronID { get; set; }
        public int ParentCheckID { get; set; }
        public int CheckProjectID { get; set; }
        public string Message { get; set; }
        public string PicUrl { get; set; }
        public System.DateTime CreateTime { get; set; }
    }
}
