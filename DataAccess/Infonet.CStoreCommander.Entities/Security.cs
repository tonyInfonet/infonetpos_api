using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using VB = Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    public class Security
    {
        // This class has Created by Behrooz Oct-26-2001
        private DateTime mvarInstall_Date;
        private string mvarSecurity_Key;
        private string mvarStatuse;
        private bool mvarIsValid;
        private string mvarInstall_Date_Encrypt;
        private string mvarMessage; //local copy
        public string[] Limit = new string[9];
        private DateTime mvarExpireDate;

        //Sep-13-2002
        private bool mvarEnhanced_reports; //local copy
        private bool mvarMulti_POS_Support; //local copy
        private bool mvarHead_Office_Support; //local copy
        private bool mvarPump_Control; //local copy
        private bool mvarPay_Pump_Credit; //local copy
        private bool mvarPay_Pump_Debit; //local copy
        private byte mvarNumber_OF_POS; //local copy
        private byte mvarMaxConcurrentPOS; 
        private string mvarNIC_Number; //local copy
        private string mvarPOS_BO_Features; //local copy
        private string mvarPump_Features; //local copy
        private bool mvarPay_InsideCreditDebit; //local copy
        private bool mvarFleet_Card; //local copy
        private bool mvarThirdParty_Card; //local copy
        private bool mvarLoyalty; //local copy
        private bool mvarA_and_R; //local copy
        private bool mvarFuel_Management; //local copy
        private bool mvarMulti_Pumps; //for One Reader to More Pumps, Nancy
        private bool mvarPriceDisplay; //for Fuel Price Display, Nancy
        private string mvarBackOfficeVersion; //local copy
        private bool mvarTax_Exempt; 
        private bool mvarPROMO_SALE;


        public string BackOfficeVersion
        {
            get
            {
                string returnValue = "";
                returnValue = mvarBackOfficeVersion;
                return returnValue;
            }
            set
            {
                mvarBackOfficeVersion = value;
            }
        }


        public bool Fuel_Management
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarFuel_Management;
                return returnValue;
            }
            set
            {
                mvarFuel_Management = value;
            }
        }

        public bool A_and_R
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarA_and_R;
                return returnValue;
            }
            set
            {
                mvarA_and_R = value;
            }
        }


        public bool Loyalty
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarLoyalty;
                return returnValue;
            }
            set
            {
                mvarLoyalty = value;
            }
        }


        public bool ThirdParty_Card
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarThirdParty_Card;
                return returnValue;
            }
            set
            {
                mvarThirdParty_Card = value;
            }
        }


        public bool Fleet_Card
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarFleet_Card;
                return returnValue;
            }
            set
            {
                mvarFleet_Card = value;
            }
        }


        public bool Pay_InsideCreditDebit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPay_InsideCreditDebit;
                return returnValue;
            }
            set
            {
                mvarPay_InsideCreditDebit = value;
            }
        }




        public string Pump_Features
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPump_Features;
                return returnValue;
            }
            set
            {

                string TS = "";

                if (value.Length > 16)
                {
                    TS = value.Substring(0, 16);
                }
                else if (value.Length < 16)
                {
                    TS = value + new string('0', 16 - value.Length);
                }
                else
                {
                    TS = value;
                }

                mvarPump_Features = TS;

                this.Pump_Control = (TS.Substring(0, 1) == "0") ? false : true;
                this.Pay_Pump_Credit = (TS.Substring(1, 1) == "0") ? false : true;
                this.Pay_Pump_Debit = (TS.Substring(2, 1) == "0") ? false : true;
                this.Fuel_Management = (TS.Substring(3, 1) == "0") ? false : true;
                this.Multi_Pumps = (TS.Substring(4, 1) == "0") ? false : true;
                this.PRICEDISPLAY = (TS.Substring(5, 1) == "0") ? false : true;


            }
        }

        public bool Multi_Pumps
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarMulti_Pumps;
                return returnValue;
            }
            set
            {
                mvarMulti_Pumps = value;
            }
        }

        public bool PRICEDISPLAY
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPriceDisplay;
                return returnValue;
            }
            set
            {
                mvarPriceDisplay = value;
            }
        }


        public string POS_BO_Features
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPOS_BO_Features;
                return returnValue;
            }
            set
            {
                string TS = "";

                if (value.Length > 32)
                {
                    TS = value.Substring(0, 32);
                }
                else if (value.Length < 32)
                {
                    TS = value + new string('0', 32 - value.Length);
                }
                else
                {
                    TS = value;
                }

                mvarPOS_BO_Features = TS;

                this.BackOfficeVersion = System.Convert.ToString((TS.Substring(0, 1) == "0") ? "Lite" : "Full");
                //-----------------
                // 
                //   Me.Fleet_Card = IIf(Mid(TS, 1, 1) = "0", False, True)
                //   Me.ThirdParty_Card = IIf(Mid(TS, 1, 1) = "0", False, True)
                //shiny end
                this.A_and_R = (TS.Substring(0, 1) == "0") ? false : true;
                this.Loyalty = (TS.Substring(0, 1) == "0") ? false : true;
                this.Enhanced_reports = (TS.Substring(0, 1) == "0") ? false : true;
                //------------------
                this.Pay_InsideCreditDebit = (TS.Substring(1, 1) == "0") ? false : true;
                this.Multi_POS_Support = (TS.Substring(2, 1) == "0") ? false : true;
                this.Head_Office_Support = (TS.Substring(3, 1) == "0") ? false : true;
                this.Fleet_Card = (TS.Substring(4, 1) == "0") ? false : true; // 

                
                this.TAX_EXEMPT = (TS.Substring(5, 1) == "0") ? false : true;
                

                this.PROMO_SALE = (TS.Substring(6, 1) == "0") ? false : true;
            }
        }

        

        public bool TAX_EXEMPT
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarTax_Exempt;
                return returnValue;
            }
            set
            {
                mvarTax_Exempt = value;
            }
        }
        

        //Promotional Sale

        public bool PROMO_SALE
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPROMO_SALE;
                return returnValue;
            }
            set
            {
                mvarPROMO_SALE = value;
            }
        }


        public string NIC_Number
        {
            get
            {
                string returnValue = "";
                returnValue = mvarNIC_Number;
                return returnValue;
            }
            set
            {
                mvarNIC_Number = value;
            }
        }


        public byte Number_OF_POS
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarNumber_OF_POS;
                return returnValue;
            }
            set
            {
                mvarNumber_OF_POS = value;
            }
        }

        

        public byte MaxConcurrentPOS
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarMaxConcurrentPOS;
                return returnValue;
            }
            set
            {
                mvarMaxConcurrentPOS = value;
            }
        }
        


        public bool Pay_Pump_Debit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPay_Pump_Debit;
                return returnValue;
            }
            set
            {
                mvarPay_Pump_Debit = value;
            }
        }


        public bool Pay_Pump_Credit
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPay_Pump_Credit;
                return returnValue;
            }
            set
            {
                mvarPay_Pump_Credit = value;
            }
        }


        public bool Pump_Control
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarPump_Control;
                return returnValue;
            }
            set
            {
                mvarPump_Control = value;
            }
        }


        public bool Head_Office_Support
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarHead_Office_Support;
                return returnValue;
            }
            set
            {
                mvarHead_Office_Support = value;
            }
        }


        public bool Multi_POS_Support
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarMulti_POS_Support;
                return returnValue;
            }
            set
            {
                mvarMulti_POS_Support = value;
            }
        }


        public bool Enhanced_reports
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarEnhanced_reports;
                return returnValue;
            }
            set
            {
                mvarEnhanced_reports = value;
            }
        }


        // Add by Behrooz Feb-27-2002
        // This property keeps Installation date + Limit Date

        public DateTime ExpireDate
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarExpireDate;
                return returnValue;
            }
            set
            {
                mvarExpireDate = value;
            }
        }

        public string Message
        {
            get
            {
                string returnValue = "";
                returnValue = mvarMessage;
                return returnValue;
            }
            set
            {
                mvarMessage = value;
            }
        }



        public string Install_Date_Encrypt
        {
            get
            {
                string returnValue = "";
                returnValue = mvarInstall_Date_Encrypt;
                return returnValue;
            }
            set
            {
                mvarInstall_Date_Encrypt = value;
            }
        }


        public bool IsValid
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsValid;
                return returnValue;
            }
            set
            {
                mvarIsValid = value;
            }
        }


        public string Statuse
        {
            get
            {
                string returnValue = "";
                returnValue = mvarStatuse;
                return returnValue;
            }
            set
            {
                mvarStatuse = value;
            }
        }


        public string Security_Key
        {
            get
            {
                string returnValue = "";
                returnValue = mvarSecurity_Key;
                return returnValue;
            }
            set
            {
                mvarSecurity_Key = value;
            }
        }


        public DateTime Install_Date
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarInstall_Date;
                return returnValue;
            }
            set
            {
                mvarInstall_Date = value;
            }
        }

        public string Return_EncryptDate(DateTime DT)
        {
            string returnValue = "";
            string str_Renamed = "";
            short i = 0;
            string DL = "";

            //    str = Format(DT, "mm/dd/yyyy")
            ///changed by nancy
            str_Renamed = DateAndTime.Month(DT).ToString("00") + "/" + 
                DateAndTime.Day(DT).ToString("00") + "/" + 
                DateAndTime.Year(DT).ToString("0000");

            for (i = 1; i <= 10; i++)
            {
                DL = DL + System.Convert.ToString(Strings.Asc(str_Renamed.Substring(i - 1, 1)) + 7);
            }

            for (i = 1; i <= 10; i++)
            {
                VBMath.Randomize();
                DL = (VBMath.Rnd() * VBMath.Rnd() * i * 5).ToString("00") + DL;
            }

            for (i = 1; i <= 10; i++)
            {
                VBMath.Randomize();
                DL = DL + (VBMath.Rnd() * VBMath.Rnd() * i * 5).ToString("00"); //Format(Rnd() + i * i + (i * Rnd()) / i, "00")
            }

            returnValue = DL;

            return returnValue;
        }    

        private void Class_Initialize_Renamed()
        {

            this.Install_Date = System.Convert.ToDateTime(System.DateTime.FromOADate(0));
            this.Install_Date_Encrypt = "";
            this.Security_Key = "";

            Limit[1] = "30";
            Limit[2] = "45";
            Limit[3] = "60";
            Limit[4] = "75";
            Limit[5] = "90";
            Limit[6] = "120";
            Limit[7] = "180";
            Limit[8] = "PAID";

        }
        public Security()
        {
            Class_Initialize_Renamed();
        }

        public void Set_Policy_Features()
        {
            //Smriti move this code to manager
            //  Pay_InsideCreditDebit
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PAY_INSIDE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pay_InsideCreditDebit;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'MASK_CARDNO\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pay_InsideCreditDebit;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'SWIPE_CARD\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pay_InsideCreditDebit;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'CC_MODE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = true; // Me.Pay_InsideCreditDebit
            //    if (!this.Pay_InsideCreditDebit)
            //    {
            //        Chaps_Main.rsComp.Fields["P_Set"].Value = "Cross-Ring";
            //    }
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'USE_PINPAD\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pay_InsideCreditDebit;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// Fleet Card
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'FLEET_CARD\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Fleet_Card;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// ThirdParty Card
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'THIRD_PARTY\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Fleet_Card;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// A/R
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'AR\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_OR_LIMIT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_ARSALE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'CREDTERM\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'USE_ARCUST\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'HO_ARPROCESS\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R && this.Head_Office_Support;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_ARSALES\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_ARPRINT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_ARCLOSE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.A_and_R;
            //    Chaps_Main.rsComp.Update();
            //}

            //// LOYALTY
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYALTY\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'USE_LOYALTY\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_TYPE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_PRICE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_DISC\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_NAME\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_PPD\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOY-EXCLUDE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_LIMIT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOY_NOREDPO\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'LOYAL_PPU\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'SHOW_POINT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'ALLOW_CUR_PT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'GIVE_POINTS\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'VOL_POINTS\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'USE_CUST\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'CUST_DISC\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_CLSTATUS\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PROD_DISC\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'OR_USER_DISC\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Loyalty;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// ENHANCED REPORTS
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'E_REPORTS\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Enhanced_reports;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// MULTI POS
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'MULTI_POS\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Multi_POS_Support;
            //    Chaps_Main.rsComp.Update();
            //}


            ////----------------------------------------------------------
            //// HEAD OFFICE SUPPORT
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'HO\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Head_Office_Support;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_SEND\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Head_Office_Support;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_GET\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Head_Office_Support;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// PUMP CONTROL
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PUMP_CONTROL\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pump_Control;
            //    Chaps_Main.rsComp.Update();
            //}
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'USE_FUEL\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pump_Control;
            //    Chaps_Main.rsComp.Update();
            //}

            ////  Making these pricechange policies  true irrespective of fuel control for posonly (pump control disabled from security)- Now we need to allow price change for posonly
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_CHGFPRICE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = true; // Me.Pump_Control
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_FUELSETUP\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = true; //Me.Pump_Control
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_FUELGP\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = true; //Me.Pump_Control
            //    Chaps_Main.rsComp.Update();
            //}


            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'FUEL_UM\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = true; //Me.Pump_Control
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'FUEL_GP\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = true; //Me.Pump_Control
            //    Chaps_Main.rsComp.Update();
            //}
            //// 
            ////----------------------------------------------------------
            //// PAY AT THE PUMP CREDIT
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PUMP_CREDIT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pay_Pump_Credit;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// PAY AT THE PUMP DEBIT
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PUMP_DEBIT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Pay_Pump_Debit;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// One Reader To more Pumps
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'MULTI_PUMP\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Multi_Pumps;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// Fuel Price Display
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PRICEDISPLAY\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.PRICEDISPLAY;
            //    Chaps_Main.rsComp.Update();
            //}

            ////----------------------------------------------------------
            //// FUEL MANAGEMENT
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'FUEL_MGMT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Fuel_Management;
            //    Chaps_Main.rsComp.Update();
            //}

            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'U_FUELMGMT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = this.Fuel_Management;
            //    Chaps_Main.rsComp.Update();
            //}

            ////-------------------------------
            //// Policies without feature link
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'GROUP_PRTY\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = mvarBackOfficeVersion == "Full";
            //    Chaps_Main.rsComp.Update();
            //}

            //// Policy for Tax Exempt
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'TAX_EXEMPT\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = mvarTax_Exempt;
            //    Chaps_Main.rsComp.Update();
            //}

            //// Policy for Promotion
            //Chaps_Main.rsComp.Find(Criteria: "P_NAME=\'PROMO_SALE\'", SearchDirection: ADODB.SearchDirectionEnum.adSearchForward, Start: 1);
            //if (!Chaps_Main.rsComp.EOF)
            //{
            //    Chaps_Main.rsComp.Fields["Implemented"].Value = mvarPROMO_SALE;
            //    Chaps_Main.rsComp.Update();
            //}
        }
    }
}
