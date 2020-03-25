using System;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class DipInputManager : ManagerBase, IDipInputManager
    {
        private readonly IDipInputService _dipInputService;
        private readonly IApiResourceManager _resourceManager;
        private readonly IPolicyManager _policyManager;
        private readonly ILoginManager _loginManager;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="dipInputService"></param>
        /// <param name="resourceManager"></param>
        /// <param name="policyManager"></param>
        /// <param name="loginManager"></param>
        public DipInputManager(
            IDipInputService dipInputService,
            IApiResourceManager resourceManager,
            IPolicyManager policyManager,
            ILoginManager loginManager)
        {
            _dipInputService = dipInputService;
            _resourceManager = resourceManager;
            _policyManager = policyManager;
            _loginManager = loginManager;
        }

        /// <summary>
        /// Get Dip Input Values
        /// </summary>
        /// <returns>List of dip inputs</returns>
        public List<DipInput> GetDipInputValues()
        {
            List<DipInput> dipInputs = _dipInputService.GetDipInputValues();

            return dipInputs;
        }

        /// <summary>
        /// Save Dip Inputs
        /// </summary>
        /// <param name="dipInputs">Dip inputs</param>
        /// <param name="error">Error</param>
        /// <returns>Lis of dip imputs</returns>
        public List<DipInput> SaveDipInputs(List<DipInput> dipInputs, out ErrorMessage error)
        {
            error = new ErrorMessage();
            foreach (var dipInput in dipInputs)
            {
                byte tankId;
                byte gradeId;
                double dipvalue;
                byte.TryParse(dipInput.TankId, out tankId);
                double.TryParse(dipInput.DipValue, out dipvalue);
                byte.TryParse(dipInput.GradeId, out gradeId);
                if (!IsValidDipInput(tankId, gradeId, dipvalue, out error))
                {
                    return null;
                }
            }

            if (CheckEmptyInput(dipInputs))
            {
                return _dipInputService.GetDipInputValues();
            }

            _dipInputService.SaveDipInputs(dipInputs);

            return _dipInputService.GetDipInputValues();
        }

        /// <summary>
        /// Get Report of the Dip input values for print
        /// </summary>
        /// <param name="tillNumber">Till number</param>
        /// <param name="shiftNumber">Shift number</param>
        /// <param name="registerNumber">Register number</param>
        /// <param name="userCode">User code</param>
        /// <param name="error">Error message</param>
        /// <returns>Dip input report</returns>
        public Report PrintDipReport(int tillNumber, int shiftNumber, int registerNumber, string userCode, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var user = _loginManager.GetExistingUser(userCode);
            var report = new Report { ReportName = Utilities.Constants.TankDipFile, Copies = 1 };
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            string timeFormatHm = string.Empty;
            string timeFormatHms = string.Empty;
            GetTimeFormats(ref timeFormatHm, ref timeFormatHms);
            var store = _policyManager.LoadStoreInfo();

            var strDate = DateAndTime.Year(DateAndTime.Today) + Strings.Right("00" + Convert.ToString(DateAndTime.Month(DateAndTime.Today)), 2) + Strings.Right("00" + Convert.ToString(DateAndTime.Day(DateAndTime.Today)), 2);
            var strFileName = Path.GetTempPath() + "\\Dip" + $"{DateTime.Now:yyyy-MM-dd_hh-mm-ss-tt}.txt";
            var dipInputs = _dipInputService.GetDipInputsForReport(strDate);
            if (dipInputs.Count > 0)
            {
                short sWidth = 20;
                short aWidth = 20;
                short hWidth = 40;


                var just = Strings.Left(Convert.ToString(_policyManager.REC_JUSTIFY), 1).ToUpper();

                var nH = FileSystem.FreeFile();
                FileSystem.FileOpen(nH, strFileName, OpenMode.Output, OpenAccess.Write);

                //  - store code printing should be based on policy - Gasking will enter store code as part of store name - so they don't want to see store code in the beginning
                //   If Policy.PRN_CO_NAME Then Print #nH, PadIt(Just, Store.Code & "  " & Store.Name, H_Width)

                if (_policyManager.PRN_CO_NAME)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, (_policyManager.PRN_CO_CODE ? store.Code + "  " : "") + store.Name, hWidth));
                }

                if (_policyManager.PRN_CO_ADDR)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street1), hWidth));

                    if (store.Address.Street2 != "")
                    {
                        FileSystem.PrintLine(nH, modStringPad.PadIt(just, Convert.ToString(store.Address.Street2), hWidth));
                    }
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.Address.City)) + ", " + store.Address.ProvState, hWidth) + "\r\n" + modStringPad.PadIt(just, Convert.ToString(store.Address.PostalCode), hWidth));
                }

                if (_policyManager.PRN_CO_PHONE)
                {
                    foreach (Phone tempLoopVarPhoneRenamed in store.Address.Phones)
                    {
                        var phoneRenamed = tempLoopVarPhoneRenamed;
                        if (phoneRenamed.Number.Trim() != "")
                        {
                            FileSystem.PrintLine(nH, modStringPad.PadC(phoneRenamed.PhoneName + " " + phoneRenamed.Number, hWidth));
                        }
                    }
                }

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.RegName)) + " " + store.RegNum, hWidth)); //& vbCrLf

                if (store.SecRegName != "")
                {
                    FileSystem.PrintLine(nH, modStringPad.PadIt(just, Strings.Trim(Convert.ToString(store.SecRegName)) + " " + store.SecRegNum, hWidth));
                }
                FileSystem.PrintLine(nH);

                FileSystem.PrintLine(nH,
                    _policyManager.PRN_UName
                        ? modStringPad.PadIt(just,
                            _resourceManager.GetResString(offSet, 225) + ": " + user.Name + " (" +
                            Strings.Left(_resourceManager.GetResString(offSet, 132), 1) + Convert.ToString(registerNumber) + "/" +
                            Strings.Left(_resourceManager.GetResString(offSet, 131), 1) + Convert.ToString(tillNumber) + "/" +
                            Strings.Left(_resourceManager.GetResString(offSet, 346), 1) + Convert.ToString(shiftNumber) + ")",
                            hWidth)
                        : modStringPad.PadIt(just,
                            _resourceManager.GetResString(offSet, 225) + ": " + user.Code + " (" +
                            Strings.Left(_resourceManager.GetResString(offSet, 132), 1) + Convert.ToString(registerNumber) + "/" +
                            Strings.Left(_resourceManager.GetResString(offSet, 131), 1) + Convert.ToString(tillNumber) + "/" +
                            Strings.Left(_resourceManager.GetResString(offSet, 346), 1) + Convert.ToString(shiftNumber) + ")",
                            hWidth));
                // 

                FileSystem.PrintLine(nH, modStringPad.PadIt(just, DateAndTime.Today.ToString("dd-MMM-yyyy") + _resourceManager.GetResString(offSet, 208) + DateAndTime.TimeOfDay.ToString(timeFormatHm), hWidth) + "\r\n"); //" at " '  
                FileSystem.PrintLine(nH, modStringPad.PadIt(just, _resourceManager.GetResString(offSet, 3403) + " " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss"), hWidth)); //"DIP READING @ "
                FileSystem.PrintLine(nH, modStringPad.PadL("=", hWidth, "=") + "\r\n");
                FileSystem.PrintLine(nH, modStringPad.PadC(_resourceManager.GetResString(offSet, 3401), sWidth) + modStringPad.PadC(_resourceManager.GetResString(offSet, 3402), aWidth)); //"Tank ID","DIP Value"
                FileSystem.PrintLine(nH, modStringPad.PadL("-", hWidth, "-")); //& vbCrLf

                foreach (DipInput dipInput in dipInputs)
                {
                    FileSystem.PrintLine(nH, modStringPad.PadC(dipInput.TankId, sWidth) + modStringPad.PadC(dipInput.DipValue, aWidth));
                }

                // Advance the specified number of lines.
                short i;
                for (i = 1; i <= _policyManager.ADV_LINES; i++)
                {
                    FileSystem.PrintLine(nH);
                }
                FileSystem.FileClose(nH);
                var stream = File.OpenRead(strFileName);
                report.ReportContent = Helper.CreateBytes(stream);
                report.Copies = 1;
                //modPrint.Dump_To_Printer((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath + "\\" + File_Name, 1, true, true, false);
            }
            else
            {
                
                //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.Critical + MsgBoxStyle.OkOnly;
                //Chaps_Main.DisplayMessage(this, 60, temp_VbStyle, null, 0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 34, 60, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            return report;
        }

        #region Private methods

        /// <summary>
        /// Checks whether Dip Input is Valid 
        /// </summary>
        /// <param name="tankNo">Tank number</param>
        /// <param name="gradeId">Grade id</param>
        /// <param name="dipInputValue">Dip input value</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        private bool IsValidDipInput(byte tankNo, byte gradeId, double dipInputValue, out ErrorMessage error)
        {
            error = new ErrorMessage();

            if (!_dipInputService.IsTankExists(tankNo, gradeId))
            {
                error = new ErrorMessage
                {
                    MessageStyle = new MessageStyle
                    {
                        MessageType = MessageType.OkOnly,
                        Message = "Invalid TankId or Grade Id"
                    },
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            var maxdip = _dipInputService.MaximumDip(tankNo);
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            if (dipInputValue > maxdip)
            {
                
                //Chaps_Main.DisplayMessage(this, (short)61, temp_VbStyle, maxdip, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 34, 61, maxdip, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }
            if (dipInputValue <= 0)
            {
                
                //Chaps_Main.DisplayMessage(this, (short)62, temp_VbStyle2, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, 34, 62, null, CriticalOkMessageType),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks Empty Dip input
        /// </summary>
        /// <param name="dipInputs"></param>
        /// <returns></returns>
        private bool CheckEmptyInput(List<DipInput> dipInputs)
        {
            var emptyInput = true;
            foreach (DipInput dipInput in dipInputs)
            {
                if (!string.IsNullOrEmpty(dipInput.DipValue))
                {
                    emptyInput = false;
                    break;
                }
            }
            if (emptyInput)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to get time formats
        /// </summary>
        /// <param name="timeFormatHm">Hour minute</param>
        /// <param name="timeFormatHms">Hour minute second </param>
        private void GetTimeFormats(ref string timeFormatHm, ref string timeFormatHms)
        {
            if (_policyManager.TIMEFORMAT == "24 HOURS")
            {
                timeFormatHm = "hh:mm";
                timeFormatHms = "hh:mm:ss";
            }
            else
            {
                timeFormatHm = "hh:mm tt";
                timeFormatHms = "hh:mm:ss tt";
            }

        }

        #endregion
    }
}
