using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApi.Common
{
    /// <summary>
    /// =====状态0===============================================================
    ///0. 成功
    ///1. 缺少参数
    ///2. 参数错误
    ///3. Token无效
    ///4. Secret无效（AppKey无效）
    ///5. 权限不足
    ///6. 程序处理异常
    ///7. 指定服务不存在
    ///8. 服务器错误
    ///100. 错误，请参考msg
    /// </summary>
    public class ReturnResult
    {
        public string status { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
    }
}