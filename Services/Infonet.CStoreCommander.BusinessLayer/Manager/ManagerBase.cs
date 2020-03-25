using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Logging;
using Infonet.CStoreCommander.Resources;
using log4net;
using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Web;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public abstract class ManagerBase
    {
        protected readonly ILog Performancelog = LoggerManager.PerformanceLogger;

        // protected readonly ILog CustLogger = LoggerManager.CustLogger;



        protected const MessageType ExclamationOkMessageType = (int)MessageType.Exclamation + MessageType.OkOnly;

        protected const MessageType CriticalOkMessageType = (int)MessageType.Critical + MessageType.OkOnly;

        protected const MessageType InformationOkMessageType = (int)MessageType.Information + MessageType.OkOnly;

        protected const MessageType YesNoQuestionMessageType = (int)MessageType.YesNo + MessageType.Question;

        protected const MessageType OkMessageType = MessageType.OkOnly;

        protected const MessageType QuestionOkMessageType = (int)MessageType.Question + MessageType.OkOnly;

        public short WriteToLogFileFl = 0;

        public int PosId
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                var token = request.Headers["authToken"];
                return TokenValidator.GetPosId(token);
            }
        }

        public string UserCode
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                var token = request.Headers["authToken"];
                return TokenValidator.GetUserCode(token);
            }
        }

        /// <summary>
        /// Method to write to log file
        /// </summary>
        /// <param name="msgStr"></param>
        protected void WriteToLogFile(string msgStr)
        {
            try
            {
                var register = CacheManager.GetRegister(PosId);

                if (register.WritePosLog)
                {
                    var logPath = @"C:\APILog\";
                    var fileName = logPath + "PosLog_" + PosId + ".txt";

                    WriteToLogFileFl++;
                    if (WriteToLogFileFl > 100)
                    {
                        WriteToLogFileFl = (short)0;

                        if (FileSystem.Dir(fileName) != "")
                        {
                            if (FileSystem.FileLen(fileName) > 1000000)
                            {
                                var newFName = logPath + "PosLog_" + PosId + DateAndTime.Day(DateAndTime.Today).ToString("00") + DateAndTime.Hour(DateAndTime.TimeOfDay).ToString("00") + ".txt";

                                Variables.CopyFile(fileName, newFName, 0);
                                Variables.DeleteFile(fileName);
                            }
                        }
                    }

                    using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                    {
                        fileWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Strings.Space(3) + msgStr);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Method to write to log file
        /// </summary>
        /// <param name="msgStr"></param>
        public void WriteUDPData(string msgStr)
        {
            try
            {
                var logPath = @"C:\APILog\";
                var fileName = logPath + "PosLog_" + DateTime.Today.ToString("MM/dd/yyyy") + ".txt";

                using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                {
                    fileWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + Strings.Space(3) + msgStr);
                }
            }
            catch (Exception ex)
            {

            }
}

/// <summary>
/// Report content
/// </summary>
/// <param name="fs">File stream</param>
/// <returns>Content</returns>
public string GetReportContent(FileStream fs)
{
    var bytes = new byte[fs.Length];
    fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
    fs.Close();
    var content = Convert.ToBase64String(bytes);
    return content;
}


public static bool WriteLog11(string strFileName, string strMessage)
{
    try
    {
        FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", Path.GetTempPath(), strFileName), FileMode.Append, FileAccess.Write);
        StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
        objStreamWriter.WriteLine(strMessage);
        objStreamWriter.Close();
        objFilestream.Close();
        return true;
    }
    catch (Exception ex)
    {
        return false;
    }
}


        /// <summary>
        /// Method to write to log file
        /// </summary>
        /// <param name="msgStr"></param>
        public void WriteTokickBackLogFile(string msgStr)
        {
            try
            {
                var register = CacheManager.GetRegister(PosId);

                if (register.WritePosLog)
                {
                    var logPath = @"C:\APILog\";
                    var fileName = logPath + "KickBackLog_" + PosId + ".txt";
                    WriteToLogFileFl++;
                    if (WriteToLogFileFl > 100)
                    {
                        WriteToLogFileFl = (short)0;

                        if (FileSystem.Dir(fileName) != "")
                        {
                            if (FileSystem.FileLen(fileName) > 1000000)
                            {
                                var newFName = logPath + "KickBackLog_" + PosId+ DateAndTime.Day(DateAndTime.Today).ToString("00") + DateAndTime.Hour(DateAndTime.TimeOfDay).ToString("00") + ".txt";

                                Variables.CopyFile(fileName, newFName, 0);
                                Variables.DeleteFile(fileName);
                            }
                        }
                    }

                    using (StreamWriter fileWriter = new StreamWriter(fileName, true))
                    {
                        fileWriter.WriteLine(DateTime.Now.ToString("dd-MMM-yy hh:mm:ss tt") + msgStr);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
