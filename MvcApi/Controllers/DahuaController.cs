using MvcApi.Common;
using MvcApi.YLDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
//using YL.Util.Log;
//using System.Web.Http.Cors;



namespace MvcApi.Controllers
{
    public class DahuaController : ApiController
    {
        log4net.ILog log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器
        [HttpGet]
        public string GetTest()
        {
            return "123456789";
        }

        [HttpGet]
        public string GetTest2()
        {
            return "sssssssssssssss";
        }

        #region ------接收大华设备传过来的数据------
        /// <summary>
        /// 接收大华设备传过来的数据
        /// </summary>
        /// <param name="key">由雅量提供，专门用于大华客流的固定字符串</param>
        /// <param name="deviceid">设备ID</param>
        /// <param name="data">数据：【时间/进人数/出人数/设备MAC】</param>
        /// <returns></returns>
        [HttpPost]
        public string DahuaData(JObject jData)
        {
            //响应头开放(用于Web前端的XHR2调用)
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
            ReturnResult result = new ReturnResult();
            //DHModel db = new DHModel();
            YLMDBDBEntities db = new YLMDBDBEntities();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dynamic json = jData;
            string key = json.key;//由雅量提供，专门用于大华客流的固定字符串
            string deviceid = json.deviceid;//设备ID
            string data = json.data;//数据：【时间/进人数/出人数/设备MAC】
            string SysKey = JsonHelp.GetValue("key");
            string Sysdeviceid = string.Empty;
            var deviceidLs = db.YLDeviceidData.Where(d => d.Deviceid == deviceid).FirstOrDefault();
            if (deviceidLs != null && deviceidLs.Deviceid != "")
            {
                Sysdeviceid = deviceidLs.Deviceid;
            }
            else
            {
                result.status = "2";
                result.msg = "设备Id或key错误";
                result.data = null;
                log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "信息：设备Id或key错误");

            }
            //GetValue("deviceid");
            //string result = string.Empty;
            if (key.Equals(SysKey) && deviceid.Equals(Sysdeviceid))
            {
                try
                {
                    var list = JsonHelp.Deserialize<DahuaInfo>(data);

                    var newInNum = list.InNum;
                    var newOutNum = list.OutNum;


                    IntelligentDataDH oldlist = null;
                    //查询最后一条数据(最大的DataID)
                    try
                    {
                        var maxId = db.IntelligentDataDH.Where(p=>p.IMEI.Contains(deviceid)).Max(p => p.DataID);
                        oldlist = db.IntelligentDataDH.FirstOrDefault(d => d.DataID == maxId);
                    }
                    catch (Exception)
                    {
                        oldlist = null;
                    }

                    if (oldlist != null)
                    {
                        var oldInNum = oldlist.InNum;
                        var oldOutNum = oldlist.OutNum;

                        newInNum = list.InNum - oldInNum;
                        newOutNum = list.OutNum - oldOutNum;
                    }


                    IntelligentDataDH da = null;
                    da = new YLDB.IntelligentDataDH();
                    da.GatewayID = 9999;
                    da.InNum = list.InNum;
                    da.OutNum = list.OutNum;
                    da.MAC = "";
                    da.IMEI = deviceid;
                    da.Flage = 1;
                    da.DataDateTime = Convert.ToDateTime(list.DataDateTime);
                    db.IntelligentDataDH.Add(da);


                    db.SaveChanges();

                    string strAction = SubStringByDeviceid(deviceid);//得到插入的表名

                    string sql = string.Format("insert into {0} (GatewayID, IMEI, MAC, DataDateTime, InNum, OutNum, Flage) values ({1},'{2}','{3}','{4}',{5},{6},{7})", strAction, 9999, deviceid, "", list.DataDateTime, newInNum, newOutNum, 1);

                    try
                    {
                        int count = DbHelperSQL.ExecuteSql(sql);
                        if (count > 0)
                        {
                            dic.Add("key", key);
                            dic.Add("deviceid", deviceid);
                            dic.Add("InNum", list.InNum + "");
                            dic.Add("OutNum", list.OutNum + "");
                            dic.Add("DataDateTime", list.DataDateTime + "");
                            result.status = "0";
                            result.msg = "ok";
                            result.data = JsonHelp.ObjectToString(dic);
                            log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "信息：成功！" + "key=" + key + "，   deviceid=" + deviceid);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.status = "6";
                        result.msg = "程序异常";
                        result.data = null;
                        log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
                    }

                    


                    //result = ObjectToString(dic);
                    //result = string.Format("result:数据推送成功！[key={0},deviceid={1},InNum={2},OutNum={3},DataDateTime={4}]",key,deviceid,list.InNum,list.OutNum,list.DataDateTime);
                }
                catch (Exception ex)
                {
                    result.status = "6";
                    result.msg = "程序异常";
                    result.data = null;
                    log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
                }

            }
            else
            {
                //result = "设备Id或key错误";
                result.status = "2";
                result.msg = "设备Id或key错误";
                result.data = null;
                log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：设备Id或key错误");
            }
            var strResult = string.Empty;
            try
            {
                strResult = JsonHelp.ObjectToString(result);
            }
            catch (Exception ex)
            {
                result.status = "6";
                result.msg = "程序异常";
                result.data = null;
                log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
            }
            return strResult;

