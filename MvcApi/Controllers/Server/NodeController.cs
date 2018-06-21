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
    /// <summary>
    /// 节点信息
    /// </summary>
    public class NodeController : ApiController
    {
        YLApiEntities db = new YLApiEntities();

        #region ------节点创建------
        /// <summary>
        /// 节点创建
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Create(JObject jData)
        {
            NodeList model = new NodeList();
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            model.appId = json.appId;
            model.contacts = json.contacts;
            model.createDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            model.description = "";
            model.name = json.name;
            model.parent = json.parent;
            model.phone = json.phone;
            model.token = json.token;
            model.type = json.type;

            try
            {
                db.NodeList.Add(model);
                db.SaveChanges();

                obj.status = "0";
                obj.msg = "ok";
                obj.data = model.Id.ToString();
                UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点创建！", "Create", json.appId, "Node");
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

        #region ------节点修改------
        /// <summary>
        /// 节点修改
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
            string type = json.type;
            string contacts = json.contacts;
            string phone = json.phone;

            try
            {
                var model = db.NodeList.Where(d => d.token == token && d.name == name && d.type == type).SingleOrDefault();
                if (model != null)
                {
                    model.appId = json.appId;
                    model.contacts = json.contacts;
                    model.createDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    model.description = "";
                    model.name = json.name;
                    model.parent = json.parent;
                    model.phone = json.phone;
                    model.token = json.token;
                    model.type = json.type;

                    db.SaveChanges();

                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = "";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点修改！", "Change", json.appId, "Node");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "no";
                    obj.data = "修改失败";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点修改！", "Change", json.appId, "Node");
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

        #region ------节点删除------
        /// <summary>
        /// 节点删除
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
                var data = db.NodeList.Where(d => d.Id == Id && d.token == token).SingleOrDefault();
                if (data != null)
                {
                    db.NodeList.Remove(data);
                    db.SaveChanges();

                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = "删除设备";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点删除！", "Delete", appId, "Node");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "no";
                    obj.data = "没有数据";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点删除！", "Delete", appId, "Node");
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

        #region ------节点列表------
        /// <summary>
        /// 节点列表
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage List(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string appId = json.appId;
            string token = json.token;
            string parent = json.parent;
            string type = json.tyep;
            try
            {
                var data = db.NodeList.Where(d => d.token == token && d.parent == parent && d.type == type).SingleOrDefault();
                if (data != null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(data);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点列表！", "List", appId, "Node");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "no";
                    obj.data = "没有数据";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点列表！", "List", appId, "Node");
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

        #region ------节点详情------
        /// <summary>
        /// 节点详情
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Detail(JObject jData)
        {
            ReturnResult obj = new ReturnResult();
            dynamic json = jData;
            string appId = json.appId;
            string token = json.token;
            string parent = json.parent;
            int Id = Convert.ToInt32(json.Id);
            try
            {
                var data = db.NodeList.Where(d => d.token == token && d.parent == parent && d.Id==Id).SingleOrDefault();
                if (data != null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(data);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点详情！", "Detail", appId, "Node");
                }
                else
                {
                    obj.status = "2";
                    obj.msg = "no";
                    obj.data = "没有数据";
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "节点详情！", "Detail", appId, "Node");
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
