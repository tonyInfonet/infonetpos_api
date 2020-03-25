using Infonet.CStoreCommander.ADOData;
using Infonet.CStoreCommander.BusinessLayer.Entities;
using Infonet.CStoreCommander.BusinessLayer.Utilities;
using Infonet.CStoreCommander.Entities;
using Infonet.CStoreCommander.Resources;
using Microsoft.VisualBasic;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.BusinessLayer.Manager
{
    public class TierLevelManager : ManagerBase, ITierLevelManager
    {
        private readonly IApiResourceManager _resourceManager;
        private readonly IFuelPumpService _fuelPumpService;
        private readonly IPolicyManager _policyManager;

        public TierLevelManager(
            IApiResourceManager resourceManager,
            IFuelPumpService fuelPumpService,
            IPolicyManager policyManager)
        {
            _resourceManager = resourceManager;
            _fuelPumpService = fuelPumpService;
            _policyManager = policyManager;
        }

        /// <summary>
        /// Get Tier and Levels for all pumps
        /// </summary>
        /// <returns></returns>
        public TierLevelResponse GetAllPumps()
        {
            var offSet = _policyManager.LoadStoreInfo().OffSet;
            var result = new TierLevelResponse
            {
                PageCaption = _resourceManager.CreateCaption(offSet,24, 44, null, 1),
                PumpTierLevels = GetPumpsTierLevel(),
                Tiers = GetAllTiers(),
                Levels = GetAllLevels()
            };
            return result;
        }

        /// <summary>
        /// Add update tier level
        /// </summary>
        /// <param name="pumpIds"></param>
        /// <param name="tierId"></param>
        /// <param name="levelId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public TierLevelResponse AddUpdateTierLevel(List<int> pumpIds, int tierId, int levelId, out ErrorMessage error)
        {
            error = new ErrorMessage();
            var result = new TierLevelResponse
            {
                Tiers = GetAllTiers(),
                Levels = GetAllLevels()
            };

            object i;
            string tierLevel = "";
            bool pumpSelected = false;
            var store = CacheManager.GetStoreInfo();
            var offSet = store?.OffSet ?? 0;
            float timeIn = 0;
            for (i = 1; (int)i <= (int)Variables.gPumps.PumpsCount; i = (int)i + 1)
            {
                if (tierId == 0)
                {
                    //        MsgBox ("Please select Tier!") '21
                    //MsgBoxStyle temp_VbStyle = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Critical;
                    //Chaps_Main.DisplayMessage(this, (short)21, temp_VbStyle, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet,44, 21, null, CriticalOkMessageType)
                    };
                    return null;
                }
                if (levelId == 0)
                {
                    //        MsgBox ("Please select Level!") '22
                    //MsgBoxStyle temp_VbStyle2 = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Critical;
                    //Chaps_Main.DisplayMessage(this, (short)22, temp_VbStyle2, null, (byte)0);
                    error = new ErrorMessage
                    {
                        MessageStyle = _resourceManager.CreateMessage(offSet,44, 22, null, CriticalOkMessageType)
                    };
                    return null;
                }

                switch (tierId + levelId.ToString())
                {
                    case "11":
                        tierLevel = "0";
                        break;
                    case "12":
                        tierLevel = "1";
                        break;
                    case "21":
                        tierLevel = "2";
                        break;
                    case "22":
                        tierLevel = "3";
                        break;
                }
                
                if (pumpIds.Contains((int)i))
                {
                    pumpSelected = true;
                    //lblinformation.Caption = "Setting in Process... Please Wait..." 'Resource.DisplayCaption(lblinformation.Tag, Me.Tag, , 2) '24-4
                    //lblinformation.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblinformation.Tag), System.Convert.ToInt16(this.Tag), null, (short)4); //24-4
                    result.PageCaption = _resourceManager.CreateCaption(offSet,24, 44, null, 4);

                    if (TCPAgent.Instance.PortOpened)
                    {
                        var response = "";
                        var strRemain = "";
                        

                        var cmd = "STL" + Strings.Right("00" + System.Convert.ToString(i), 2) + tierLevel;
                        TCPAgent.Instance.Send_TCP(ref cmd, true);
                        if (timeIn > DateAndTime.Timer)
                        {
                            timeIn = 0; //reset on midnight
                        }
                        else
                        {
                            timeIn = (float)DateAndTime.Timer;
                        }

                        while (!(DateAndTime.Timer - timeIn > Variables.gPumps.CommunicationTimeOut))
                        {
                            var strBuffer = System.Convert.ToString(TCPAgent.Instance.NewPortReading);
                            WriteToLogFile("TCPAgent.PortReading: " + strBuffer + " from waiting STL" + Strings.Right("00" + System.Convert.ToString(i), 2));
                            if (!string.IsNullOrEmpty(strBuffer))
                            {

                                modStringPad.SplitResponse(strBuffer, "STL" + Strings.Right("00" + System.Convert.ToString(i), 2), ref response, ref strRemain); //strBuffer<>""
                                if (!string.IsNullOrEmpty(response)) //got what we are waiting
                                {

                                    TCPAgent.Instance.PortReading = strRemain; //& ";" & TCPAgent.PortReading
                                    WriteToLogFile("modify TCPAgent.PortReading from Get Totalizer: " + strRemain);
                                    break;
                                }
                            }
                            Variables.Sleep(100);
                            if (DateAndTime.Timer < timeIn)
                            {
                                timeIn = (float)DateAndTime.Timer;
                            }
                        }


                        if (Strings.Left(response, 7) != "STL" + Strings.Right("00" + System.Convert.ToString(i), 2) + "OK")
                        {

                            string tempCommandRenamed = "ENDPOS";
                            TCPAgent.Instance.Send_TCP(ref tempCommandRenamed, true);
                            //  lblinformation.Caption = "Setting TierLevel failed !" 'Resource.DisplayCaption(lblinformation.Tag, Me.Tag, , 2) '24-3
                            //(System.Convert.ToInt16(lblinformation.Tag), System.Convert.ToInt16(this.Tag), null, (short)3); //24-3
                            result.PageCaption = _resourceManager.CreateCaption(offSet,24, 44, null, 3);
                            return result;
                        }

                        
                        _fuelPumpService.UpdateTierLevelForPump((int)i, tierId, levelId);

                        
                        
                        

                        Variables.gPumps.get_Pump(System.Convert.ToByte(i)).TierID = (byte)tierId;

                        Variables.gPumps.get_Pump(System.Convert.ToByte(i)).LevelID = (byte)levelId;
                        

                        if (Variables.Pump[(int)i].cashUP == null)
                        {
                            Variables.Pump[(int)i].cashUP = new float[10];
                        }

                        if (Variables.Pump[(int)i].creditUP == null)
                        {
                            Variables.Pump[(int)i].creditUP = new float[10];
                        }

                        if (Variables.Pump[(int)i].Stock_Code == null)
                        {
                            Variables.Pump[(int)i].Stock_Code = new string[10];
                        }


                        short j;
                        for (j = 1; j <= Variables.gPumps.get_PositionsCount(System.Convert.ToByte(i)); j++)
                        {


                            Variables.Pump[(int)i].cashUP[j] = Variables.gPumps.get_FuelPrice(System.Convert.ToByte(Variables.gPumps.get_Assignment(System.Convert.ToByte(i), (byte)j).GradeID), (byte)(Conversion.Val(tierId)), (byte)(Conversion.Val(levelId))).CashPrice;

                            Variables.Pump[(int)i].creditUP[j] = Variables.gPumps.get_FuelPrice(System.Convert.ToByte(Variables.gPumps.get_Assignment(System.Convert.ToByte(i), (byte)j).GradeID), (byte)(Conversion.Val(tierId)), (byte)(Conversion.Val(levelId))).CreditPrice;
                        }
                    }
                    else
                    {
                        //            lblinformation.Caption = "Please select pump and corresponding Tier/Level, then press 'SET TO PUMP' button to set!"  '24-1
                        //lblinformation.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblinformation.Tag), System.Convert.ToInt16(this.Tag), null, (short)1); //24-1
                        result.PageCaption = _resourceManager.CreateCaption(offSet,24, 44, null, 1);
                        result.PumpTierLevels = GetPumpsTierLevel();
                        //MsgBox "No TCP connection to Host !!!", vbCritical
                        //MsgBoxStyle temp_VbStyle3 = (int)MsgBoxStyle.OkOnly + MsgBoxStyle.Critical;
                        //Chaps_Main.DisplayMessage(this, (short)92, temp_VbStyle3, null, (byte)0);
                        error.MessageStyle = new MessageStyle
                        {
                            Message = _resourceManager.GetResString(offSet,3892),
                            MessageType = MessageType.OkOnly
                        };
                        return result;
                    }
                }
            }

            if (!pumpSelected) //if no pump is selected
            {
                //    MsgBox ("Please select Pump!") '23
                //Chaps_Main.DisplayMessage(this, (short)23, temp_VbStyle4, null, (byte)0);
                error = new ErrorMessage
                {
                    MessageStyle = _resourceManager.CreateMessage(offSet,44, 23, null, CriticalOkMessageType)
                };
                //    lblinformation.Caption = "Please select pump and corresponding Tier/Level, then press 'SET TO PUMP' button to set!" '24-1
                //lblinformation.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblinformation.Tag), System.Convert.ToInt16(this.Tag), null, (short)1); //24-1
                result.PageCaption = _resourceManager.CreateCaption(offSet,24, 44, null, 1);
                result.PumpTierLevels = GetPumpsTierLevel();
                return result;
            }
            //    lblinformation.Caption = "Setting TierLevel is Done successfully !" 'Resource.DisplayCaption(lblinformation.Tag, Me.Tag, , 2) '24-2
            //lblinformation.Text = Chaps_Main.Resource.DisplayCaption(System.Convert.ToInt16(lblinformation.Tag), System.Convert.ToInt16(this.Tag), null, (short)2); //24-2
            result.PageCaption = _resourceManager.CreateCaption(offSet,24, 44, null, 2);
            result.PumpTierLevels = GetPumpsTierLevel();
            return result;
        }

        #region Private methods

        /// <summary>
        /// Method to get pumps tier level
        /// </summary>
        /// <returns>Pump tier level</returns>
        private List<PumpTierLevel> GetPumpsTierLevel()
        {
            short i;
            var pumpTierLevels = new List<PumpTierLevel>();
            for (i = 1; i <= Variables.iPumpCount; i++)
            {
                var pumpTierLevel = new PumpTierLevel();
                pumpTierLevel.PumpId = i;
                pumpTierLevel.TierId = Variables.gPumps.get_Pump((byte)i).TierID;
                pumpTierLevel.LevelId = Variables.gPumps.get_Pump((byte)i).LevelID;

                pumpTierLevel.TierName = Variables.gPumps.get_Tier(pumpTierLevel.TierId);
                pumpTierLevel.LevelName = Variables.gPumps.get_Level(pumpTierLevel.LevelId);

                pumpTierLevels.Add(pumpTierLevel);
            }
            return pumpTierLevels;
        }

        /// <summary>
        /// Method to get all tiers
        /// </summary>
        /// <returns></returns>
        private List<Tier> GetAllTiers()
        {
            return new List<Tier>
            {
                new Tier
                {
                    TierId = 1,
                    TierName = Variables.gPumps.get_Tier(1)
                },
                new Tier
                {
                    TierId = 2,
                    TierName = Variables.gPumps.get_Tier(2)
                }
            };
        }

        /// <summary>
        /// Method to get all levels
        /// </summary>
        /// <returns></returns>
        private List<Level> GetAllLevels()
        {
            return new List<Level>
            {
                new Level
                {
                    LevelId = 1,
                    LevelName = Variables.gPumps.get_Level(1)
                },
                new Level
                {
                    LevelId= 2,
                    LevelName = Variables.gPumps.get_Level(2)
                }
            };
        }

        #endregion

    }
}