            //[{"datetime":"2018-5-18 12:23 251","in":1,"out":0,"MAC":"00:72:11:60:22:f0"},{"datetime":"2018-5-18 12:20 512","in":0,"out":2,"MAC":"00:72:11:60:22:f0"}]
        }
        
        #endregion

        #region ------List------
        /// <summary>
        /// 接收大华设备传过来的数据
        /// </summary>
        /// <param name="key">由雅量提供，专门用于大华客流的固定字符串</param>
        /// <param name="deviceid">设备ID</param>
        /// <param name="data">数据：【时间/进人数/出人数/设备MAC】</param>
        /// <returns></returns>
        [HttpPost]
        public string DahuaDataList(JObject jData)
        {
            //DHModel db = new DHModel();
            ReturnResult result = new ReturnResult();
            YLMDBDBEntities db = new YLMDBDBEntities();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dynamic json = jData;
            string key = json.key;//由雅量提供，专门用于大华客流的固定字符串
            string deviceid = json.deviceid;//设备ID
            string data = json.data;//数据：【时间/进人数/出人数/设备MAC】
            string SysKey = JsonHelp.GetValue("key");
            string Sysdeviceid = string.Empty;
            var deviceidLs = db.YLDeviceidData.Where(d => d.Deviceid == deviceid).FirstOrDefault();
            if (deviceidLs != null && deviceidLs.Deviceid != "")
            {
                Sysdeviceid = deviceidLs.Deviceid;
            }
            else
            {
                result.status = "6";
                result.msg = "设备Id或key错误";
                result.data = null;
                log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：设备Id或key错误");
            }
            //GetValue("deviceid");
            //string result = string.Empty;
            if (key.Equals(SysKey) && deviceid.Equals(Sysdeviceid))
            {
                var list = JsonHelp.Deserialize<List<DahuaInfo>>(data);

                IntelligentDataDH da = null;

                foreach (var item in list)
                {
                    da = new YLDB.IntelligentDataDH();
                    da.GatewayID = 9999;
                    da.InNum = item.InNum;
                    da.OutNum = item.OutNum;
                    da.MAC = "";
                    da.IMEI = deviceid;
                    da.Flage = 1;
                    da.DataDateTime = Convert.ToDateTime(item.DataDateTime);
                    db.IntelligentDataDH.Add(da);
                }
                try
                {
                    db.SaveChanges();
                    dic.Add("key", key);
                    dic.Add("deviceid", deviceid);
                    dic.Add("InNum", list[0].InNum + "");
                    dic.Add("OutNum", list[0].OutNum + "");
                    dic.Add("DataDateTime", list[0].DataDateTime + "");

                    result.status = "ok";
                    result.msg = "成功";
                    result.data = JsonHelp.ObjectToString(dic);
                    log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "信息：成功！" + "key=" + key + "，   deviceid=" + deviceid);
                    //result = string.Format("result:数据推送成功！[key={0},deviceid={1},InNum={2},OutNum={3},DataDateTime={4}]",key,deviceid,list[0].InNum,list[0].OutNum,list[0].DataDateTime);
                }
                catch (Exception ex)
                {
                    result.status = "6";
                    result.msg = "程序异常";
                    result.data = null;
                    UtilSysLog.LogErr(ex);
                    log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
                }
            }
            else
            {
                result.status = "2";
                result.msg = "设备Id或key错误";
                result.data = null;
                log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：设备Id或key错误");
            }
            var strResult = string.Empty;
            try
            {
                strResult = JsonHelp.ObjectToString(result);
            }
            catch (Exception ex)
            {
                result.status = "6";
                result.msg = "程序异常";
                result.data = null;
                log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
            }
            return strResult;

