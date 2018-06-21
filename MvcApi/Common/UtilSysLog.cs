using log4net;
using MvcApi.YLDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace MvcApi.Common
{
    public class UtilSysLog
    {
        private static log4net.ILog loginfo = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器
        //private static ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static ILog log;
        public UtilSysLog()
        {
            log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        public UtilSysLog(Type type)
        {
            log = log4net.LogManager.GetLogger(type);
        }


        /// <summary>
        ///  记录错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void LogErr(Exception ex)
        {

            log.Error("error", ex);
        }

        public void LogErr_(Exception ex)
        {

            log.Error("error", ex);
        }

        /// <summary>
        /// 记录严重错误
        /// </summary>
        /// <param name="ex"></param>
        public static void LogFat(Exception ex)
        {
            log.Fatal("fatal", ex);

        }

        public void LogFat_(Exception ex)
        {
            log.Fatal("fatal", ex);

        }

        /// <summary>
        /// 记录一般信息
        /// </summary>
        /// <param name="msg"></param>
        public static void LogInf(string msg)
        {
            log.Info(msg);
        }

        public void LogInf_(string msg)
        {
            log.Info(msg);
        }


        /// <summary>
        /// 记录调试信息  
        /// </summary>
        /// <param name="msg"></param>

        public static void LogDebug(string msg)
        {
            log.Debug(msg);
        }

        public void LogDebug_(string msg)
        {
            log.Debug(msg);
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        /// <param name="msg"></param>
        public static void LogWarn(string msg)
        {
            log.Warn(msg);
        }

        public void LogWarn_(string msg)
        {
            log.Warn(msg);
        }


        
        public static void SetAppIdlog(string appid,string controller,string action)
        {
            YLApiEntities db = new YLApiEntities();
            AppIdLog appidlog = new AppIdLog();
            appidlog.appId = appid;
            appidlog.actionName = action;
            appidlog.addTime = DateTime.Now;
            appidlog.controller = controller;
            appidlog.remark = "";
            try
            {
                db.AppIdLog.Add(appidlog);
                db.SaveChanges();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// 记录正常日志信息
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="msg">信息</param>
        /// <param name="action">方法</param>
        /// <param name="appId">appid</param>
        /// <param name="controller">控制器</param>
        public static void NewLogInfo(string time,string msg,string action,string appId,string controller)
        {
            loginfo.Info("时间：" + time + "--信息：" + msg + "！" + "--方法=" + action + " " + "--appId=" + appId);
            SetAppIdlog(appId, controller, action);
        }


        /// <summary>
        /// 记录异常日志信息
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="ex">异常信息</param>
        public static void NewLogErre(string time,string ex)
        {
            loginfo.Error("时间：" + time + "异常信息：" + ex);
        }


    }
}