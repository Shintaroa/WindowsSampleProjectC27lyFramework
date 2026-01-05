using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

//日志文件
namespace  TFramework
{
    public class LogFile : MonoSingleton<LogFile>
    {

        public void Init()
        {
            logType = SystemConfig.Instance.logType;
            Active();
        }

        private void OnApplicationQuit()
        {
            Release();
        }

        private void Active()
        {
            CreateFile();
            Application.logMessageReceived += new Application.LogCallback(this.LogHandler);
        }
        
        private void Release()
        {
            streamWrite_log?.Close();
            fileStream_log?.Close();
            Application.logMessageReceived -= new Application.LogCallback(this.LogHandler);
        }

        private void CreateFile()
        {
            dirPath = VitalPath.CurrentDirectory + "/Log";
            
            if (!Directory.Exists(this.dirPath))
            {
                Directory.CreateDirectory(this.dirPath);
            }
            
            string text = this.dirPath + "/LOG--" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            
            Debug.Log("日志文件路径：" + text);
            
            fileStream_log = new FileStream(text, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            streamWrite_log = new StreamWriter(this.fileStream_log);
            
            CheckLogFileNum(this.dirPath, 20);
        }

        private void LogHandler(string condition, string stackTrace, LogType type)
        {
            if (this.logType == LOG_TYPE.None) { return; }
            if (this.logType == LOG_TYPE.Warning && !(type.GetHashCode() >= LogType.Warning.GetHashCode())){ return; }
            if (this.logType == LOG_TYPE.Assert && !(type.GetHashCode() >= LogType.Assert.GetHashCode())){ return; }
            if (this.logType == LOG_TYPE.Error && !(type.GetHashCode() >= LogType.Error.GetHashCode())){ return; }
            
            logQueue.Enqueue(new LogItem
            {
                messageString = condition,
                stackTrace = stackTrace,
                logType = type,
                time = DateTime.Now
            });
        }
        
        private void FixedUpdate()
        {
            if (logQueue.Count > 0)
            {
                try
                {
                    LogItem logItem = logQueue.Dequeue();
                    string text = logItem.time.ToString("HH:mm:ss.ff");
                    string text2 = string.Format("{0}-[{1}] >> {2} <======> {3}", new object[]
                    {
                        text,
                        logItem.logType,
                        logItem.messageString,
                        logItem.stackTrace
                    });
                    this.streamWrite_log.WriteLine(text2);
                    this.streamWrite_log.Flush();
                }
                catch (IOException ex)
                {
                    Debug.Log(ex.Message);
                }
            }
        }
        
        private void CheckLogFileNum(string path, int maxNum)
        {
            List<string> list = Enumerable.ToList<string>(Directory.GetFiles(path));
            int num = list.Count - maxNum;
            if (num > 0)
            {
                for (int i = 0; i < num; i++)
                {
                    File.Delete(list[i]);
                }
            }
        }
        
        public enum LOG_TYPE
        {
            All = 0,						//全部
            Warning,						//Warning以上
            Assert,							//Assert以上
            Error,							//Error以上
            None							//不输出
        }
        
        private FileStream fileStream_log;
        private StreamWriter streamWrite_log;
        private Queue<LogItem> logQueue = new Queue<LogItem>();
        private LOG_TYPE logType = LOG_TYPE.None;
        private string dirPath = "";
    }
}