            //[{"datetime":"2018-5-18 12:23 251","in":1,"out":0,"MAC":"00:72:11:60:22:f0"},{"datetime":"2018-5-18 12:20 512","in":0,"out":2,"MAC":"00:72:11:60:22:f0"}]
        } 
        #endregion




        /// <summary>
        /// 接收大华设备传过来的数据
        /// </summary>
        /// <param name="key">由雅量提供，专门用于大华客流的固定字符串</param>
        /// <param name="deviceid">设备ID</param>
        /// <param name="data">数据：【时间/进人数/出人数/设备MAC】</param>
        /// <returns></returns>
        [HttpPost]
        public string DahuaTest(JObject jData)
        {
            //响应头开放(用于Web前端的XHR2调用)
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
            //DHModel db = new DHModel();
            ReturnResult result = new ReturnResult();
            YLMDBDBEntities db = new YLMDBDBEntities();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dynamic json = jData;
            string key = json.key;//由雅量提供，专门用于大华客流的固定字符串
            string deviceid = json.deviceid;//设备ID
            string data = json.data;//数据：【时间/进人数/出人数/设备MAC】
            string SysKey = JsonHelp.GetValue("key");
            string Sysdeviceid = string.Empty;
            var deviceidLs = db.YLDeviceidData.Where(d => d.Deviceid == deviceid).FirstOrDefault();
            if (deviceidLs != null && deviceidLs.Deviceid != "")
            {
                Sysdeviceid = deviceidLs.Deviceid;
            }
            else
            {
                result.status = "6";
                result.msg = "设备Id或key错误";
                result.data = null;
                log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：设备Id或key错误");
            }
            //GetValue("deviceid");
            //string result = string.Empty;
            if (key.Equals(SysKey) && deviceid.Equals(Sysdeviceid))
            {
                var list = JsonHelp.Deserialize<DahuaInfo>(data);

                IntelligentData da = null;

                da = new YLDB.IntelligentData();
                da.GatewayID = 9999;
                da.InNum = list.InNum;
                da.OutNum = list.OutNum;
                da.MAC = "";
                da.IMEI = deviceid;
                da.Flage = 1;
                da.DataDateTime = Convert.ToDateTime(list.DataDateTime);
                db.IntelligentData.Add(da);
                try
                {
                    db.SaveChanges();
                    dic.Add("key", key);
                    dic.Add("deviceid", deviceid);
                    dic.Add("InNum", list.InNum + "");
                    dic.Add("OutNum", list.OutNum + "");
                    dic.Add("DataDateTime", list.DataDateTime + "");

                    result.status = "ok";
                    result.msg = "成功";
                    result.data = JsonHelp.ObjectToString(dic);
                    log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "信息：成功！" + "key=" + key + "，   deviceid=" + deviceid);
                    //result = string.Format("result:数据推送成功！[key={0},deviceid={1},InNum={2},OutNum={3},DataDateTime={4}]",key,deviceid,list[0].InNum,list[0].OutNum,list[0].DataDateTime);
                }
                catch (Exception ex)
                {
                    result.status = "6";
                    result.msg = "程序异常";
                    result.data = null;
                    UtilSysLog.LogErr(ex);
                    log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
                }
            }
            else
            {
                result.status = "2";
                result.msg = "设备Id或key错误";
                result.data = null;
                log.Info("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：设备Id或key错误");
            }
            var strResult = string.Empty;
            try
            {
                strResult = JsonHelp.ObjectToString(result);
            }
            catch (Exception ex)
            {
                result.status = "6";
                result.msg = "程序异常";
                result.data = null;
                log.Error("时间：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "异常信息：" + ex.ToString());
            }
            return strResult;

            //[{"datetime":"2018-5-18 12:23 251","in":1,"out":0,"MAC":"00:72:11:60:22:f0"},{"datetime":"2018-5-18 12:20 512","in":0,"out":2,"MAC":"00:72:11:60:22:f0"}]
        }

        #region ------截取设备ID最后一位确定插入那个表------
        /// <summary>
        /// 截取设备ID最后一位确定插入那个表
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        public string SubStringByDeviceid(string deviceid)
        {
            string action = string.Empty;
            string values = "0123456789";
            try
            {
                string number = deviceid.Substring(deviceid.Length - 1);
                if (values.Contains(number))
                {
                    action = "IntelligentData" + number;
                }
                else
                {
                    action = "IntelligentData0";
                }
            }
            catch (Exception)
            {
                action = "IntelligentData0";
            }


            return action;
        } 
        #endregion


        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="Deviceid"></param>
        /// <param name="DeviceidName"></param>
        /// <returns></returns>
        public HttpResponseMessage GetDeviceidData(string Deviceid, string DeviceidName)
        {
            ReturnResult obj = new ReturnResult();

            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }


        public HttpResponseMessage DeviceidUpdate()
        {
            ReturnResult obj = new ReturnResult();

            var resultObj = JsonConvert.SerializeObject(obj);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(resultObj, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }
    }



    public class DahuaInfo
    {
        

        public string DataDateTime { get; set; }

        public int InNum { get; set; }

        public int OutNum { get; set; }
    }

}
