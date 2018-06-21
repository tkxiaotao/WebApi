using MvcApi.Common;
using MvcApi.Models;
using MvcApi.YLDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MvcApi.Controllers
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    [AuthFilterOutside]
    public class AccountController : ApiController
    {
        log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器
        YLApiEntities db = new YLApiEntities();

        #region ------用户登录授权------
        /// <summary>  
        /// 用户登录授权  
        /// </summary>  
        /// <param name="username">用户名</param>  
        /// <param name="password">密码</param>  
        /// <returns></returns>  
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage Login(JObject jData)
        {
            //定义  
            ResponseResult obj = new ResponseResult();
            dynamic json = jData;
            string username=json.phone;
            string password = json.password;
            string appId = json.appId;
            var model = GetLoginModel(username, password);
            if (model != null)
            {
                
                if (ValidateTicket(model.token))
                {
                    obj.Token = model.token;
                }
                else
                {
                    var token = Guid.NewGuid().ToString("N");
                    model.token = token;
                    model.expireDate = DateTime.Now.AddDays(7);
                    obj.Token = token;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
                    }
                }
                obj.status = "0";
                obj.msg = "ok";
                obj.data = JsonHelp.ObjectToString(model);
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "用户登录", "Login", appId, "Account");
            }
            else
            {
                obj.status = "2";
                obj.msg = "用户名或密码错误";
                obj.data = "";
                obj.Token = "";
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "用户名或密码错误", "Login", appId, "Account");
            }
            var resultObj = JsonConvert.SerializeObject(obj, Formatting.Indented);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        #endregion

        #region ------重新获取token------
        /// <summary>
        /// 重新获取token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetToken(string user, string pwd, string appId)
        {
            ResponseResult obj = new ResponseResult();
            try
            {
                var model = GetLoginModel(user, pwd);
                if (model != null && model.token != "")
                {
                    string token = Guid.NewGuid().ToString("N");
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = model.ToString();
                    obj.Token = token;

                    model.token = token;
                    db.SaveChanges();
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "获取token", "GetToken", appId, "Account");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "用户名或密码错误";
                    obj.data = "";
                    obj.Token = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "重新获取失败(用户名或密码错误)！", "GetToken", appId, "Account");
                }

            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.Token = "";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }


            var resultObj = JsonConvert.SerializeObject(obj, Formatting.Indented);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        
        #endregion

        #region ------查询Token是否有效------
        /// <summary>  
        /// 查询Token是否有效  
        /// </summary>  
        /// <param name="token">token</param>  
        /// <returns></returns>  
        [HttpGet]
        public HttpResponseMessage ValidateToken(string token)
        {
            //定义  
            ResponseResult obj = new ResponseResult();
            bool flag = ValidateTicket(token);
            if (flag)
            {
                //返回信息              
                obj.status = "0";
                obj.msg = "token有效";
                obj.Token = token;
                obj.data = "";
            }
            else
            {
                obj.status = "0";
                obj.msg = "token无效";
                obj.Token = token;
                obj.data = "";
            }
            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        #endregion  

        #region ------验证票据是否有效------
        /// <summary>  
        /// 验证票据是否有效  
        /// </summary>  
        /// <param name="encryptToken">token</param>  
        /// <returns></returns>  
        private bool ValidateTicket(string encryptToken)
        {
            bool flag = false;
            try
            {
                //获取数据库Token  
                var model = db.Account.Where(d => d.token == encryptToken).SingleOrDefault();
                if (model != null && model.token == encryptToken) //存在  
                {
                    //未超时  
                    flag = (DateTime.Now <= model.expireDate) ? true : false;
                }
            }
            catch (Exception ex) { flag = false; }
            return flag;
        }
        #endregion  

        #region ------用户登录------
        /// <summary>  
        /// 用户登录  
        /// </summary>  
        /// <param name="userName">用户名</param>  
        /// <param name="userPwd">密码</param>  
        /// <returns></returns>  
        private Account GetLoginModel(string userName, string userPwd)
        {
            Account model = new Account();
            string md5Pwd = EncryptHelper.MD5Encrypt32(userPwd);
            try
            {
                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(md5Pwd))
                {
                    //数据库比对  
                    model = db.Account.Where(d => d.phone == userName && d.Password == md5Pwd).SingleOrDefault();
                }
            }
            catch (Exception ex) { }
            return model;
        }
        #endregion

        #region ------注册------
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage Register([FromBody]Account users)
        {
            ResponseResult obj = new ResponseResult();
            Account model = new Account();
            string token = Guid.NewGuid().ToString("N");
            model.name = users.name;
            string password = users.Password;
            model.Password = EncryptHelper.MD5Encrypt32(password);
            model.phone = users.phone;
            model.status = 1;
            model.token = token;
            model.expireDate = DateTime.Now.AddDays(1);
            model.email = users.email;
            model.createDate = DateTime.Now.ToString();
            model.description = users.description;
            model.appId = users.appId;
            db.Account.Add(model);
            try
            {
                db.SaveChanges();
                obj.status = "0";
                obj.msg = "OK";
                obj.Token = token;
                obj.data = model.ToString();
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "注册", "Register", users.appId, "Account");
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.Token = "";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }

            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
        #endregion

        #region ------修改密码------
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="newpwd">新密码</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UserModifyPwd(JObject jData)
        {
            ResponseResult obj = new ResponseResult();
            dynamic json = jData;
            string user = json.phone;
            string oldpwd = json.oldpwd;
            string newpwd = json.newpwd;
            string appId = json.appId;
            try
            {
                oldpwd = EncryptHelper.MD5Encrypt32(oldpwd);
                newpwd = EncryptHelper.MD5Encrypt32(newpwd);
                var users = db.Account.Where(d => d.phone == user && d.Password == oldpwd).SingleOrDefault();
                if (users != null)
                {
                    users.Password = newpwd;
                    db.SaveChanges();
                    obj.status = "0";
                    obj.Token = "";
                    obj.msg = "修改成功";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "修改密码", "UserModifyPwd", appId, "Account");
                }
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.Token = "";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }


            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        } 
        #endregion

        #region ------退出登录------
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage LogOut(JObject jData)
        {
            ResponseResult obj = new ResponseResult();
            dynamic json = jData;
            string user = json.phone;
            string pwd = json.pwd;
            string appId = json.appId;
            try
            {
                var model = GetLoginModel(user, pwd);
                if (model != null)
                {
                    model.token = "";//删除token
                    db.SaveChanges();
                    obj.status = "0";
                    obj.Token = "";
                    obj.msg = "退出成功";
                    obj.data = "";

                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "退出登录", "LogOut", appId, "Account");
                }
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.Token = "";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }

            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;

        } 
        #endregion

        #region ------获取用户信息------
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetAccountInfo(string user, string pwd, string appId,string token)
        {
            ResponseResult obj = new ResponseResult();
            //dynamic json = jData;
            //string user = json.phone;
            //string pwd = json.pwd;
            //string appId = json.appId;
            try
            {
                var model = GetLoginModel(user, pwd);
                if (model != null)
                {
                    obj.status = "0";
                    obj.Token = model.token;
                    obj.msg = "成功";
                    obj.data = JsonHelp.ObjectToString(model);

                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "获取用户信息", "GetAccountInfo", appId, "Account");
                }
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.Token = "";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }
            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        } 
        #endregion
    }
}
