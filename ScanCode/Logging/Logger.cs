using log4net;
using log4net.Config;
using ScanCode.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ScanCode.Logging
{
    /// <summary>
    /// Logger
    /// </summary>
    public class Logger
    {
        private static bool isInitialized = false;
        private static ILog log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appDataPath"></param>
        public static void Initialize(string appDataPath)
        {
            // set the log folder path to a global property
            GlobalContext.Properties[Constants.LogPathName] = appDataPath;

            string curPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configPath = Path.Combine(curPath, Constants.LogConfigFile);
            if (!File.Exists(configPath)) { return; }
            XmlConfigurator.Configure(new FileInfo(configPath));

            log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            isInitialized = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Fatal(object type, string message, Exception ex)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Fatal(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void Fatal(object type, string message)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Fatal(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Error(object type, string message, Exception ex)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Error(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void Error(object type, string message)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Error(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Warn(object type, string message, Exception ex)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Warn(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void Warn(object type, string message)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Warn(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Debug(object type, string message, Exception ex)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Debug(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void Debug(object type, string message)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Debug(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void Info(object type, string message, Exception ex)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Info(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message), ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void Info(object type, string message)
        {
            if (!isInitialized)
                return;

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            log.Info(string.Format("{0} {1} - {2}", type.GetType().FullName, methodBase.Name, message));
        }

        public static void SetLogProperties(string key, string value)
        {
            GlobalContext.Properties[key] = value;

            string logFoler = GlobalContext.Properties["LogFilePath"].ToString();
            Initialize(logFoler);
        }
    }
}
