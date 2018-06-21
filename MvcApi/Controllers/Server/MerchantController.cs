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
    /// 商户信息
    /// </summary>
    [AuthFilterOutside]
    public class MerchantController : ApiController
    {
        //log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器
        YLApiEntities db = new YLApiEntities();

        #region ------创建商户------
        /// <summary>
        /// 创建商户
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Create(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string token = json.token;
            string appId = json.appId;
            string clientId = json.clientId;
            string name = json.name;
            string status = json.status;
            string module = json.module;
            string contacts = json.contacts;
            string phone = json.phone;
            string email = json.email;
            string address = json.address;
            Merchant model = new Merchant();
            model.address = address;
            model.clientId = clientId;
            model.token = token;
            model.name = name;
            model.status = status;
            model.module = module;
            model.email = email;
            model.phone = phone;
            model.appId = appId;
            model.contacts = contacts;
            model.createDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            try
            {
                db.Merchant.Add(model);
                db.SaveChanges();
                obj.status = "0";
                obj.msg = "成功";
                obj.data = model.Id.ToString();
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "创建商户", "Create", appId, "Merchant");
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }
            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        } 
        #endregion

        #region ------修改商户------
        /// <summary>
        /// 修改商户
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Change(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string token = json.token;
            string appId = json.appId;
            string clientId = json.clientId;
            string name = json.name;
            string status = json.status;
            string module = json.module;
            string contacts = json.contacts;
            string phone = json.phone;
            string email = json.email;
            string address = json.address;

            try
            {
                Merchant model = db.Merchant.Where(d => d.token == token).Where(d => d.phone == phone).Where(d => d.name == name).SingleOrDefault();
                model.address = address;
                model.clientId = clientId;
                model.token = token;
                model.name = name;
                model.status = status;
                model.module = module;
                model.email = email;
                model.phone = phone;
                model.appId = appId;
                model.contacts = contacts;

                db.SaveChanges();
                obj.status = "0";
                obj.msg = "成功";
                obj.data = JsonHelp.ObjectToString(model);
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "商户修改", "Change", appId, "Merchant");
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }

            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        } 
        #endregion

        #region ------商户列表------
        /// <summary>
        /// 商户列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage List(string token, string appId, string status)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string token = json.token;
            //string appId = json.appId;
            //string status = json.status;
            Merchant model = new Merchant();
            try
            {
                model = db.Merchant.Where(d => d.token == token && d.status == status).SingleOrDefault();
                if (model != null)
                {
                    obj.status = "0";
                    obj.msg = "成功";
                    obj.data = JsonHelp.ObjectToString(model);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "商户列表信息", "List", appId, "Merchant");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "失败！";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "商户列表信息", "List", appId, "Merchant");
                }
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
                obj.data = "";
                UtilSysLog.NewLogErre(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), ex.ToString());
            }


            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;

        } 
        #endregion

        #region ------商户详情------
        /// <summary>
        /// 商户详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Detail(string token, string appId, int Id)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string token = json.token;
            //string appId = json.appId;
            //int Id = Convert.ToInt32(json.Id);
            Merchant model = new Merchant();
            try
            {
                model = db.Merchant.Where(d => d.token == token && d.Id == Id).SingleOrDefault();
                if (model != null)
                {
                    obj.status = "0";
                    obj.msg = "成功";
                    obj.data = JsonHelp.ObjectToString(model);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "商户详细", "Detail", appId, "Merchant");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "失败";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "商户详细", "Detail", appId, "Merchant");
                }
            }
            catch (Exception ex)
            {
                obj.status = "6";
                obj.msg = "失败";
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
