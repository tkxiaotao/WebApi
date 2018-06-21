using MvcApi.YLDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MvcApi.Controllers
{
    public class AuthFilterOutside : AuthorizeAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            //url获取token  
            var content = actionContext.Request.Properties["MS_HttpContext"] as HttpContextBase;
            var token = content.Request["token"];
            var appId = content.Request["appid"];
            if (!string.IsNullOrEmpty(token))
            {
                //解密用户ticket,并校验用户名密码是否匹配  
                if (ValidateTicket(token,appId))
                {
                    base.IsAuthorized(actionContext);
                }
                else
                {
                    HandleUnauthorizedRequest(actionContext);
                }
            }
            //如果取不到身份验证信息，并且不允许匿名访问，则返回未验证401  
            else
            {
                var attributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().OfType<AllowAnonymousAttribute>();
                bool isAnonymous = attributes.Any(a => a is AllowAnonymousAttribute);
                if (isAnonymous) base.OnAuthorization(actionContext);
                else HandleUnauthorizedRequest(actionContext);
            }
        }


        //校验票据（数据库数据匹配）  
        private bool ValidateTicket(string encryptToken,string appId)
        {
            YLApiEntities db = new YLApiEntities();
            bool flag = false;
            try
            {
                //获取数据库Token  
                var model = db.Account.Where(d=>d.token==encryptToken).SingleOrDefault();
                if (model!=null && model.token == encryptToken) //存在  
                {
                    //未超时  
                    flag = (DateTime.Now <= model.expireDate) ? true : false;
                }
            }
            catch (Exception ex) { }
            return flag;
        }
    }
}