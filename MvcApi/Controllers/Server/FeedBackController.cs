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
    /// 反馈
    /// </summary>
    public class FeedBackController : ApiController
    {
        YLApiEntities db = new YLApiEntities();
        /// <summary>
        /// 反馈列表
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage List(string appId,string token,string username)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string appId = json.appId;
            //string token = json.token;
            //string username = json.username;
            try
            {
                var data = db.FeedBack.Where(d => d.token == token && d.username == username).SingleOrDefault();
                if (data!=null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(data);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "反馈列表！", "List", appId, "FeedBack");
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


        /// <summary>
        /// 反馈详情
        /// </summary>
        /// <param name="jData"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Detail(string appId,string token,int Id)
        {
            ReturnResult obj = new ReturnResult();
            //dynamic json = jData;
            //string appId = json.appId;
            //string token = json.token;
            //int Id = Convert.ToInt32(json.Id);
            try
            {
                var data = db.FeedBack.Where(d => d.token == token && d.Id == Id).SingleOrDefault();
                if (data!=null)
                {
                    obj.status = "0";
                    obj.msg = "ok";
                    obj.data = JsonHelp.ObjectToString(data);
                    UtilSysLog.NewLogInfo(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "反馈详情！", "Detail", appId, "FeedBack");
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
    }
}
