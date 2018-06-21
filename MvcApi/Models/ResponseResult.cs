using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcApi.Models
{
    public class ResponseResult
    {
        public string status { get; set; }
        public string msg { get; set; }
        public string data { get; set; }
        public string Token { get; set; }
    }
}