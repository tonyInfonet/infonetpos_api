using System;
using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.Entities;
using Microsoft.VisualBasic;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Resources;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class PrepayManager : ManagerBase, IPrepayManager
    {
        private readonly IPrepayService _prepayService;
        private readonly IApiResourceManager _resourceManager;

        public PrepayManager(IPrepayService prepayService,
            IApiResourceManager resourceManager)
        {
            _prepayService = prepayService;
            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Method to refresh prepay
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="prepayStatus">Prepay status</param>
        /// <returns>True or false</returns>
        public bool RefreshPrepay(short pumpId, short prepayStatus)
        {
            bool returnValue;

            if (prepayStatus != 6)
            {
                if (OldPrepayStatus(pumpId) == prepayStatus)
                {
                    return true;
                }
            }

            if (prepayStatus > 0)
            {
                Variables.Pump[pumpId].IsHoldPrepay = true;
            }
            else
            {
                Variables.Pump[pumpId].IsHoldPrepay = false;
                
                
                
                Variables.Pump[pumpId].IsPrepay = false;
                Variables.Pump[pumpId].PrepayAmount = 0;
                Variables.Pump[pumpId].IsPrepayLocked = false;
                Variables.Pump[pumpId].PrepayInvoiceID = 0;
                Variables.Pump[pumpId].PrepayPosition = 0;


                return true;
            }

            try
            {

                var dRecset = _prepayService.GetPrepayGlobal(pumpId);
                Variables.Pump[pumpId] = dRecset;
                if (dRecset.PrepayAmount == 0)
                {
                    if (prepayStatus > 4)
                    {
                        return false;
                    }
                }
                returnValue = true;
            }
            catch
            {
                WriteToLogFile("RefreshPrepay Failed for Pump: " + Convert.ToString(pumpId) + " PrepayStatus: " + Convert.ToString(prepayStatus) + ". Record in PrepayGlobal maybe is deleted.");
                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Method to get prepay status string
        /// </summary>
        /// <returns></returns>
        public string PrepayStatusString()
        {
            //   IsHoldPrepay   IsPrepay   IsPrepayLocked      PrepayStatus
            //     0             0               0                  000=0
            //     1             0               0                  100=4
            //     1             1               0                  101=6
            //     1             1               1                  111=7
            short i;

            var returnValue = "";
            for (i = 1; i <= Variables.gPumps.PumpsCount; i++)
            {
                var vPrepayStatus = (short)0;
                if (Variables.Pump[i].IsHoldPrepay)
                {
                    vPrepayStatus = 4;
                    if (Variables.Pump[i].IsPrepay)
                    {
                        vPrepayStatus = (short)(vPrepayStatus + 2);
                    }
                    if (Variables.Pump[i].IsPrepayLocked)
                    {
                        vPrepayStatus++;
                    }
                }
                returnValue = returnValue + Convert.ToString(vPrepayStatus);
            }

            return returnValue;
        }

        /// <summary>
        /// Method to set prepayment
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="position">Postion Id</param>
        /// <returns>True or false</returns>
        public void SetPrepayment(int invoiceId, short pumpId, float amount, byte position)
        {
            //reset prepay variables
            Variables.Pump[pumpId].IsPrepay = true;
            Variables.Pump[pumpId].PrepayAmount = amount;
            Variables.Pump[pumpId].IsPrepayLocked = false;
            Variables.Pump[pumpId].PrepayInvoiceID = invoiceId;
            Variables.Pump[pumpId].PrepayPosition = position;
        }

        
        /// <summary>
        /// Method to set prepayment from POS
        /// </summary>
        /// <param name="invoiceId">Invoice Id</param>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="mop">MOP</param>
        /// <param name="positionId">Postion Id</param>
        /// <param name="tillNumber"></param>
        /// <returns>True or false</returns>
        public bool SetPrepaymentFromPos(int invoiceId, short pumpId, float amount, byte mop, byte positionId, int tillNumber)
        {
            if (amount <= 0)
            {
                return false;
            }
            _prepayService.SetPrepaymentFromPos(invoiceId, pumpId, amount, mop, positionId, tillNumber);

            //and reset prepay variables
            Variables.Pump[pumpId].IsPrepay = true;
            Variables.Pump[pumpId].PrepayAmount = amount;
            Variables.Pump[pumpId].IsPrepayLocked = false;
            Variables.Pump[pumpId].PrepayInvoiceID = invoiceId;
            Variables.Pump[pumpId].PrepayPosition = positionId;

            return true;
        }
        

        
        /// <summary>
        /// Method to delete prepay from POS
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True ro false</returns>
        public void DeletePrepaymentFromPos(short pumpId)
        {
            _prepayService.DeletePrepaymentFromPos(pumpId);
            Variables.RecsAffected = 1;

            //and reset prepay variables
            Variables.Pump[pumpId].IsHoldPrepay = false;
            Variables.Pump[pumpId].IsPrepay = false;
            Variables.Pump[pumpId].PrepayAmount = 0;
            Variables.Pump[pumpId].IsPrepayLocked = false;
            Variables.Pump[pumpId].PrepayInvoiceID = 0;
        }
        

        /// <summary>
        /// Method to swirch prepay
        /// </summary>
        /// <param name="oldPumpId">Old pump Id</param>
        /// <param name="newPumpId">New pump Id</param>
        /// <returns>True or false</returns>
        public void SwitchPrepayment(short oldPumpId, short newPumpId)
        {
            Variables.Pump[newPumpId].IsHoldPrepay = Variables.Pump[oldPumpId].IsHoldPrepay;
            Variables.Pump[newPumpId].IsPrepay = Variables.Pump[oldPumpId].IsPrepay;
            Variables.Pump[newPumpId].PrepayAmount = Variables.Pump[oldPumpId].PrepayAmount;
            Variables.Pump[newPumpId].IsPrepayLocked = Variables.Pump[oldPumpId].IsPrepayLocked;
            Variables.Pump[newPumpId].PrepayInvoiceID = Variables.Pump[oldPumpId].PrepayInvoiceID;
            Variables.Pump[newPumpId].PrepayPosition = Variables.Pump[oldPumpId].PrepayPosition;
            Variables.Pump[oldPumpId].IsHoldPrepay = false;
            Variables.Pump[oldPumpId].IsPrepay = false;
            Variables.Pump[oldPumpId].PrepayAmount = 0;
            Variables.Pump[oldPumpId].IsPrepayLocked = false;
            Variables.Pump[oldPumpId].PrepayInvoiceID = 0;
            Variables.Pump[oldPumpId].PrepayPosition = 0;
        }

        /// <summary>
        /// Method to delete prepay from POS
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="showMessage">Show message or not</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        public bool DeletePrepayFromFc(short pumpId, bool showMessage, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            if (!Variables.Pump[pumpId].IsPrepay && !Variables.Pump[pumpId].IsHoldPrepay)
            {
                return false;
            }

            if (!TCPAgent.Instance.IsConnected)
            {
                if (showMessage)
                {
                    error = new ErrorMessage()
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 38, 19, null, CriticalOkMessageType)
                    };
                }
                return false;
            }

            var response = "";
            var strRemain = "";
            Variables.IsWaiting = true;

            var timeIn = (float)DateAndTime.Timer;
            string tempCommandRenamed = "Rpr" + Strings.Right("0" + Convert.ToString(pumpId), 2);
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);

            while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Rpr" + Strings.Right("00" + Convert.ToString(pumpId), 2));
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "Rpr" + Strings.Right("00" + Convert.ToString(pumpId), 2), ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify PortReading from Deleting prepay: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIn)
                {
                    timeIn = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 8) == "Rpr" + Strings.Right("00" + Convert.ToString(pumpId), 2) + "ERR")
            {
                if (showMessage)
                {
                    error = new ErrorMessage()
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 38, 19, null, CriticalOkMessageType)
                    };
                }
                Variables.IsWaiting = false;
                return false;
            }

            if (Strings.Left(response, 7) != "Rpr" + Strings.Right("00" + Convert.ToString(pumpId), 2) + "OK") //response is not RprERR or RprOK
            {
                if (showMessage)
                {
                    error = new ErrorMessage()
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet, 38, 19, null, CriticalOkMessageType)
                    };
                }
                string tempCommandRenamed2 = "ENDPOS";
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, false);
                Variables.IsWaiting = false;
                return false;
            }
            Variables.IsWaiting = false;
            return true;
        }

        /// <summary>
        /// Method to hold prepayment
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True or false</returns>
        public void HoldPrepayment(short pumpId)
        {
            //FC will take care of Activating prepay in DB
            Variables.Pump[pumpId].IsHoldPrepay = true;
        }

        /// <summary>
        /// Method to get prepay item Id
        /// </summary>
        /// <param name="sale">Sale</param>
        /// <returns>Prepay item Id</returns>
        public short PrepayItemId(ref Sale sale)
        {
            short i;
            

            var returnValue = (short)0;
            if (sale == null)
            {
                return returnValue;
            }
            if (sale.Sale_Lines.Count < 1)
            {
                return returnValue;
            }

            
            for (i = 1; i <= sale.Sale_Lines.Count; i++)
            {
                if (!sale.Sale_Lines[i].Prepay) continue;
                returnValue = i;
                break;
            }

            return returnValue;
        }

        /// <summary>
        /// Method to get prepay basket
        /// </summary>
        /// <param name="strBasket">Basket</param>
        /// <param name="tillNumber">Till number</param>
        /// <returns>Prepay basket</returns>
        public int IsMyPrepayBasket(string strBasket, int tillNumber)
        {
            var ppInvoice = 0;
            var ppTillId = (short)0;
            var i = (short)(strBasket.IndexOf(".", StringComparison.Ordinal) + 1);
            if (i > 0)
            {
                var j = (short)(strBasket.IndexOf(".", i + 1 - 1, StringComparison.Ordinal) + 1);
                if (j > 0)
                {
                    ppInvoice = (int)(Conversion.Val(strBasket.Substring(i + 1 - 1, j - i - 1)));
                    i = (short)(strBasket.IndexOf(";", j + 1 - 1, StringComparison.Ordinal) + 1);
                    if (i > 0)
                    {
                        ppTillId = (short)(Conversion.Val(strBasket.Substring(j + 1 - 1, i - j - 1)));
                    }
                }
            }
            //var returnValue = ppTillId == tillNumber ? ppInvoice : 0;
            return ppInvoice;
        }

        /// <summary>
        /// Method to set prepay from Fuel control
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="tillNumber">Till number</param>
        /// <param name="saleNumber">Sale number</param>
        /// <param name="prepayPriceType">Prpeay price type</param>
        /// <param name="error">Error</param>
        /// <returns>True or false</returns>
        public bool SetPrepayFromFc(short pumpId, int tillNumber, int saleNumber, string prepayPriceType,
            out ErrorMessage error)
        {
            error = new ErrorMessage();

            if (!TCPAgent.Instance.IsConnected)
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 60, null);
                return false;
            }
            Variables.Pump[pumpId].PrepayInvoiceID = saleNumber;

            var response = "";
            var strRemain = "";
            var tempCommandRenamed = "Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2) + Strings.Right("00000000" + Convert.ToString(Variables.Pump[pumpId].PrepayInvoiceID), 8) + prepayPriceType + (Variables.Pump[pumpId].PrepayAmount * 100).ToString("000000") + Strings.Right("000" + Convert.ToString(tillNumber), 3) + Strings.Right("0" + Convert.ToString(Variables.Pump[pumpId].PrepayPosition), 1);
            //var tcpAgent = new TCPAgent();
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
            var timeIN = (float)DateAndTime.Timer;
            while (!(DateAndTime.Timer - timeIN > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                if (string.IsNullOrEmpty(strBuffer))
                {
                    return false;
                }
                WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2));
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2), ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify TCPAgent.PortReading from set Prepayment: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIN)
                {
                    timeIN = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }

            if (Strings.Left(response, 7) != "Prp" + Strings.Right("0" + Convert.ToString(pumpId), 2) + "OK")
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                error.MessageStyle = _resourceManager.CreateMessage(offSet, 11, 60, null);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to basket undo
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <param name="posId">Pos Id</param>
        /// <param name="gradeId">Grade Id</param>
        /// <param name="amount">Amount</param>
        /// <param name="up">Up</param>
        /// <param name="volume">Volume</param>
        /// <param name="stockCode">Stock code</param>
        /// <param name="mop">Mop</param>
        /// <param name="error">Error message</param>
        /// <returns>True or false</returns>
        public bool BasketUndo(short pumpId, short posId, short gradeId, float amount, float up, float volume,
            string stockCode, byte mop, out ErrorMessage error)
        {
            float basketUndoTimeIn = 0;
            error = new ErrorMessage();
            if (!TCPAgent.Instance.IsConnected)
            {
                var store = CacheManager.GetStoreInfo();
                var offSet = store?.OffSet ?? 0;
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet, (38), 92, null, CriticalOkMessageType)
                };
                return false;
            }

            
            if (DateAndTime.Timer - Variables.basketClick < Variables.basket_click_delay && (DateAndTime.Timer - Variables.basketClick) > 0)
            {
                return false; //3 sec delay
            }

            if ((DateAndTime.Timer - basketUndoTimeIn < 2) && (DateAndTime.Timer - basketUndoTimeIn) > 0)
            {
                return false;
            }

            if (basketUndoTimeIn > DateAndTime.Timer)
            {
                basketUndoTimeIn = 0; //reset on midnight
            }
            else
            {
                basketUndoTimeIn = (float)DateAndTime.Timer;
            }

            if (Variables.basketClick > DateAndTime.Timer)
            {
                Variables.basketClick = 0;
            }
            else
            {
                Variables.basketClick = (float)DateAndTime.Timer;
            }

            var response = "";
            var strRemain = "";
            Variables.IsWaiting = true;
            string tempCommandRenamed = "Ist" + Strings.Right("0" + System.Convert.ToString(pumpId), 2) + System.Convert.ToString(mop) + System.Convert.ToString(posId) + Strings.Right("0000000" + System.Convert.ToString(amount * 1000), 8) + Strings.Right("00000000" + System.Convert.ToString(volume * 1000), 8);
            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);


            var timeIN = (float)DateAndTime.Timer;
            while (!(DateAndTime.Timer - timeIN > Variables.gPumps.CommunicationTimeOut))
            {
                var strBuffer = Convert.ToString(TCPAgent.Instance.NewPortReading);
                modStringPad.WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting Ist" + Strings.Right("00" + System.Convert.ToString(pumpId), 2));
                if (!string.IsNullOrEmpty(strBuffer))
                {
                    modStringPad.SplitResponse(strBuffer, "Ist" + Strings.Right("00" + Convert.ToString(pumpId), 2), ref response, ref strRemain); //strBuffer<>""
                    if (!string.IsNullOrEmpty(response)) //got what we are waiting
                    {
                        TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                        WriteToLogFile("modify PortReading from BasketUndo: " + strRemain);
                        break;
                    }
                }
                if (DateAndTime.Timer < timeIN)
                {
                    timeIN = (float)DateAndTime.Timer;
                }
                Variables.Sleep(100);
            }
            if (Strings.Left(response, 8) == "Ist" + Strings.Right("00" + Convert.ToString(pumpId), 2) + "ERR")
            {
                Variables.IsWaiting = false;
                return false;
            }

            if (Strings.Left(response, 7) != "Ist" + Strings.Right("00" + System.Convert.ToString(pumpId), 2) + "OK") //response is not IstERR or IstOK
            {
                string tempCommandRenamed2 = "ENDPOS";
                TCPAgent.Instance.Send_TCP(ref tempCommandRenamed2, true);
                Variables.IsWaiting = false;
                return false;
            }

            Variables.gBasket[pumpId].AmountCurrent = amount;
            Variables.gBasket[pumpId].VolumeCurrent = volume;
            Variables.gBasket[pumpId].UPCurrent = up;
            Variables.gBasket[pumpId].gradeIDCurr = gradeId;
            Variables.gBasket[pumpId].PosIDCurr = posId;
            Variables.gBasket[pumpId].CurrentFilled = true;
            Variables.gBasket[pumpId].currMOP = mop;
            Variables.gBasket[pumpId].Stock_Code_Cur = stockCode;
            Variables.IsWaiting = false;
            return true;
        }

        #region Private methods

        /// <summary>
        /// Method to verify if old prepay status
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>Prepay status</returns>
        private short OldPrepayStatus(short pumpId)
        {
            //   IsHoldPrepay  IsPrepay   IsPrepayLocked   PrepayStatus
            //     0             0               0                  000=0
            //     1             0               0                  100=4
            //     1             1               0                  101=6
            //     1             1               1                  111=7
            short returnValue = 0;
            if (Variables.Pump[pumpId].IsHoldPrepay)
            {
                returnValue = (short)4;
            }
            if (Variables.Pump[pumpId].IsPrepay)
            {
                returnValue = (short)(returnValue + 2);
            }
            if (Variables.Pump[pumpId].IsPrepayLocked)
            {
                returnValue++;
            }
            return returnValue;
        }


        /// <summary>
        /// Method to complete preapay for pump
        /// </summary>
        /// <param name="pumpId">Pump Id</param>
        /// <returns>True ro false</returns>
        private bool CompletePrePay(short pumpId)
        {
            Variables.Pump[pumpId].IsHoldPrepay = false;
            Variables.Pump[pumpId].IsPrepay = false;
            //Pump(PumpID).IsMyPrepay = False
            Variables.Pump[pumpId].PrepayAmount = 0;
            //Pump(PumpID).IsPrepayActivated = False
            Variables.Pump[pumpId].IsPrepayLocked = false;
            Variables.Pump[pumpId].PrepayInvoiceID = 0;
            Variables.Pump[pumpId].PrepayPosition = 0;
            
            
            
            
            
            return true;
        }

        //gradeID=0 means active prepay for selected pump
        /// <summary>
        /// Method to load prepay
        /// </summary>
        private bool LockPrepay(short pumpId)
        {
            if (!_prepayService.LockPrepay(pumpId)) return false;
            Variables.Pump[pumpId].IsPrepayLocked = true;
            return true;
        }

        /// <summary>
        /// Method to load prepay
        /// </summary>
        private void LoadPrepay()
        {
            short n;

            // load the prepay data from PrepayGlobal table
            for (n = 1; n <= Variables.gPumps.PumpsCount; n++)
            {
                Variables.Pump[n] = _prepayService.LoadPrepay(n);
            }
        }

        #endregion
    }
}
