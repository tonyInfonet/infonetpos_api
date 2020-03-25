using System;

namespace Infonet.CStoreCommander.Entities
{
    public class GetProperty
    {
        private CGrade myEGrade = new CGrade();
        public CFuelPrice EFuelPrice = new CFuelPrice();
        private CTankType myTankType;
        private CTankInfo myTankInfo; 
        private CPump myEPump = new CPump();
        private CTierLevelPriceDiff myTierLevelPriceDiff;
        private CGradePriceIncrement myGradePriceIncrement;
        private CPricesToDisplay myPricesToDisplay;
        private string[] myPumpStatus = new string[33];
        public bool[] GradeIsExist = new bool[10];
        public byte[] PositionsCount = new byte[33];
        public string[] Level = new string[5];
        public string[] Tier = new string[5];
        public CAssignment[,] Assignment = new CAssignment[33, 10];
        public CGrade[] Grade = new CGrade[10];
        public CFuelPrice[,,] FuelPrice = new CFuelPrice[10, 3, 3];
        public CPump[] Pump = new CPump[33];
        private CAssignment myEAssignment = new CAssignment();
        public TierLevelType[] myTierLevel;
        public TierLevelType[] myTierLevelTaxExemption;
        public MyGradeType2[] myUnBaseGrade;
        public MyGradeType2[] myUnBaseGradeTaxExemption;

        public CGrade get_Grade(byte ID)
        {
            CGrade returnValue = default(CGrade);
            if (ID > 9 | ID < 1)
            {
                returnValue = myEGrade;
            }
            else
            {
                if (Grade[ID] == null)
                {
                    returnValue = myEGrade;
                }
                else
                {
                    returnValue = Grade[ID];
                }
            }
            return returnValue;
        }

        public byte Pump_Space { get; set; }

        public short PumpsCount { get; set; }

        public short GradesCount { get; set; }

        public short MaxPlaceOnScreen { get; set; }

        public short TanksCount { get; set; }

        public dynamic get_PositionsCount(byte pumpID)
        {
            dynamic returnValue = default(dynamic);
            returnValue = PositionsCount[pumpID];
            return returnValue;
        }

        public CFuelPrice get_FuelPrice(byte GradeID, byte TierID, byte LevelID)
        {
            CFuelPrice returnValue = default(CFuelPrice);
            if (GradeID < 1 | GradeID > 9 | TierID < 1 | TierID > 2 | LevelID < 1 | LevelID > 2)
            {
                returnValue = EFuelPrice;
            }
            else
            {
                if (FuelPrice[GradeID, TierID, LevelID] == null)
                {
                    returnValue = EFuelPrice;
                }
                else
                {
                    returnValue = FuelPrice[GradeID, TierID, LevelID];
                }
            }
            return returnValue;
        }

        public dynamic LevelsCount { get; set; }

        public dynamic get_Level(byte ID)
        {
            dynamic returnValue = default(dynamic);
            returnValue = Level[ID];
            return returnValue;
        }

        public dynamic TiersCount { get; set; }

        public dynamic get_Tier(byte ID)
        {
            dynamic returnValue = default(dynamic);
            returnValue = Tier[ID];
            return returnValue;
        }

        public CPump get_Pump(byte ID)
        {
            CPump returnValue = default(CPump);
            if (Pump[ID] == null)
            {
                returnValue = myEPump;
            }
            else
            {
                returnValue = Pump[ID];
            }
            return returnValue;
        }

        public CAssignment get_Assignment(byte pumpID, byte PositionID)
        {
            try
            {
                CAssignment returnValue = default(CAssignment);
                if (Assignment[pumpID, PositionID] == null)
                {
                    returnValue = myEAssignment;
                }
                else
                {
                    returnValue = Assignment[pumpID, PositionID];
                }
                return returnValue;
            }
            catch (IndexOutOfRangeException ex)
            {
                return null;
            }
        }

        public string UnitMeasurement { get; set; }

        public float BasketInterval { get; set; }

        public float SoundInterval { get; set; }

        public byte RepeatSoundRatio { get; set; }

        public byte ReadTotDelay { get; set; }

        public bool BrdCst_Value { get; set; }

        public bool WriteLogFile { get; set; }

        public byte ComPort { get; set; }

        public bool PumpLogFile { get; set; }

        public short PoolingTimer { get; set; }

        public CIP IP { get; set; }

        public string get_PumpStatus(byte ID)
        {
            string returnValue = "";
            returnValue = myPumpStatus[ID];
            return returnValue;
        }

        public void set_PumpStatus(byte ID, string Value)
        {
            myPumpStatus[ID] = Value;
        }

        public short UnBaseGradesCount { get; set; }

        public short UnBaseGasGradesCount { get; set; }

        public short UnBaseTierLevelCount { get; set; }

        public bool get_GradeIsExist(byte ID)
        {
            bool returnValue = false;
            returnValue = GradeIsExist[ID];
            return returnValue;
        }

        public byte ClickDelay { get; set; }

        public string SpacesCount { get; set; }

        public byte CommunicationTimeOut { get; set; }

        public byte PriceDisplayComPort { get; set; }

        public byte PriceDisplayRows { get; set; }

        private void Class_Terminate_Renamed()
        {
            short i = 0;
            short j = 0;
            short k = 0;


            for (i = 1; i <= 9; i++)
            {
                Grade[i] = null;
            }

            myEGrade = null;

            for (i = 1; i <= 9; i++)
            {
                for (j = 1; j <= 2; j++)
                {
                    for (k = 1; k <= 2; k++)
                    {
                        FuelPrice[i, j, k] = null;
                    }
                }
            }

            for (i = 1; i <= 32; i++)
            {
                Pump[i] = null;
            }
            myEPump = null;


            for (i = 1; i <= 32; i++)
            {
                for (j = 1; j <= 1; j++)
                {
                    Assignment[i, j] = null;
                }
            }
            myEAssignment = null;
        }

        ~GetProperty()
        {
            Class_Terminate_Renamed();
        }
    }

    public struct PriceType
    {
        public object CashP;
        public object CreditP;
        public double TECashP;
        public double TECreditP;
    }

    public struct MyGradeType2
    {
        public short GradeId;
        public string GradeDesp;
        public PriceType Price;
    }

    public struct TierLevelType
    {
        public short Tier;
        public short Level;
        public object CashP;
        public object CreditP;
    }
}
