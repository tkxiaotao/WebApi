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

namespace MvcApi.Controllers.Server
{
    public class DistrictController : ApiController
    {
        YLApiEntities db = new YLApiEntities();

        #region ------区域创建------
        /// <summary>
        /// 区域创建
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
            string parent = json.parent;
            string name = json.name;
            string shortname = json.shortname;
            string level = json.level;
            string type = json.type;

            District model = new District();
            model.appId = appId;
            model.token = token;
            model.parent = parent;
            model.name = name;
            model.shortname = shortname;
            model.level = level;
            model.type = type;
            model.createDate = DateTime.Now;
            model.description = "";


            try
            {
                db.District.Add(model);
                db.SaveChanges();

                obj.status = "0";
                obj.msg = "ok";
                obj.data = model.Id.ToString();
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "区域创建！", "Create", appId, "District");
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

        #region ------区域修改------
        /// <summary>
        /// 区域修改
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
            string parent = json.parent;
            string name = json.name;
            string shortname = json.shortname;
            string level = json.level;
            string type = json.type;

            try
            {
                var model = db.District.Where(d => d.token == token && d.name == name && d.type == type).SingleOrDefault();
                if (model != null)
                {
                    model.token = token;
                    model.appId = appId;
                    model.parent = parent;
                    model.shortname = shortname;
                    model.name = name;
                    model.level = level;
                    model.type = type;
                    model.createDate = DateTime.Now;
                    model.description = "";

                    db.SaveChanges();

                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = model.Id.ToString();
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "区域修改！", "Change", appId, "District");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "失败！";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "区域修改!", "Change", appId, "District");
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

        #region ------区域删除------
        /// <summary>
        /// 区域删除
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Delete(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string appId = json.appId;
            string token = json.token;
            int Id = Convert.ToInt32(json.Id);

            try
            {
                var model = db.District.Where(d => d.Id == Id && d.token == token).SingleOrDefault();
                if (model != null)
                {
                    db.District.Remove(model);
                    db.SaveChanges();

                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "区域删除！", "Delete", appId, "District");
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

        #region ------区域列表------
        /// <summary>
        /// 区域列表
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage List(string appId, string token, string parent, string type)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string appId = json.appId;
            //string token = json.token;
            //string parent = json.parent;
            //string type = json.type;
            try
            {
                var model = db.District.Where(d => d.token == token && d.parent == parent && d.type == type).SingleOrDefault();
                if (model != null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(model);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "区域列表！", "List", appId, "District");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "没有数据";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "区域列表！", "List", appId, "District");
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
