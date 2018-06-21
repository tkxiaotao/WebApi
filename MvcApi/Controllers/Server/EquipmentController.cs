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
    /// 设备信息
    /// </summary>
    [AuthFilterOutside]
    public class EquipmentController : ApiController
    {
        //log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器
        YLApiEntities db = new YLApiEntities();


        #region ------创建设备------
        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Create(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string appId = json.appId;
            string token = json.token;
            string clientId = json.clientId;
            string type = json.type;
            string model = json.model;
            string name = json.name;
            string sn = json.sn;
            string code = json.code;
            try
            {
                Equipment data = new Equipment();
                data.appId = appId;
                data.clientId = clientId;
                data.token = token;
                data.type = type;
                data.model = model;
                data.name = name;
                data.sn = sn;
                data.code = code;

                db.Equipment.Add(data);
                db.SaveChanges();


                obj.status = "0";
                obj.msg = "ok";
                obj.data = "设备入库";
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "设备入库！", "Create", appId, "Equipment");
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

        #region ------修改设备------
        /// <summary>
        /// 修改设备
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Change(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string appId = json.appId;
            string token = json.token;
            string clientId = json.clientId;
            string type = json.type;
            string model = json.model;
            string name = json.name;
            string sn = json.sn;
            string code = json.code;
            try
            {
                var data = db.Equipment.Where(d => d.clientId == clientId && d.token == token && d.sn == sn).SingleOrDefault();
                if (data!=null)
                {
                    data.appId = appId;
                    data.clientId = clientId;
                    data.token = token;
                    data.type = type;
                    data.model = model;
                    data.name = name;
                    data.sn = sn;
                    data.code = code;

                    db.SaveChanges();


                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = "修改设备信息";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "修改设备信息！", "Change", appId, "Equipment");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "no";
                    obj.data = "失败";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "修改设备信息！", "Change", appId, "Equipment");
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

        #region ------设备列表------
        /// <summary>
        /// 设备列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage List(string appId, string token, string clientId, string type)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string appId = json.appId;
            //string token = json.token;
            //string clientId = json.clientId;
            //string type = json.type;

            try
            {
                var data = db.Equipment.Where(d => d.token == token && d.clientId == d.clientId && d.type == type).SingleOrDefault();
                if (data != null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(data);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "设备列表！", "List", appId, "Equipment");
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

        #region ------设备详情------
        /// <summary>
        /// 设备详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Detail(string appId, string token, int Id)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string appId = json.appId;
            //string token = json.token;
            //int Id = Convert.ToInt32(json.Id);
            try
            {
                var data = db.Equipment.Where(d => d.token == token && d.Id == Id).SingleOrDefault();
                if (data != null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(data);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "设备详情！", "Detail", appId, "Equipment");
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

        #region ------删除设备------
        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Delete(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            int Id = Convert.ToInt32(json.Id);
            string appId = json.appId;
            string token = json.token;
            try
            {
                var data = db.Equipment.Where(d => d.Id == Id && d.token == token).SingleOrDefault();
                if (data != null)
                {
                    db.Equipment.Remove(data);
                    db.SaveChanges();

                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = "删除设备";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "删除设备！", "Delete", appId, "Equipment");
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
