using System;

namespace Infonet.CStoreCommander.Entities
{
    public class teCardholder
    {

        private string mvarName;
        private string mvarCardholderID;
        private string mvarCardNumber;
        private string mvarBarcode;
        private DateTime mvarBirthDate;
        private bool mvarIsValidCardHolder;
        private byte mvarValidateMode; 
        private float mvarGasQuota;
        private float mvarPropaneQuota;
        private float mvarTobaccoQuota;
        private bool mvarGstExempt; 
        private string mvarAddress;
        private string mvarCity;
        private string mvarPostalCode;
        private string mvarPlatenumber;
        private string mvarNote; //  

        public bool AffixBarcode(string strBarcode)
        {
            bool returnValue = false;
            //Smriti move this code to manager
            //ADODB.Recordset rs = default(ADODB.Recordset);

            //returnValue = false;
            //if (string.IsNullOrEmpty(mvarCardNumber))
            //{
            //    return returnValue;
            //}
            //
            //
            //

            //
            //
            //rs = Chaps_Main.Get_Records("Select * from TaxExemptCardRegistry where BarCode=\'" + strBarcode + "\' " + " OR AltBarCode=\'" + strBarcode + "\'", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);
            //if (!rs.EOF)
            //{
            //    return returnValue;
            //}
            //

            //
            //
            //if (mvarValidateMode == 3 | mvarValidateMode == 1)
            //{
            //    
            //    rs = Chaps_Main.Get_Records("select * from TaxExemptCardRegistry where CardNumber=\'" + mvarCardNumber + "\' ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);
            //    if (rs.EOF)
            //    {
            //        rs = null;
            //        return returnValue;
            //    }
            //    else
            //    {
            //        if (rs.Fields["Barcode"].Value != strBarcode || Information.IsDBNull(rs.Fields["Barcode"].Value)) //   added Or IsNull(rs!Barcode) condition to allow update for null values
            //        {
            //            rs.Fields["Barcode"].Value = strBarcode;
            //            rs.Fields["Updated"].Value = 1; 
            //            rs.Fields["UpdateTime"].Value = DateTime.Now;
            //            rs.Update();
            //        }
            //    }
            //}
            //else
            //{
            //    rs = Chaps_Main.Get_Records("select * from TaxExemptCardRegistry where AltCardNumber=\'" + mvarCardNumber + "\' ", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly);
            //    if (rs.EOF)
            //    {
            //        rs = null;
            //        return returnValue;
            //    }
            //    else
            //    {
            //        if (rs.Fields["AltBarCode"].Value != strBarcode)
            //        {
            //            rs.Fields["AltBarCode"].Value = strBarcode;
            //            rs.Fields["Updated"].Value = 1; 
            //            rs.Fields["UpdateTime"].Value = DateTime.Now;
            //            rs.Update();
            //        }
            //    }
            //}
            //mvarBarcode = strBarcode;
            //rs = null;
            //returnValue = true;
            return returnValue;
        }


        public string CardholderID
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardholderID;
                return returnValue;
            }
            set
            {
                mvarCardholderID = value;
            }
        }


        public string Name
        {
            get
            {
                string returnValue = "";
                returnValue = mvarName;
                return returnValue;
            }
            set
            {
                mvarName = value;
            }
        }


        public string CardNumber
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCardNumber;
                return returnValue;
            }
            set
            {
                mvarCardNumber = value;
            }
        }


        public string Barcode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarBarcode;
                return returnValue;
            }
            set
            {
                mvarBarcode = value;
            }
        }


        public DateTime Birthdate
        {
            get
            {
                DateTime returnValue = default(DateTime);
                returnValue = mvarBirthDate;
                return returnValue;
            }
            set
            {
                mvarBirthDate = value;
            }
        }


        public bool IsValidCardHolder
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarIsValidCardHolder;
                return returnValue;
            }
            set
            {
                mvarIsValidCardHolder = value;
            }
        }

        

        public bool GstExempt
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarGstExempt;
                return returnValue;
            }
            set
            {
                mvarGstExempt = value;
            }
        }
        


        public byte ValidateMode
        {
            get
            {
                byte returnValue = 0;
                returnValue = mvarValidateMode;
                return returnValue;
            }
            set
            {
                mvarValidateMode = value;
            }
        }


        public float GasQuota
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarGasQuota;
                return returnValue;
            }
            set
            {
                mvarGasQuota = value;
            }
        }


        public float PropaneQuota
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarPropaneQuota;
                return returnValue;
            }
            set
            {
                mvarPropaneQuota = value;
            }
        }


        public float TobaccoQuota
        {
            get
            {
                float returnValue = 0;
                returnValue = mvarTobaccoQuota;
                return returnValue;
            }
            set
            {
                mvarTobaccoQuota = value;
            }
        }

        //  - For QITE - we don't need to use the  taxexempt registry- customer will be client table and Taxexempt flag will be true
        //After discussion with PM we are not populationng Sa.Cstomer- To avoid, discount and other customer related stuff- we will keeep separte for Tax exempt


        public string Address
        {
            get
            {
                string returnValue = "";
                returnValue = mvarAddress;
                return returnValue;
            }
            set
            {
                mvarAddress = value;
            }
        }

        public string City
        {
            get
            {
                string returnValue = "";
                returnValue = mvarCity;
                return returnValue;
            }
            set
            {
                mvarCity = value;
            }
        }

        public string PostalCode
        {
            get
            {
                string returnValue = "";
                returnValue = mvarPostalCode;
                return returnValue;
            }
            set
            {
                mvarPostalCode = value;
            }
        }


        public string PlateNumber
        {
            get
            {
                return mvarPlatenumber;
            }
            set
            {
                mvarPlatenumber = value;
            }
        }


        public string Note
        {
            get
            {
                string returnValue = "";
                returnValue = mvarNote;
                return returnValue;
            }
            set
            {
                mvarNote = value;
            }
        }

        private void Class_Initialize_Renamed()
        {
            mvarIsValidCardHolder = false;
            mvarValidateMode = (byte)0;
            mvarGstExempt = false; 
        }

        public teCardholder()
        {
            Class_Initialize_Renamed();
        }

        
        
        //public bool ValidCardHolder(bool IsBarCode, string strNumber, ref short MatchCount)
        //{
        //    bool returnValue = false;
        
        ////    dynamic Policy_Renamed = default(dynamic);
        ////    
        ////    ADODB.Recordset rs = default(ADODB.Recordset);
        ////    string strSql = "";
        ////    DateTime mBirthDate = default(DateTime);

        ////    MatchCount = (short)0; 

        ////    returnValue = false;
        ////    if (IsBarCode)
        ////    {
        ////        strSql = "Select * from TaxExemptCardRegistry where BarCode=\'" + strNumber + "\' " + " OR AltBarCode=\'" + strNumber + "\'";
        ////    }
        ////    else
        ////    {
        ////        strSql = "Select * from TaxExemptCardRegistry where CardNumber=\'" + strNumber + "\' " + " OR AltCardNumber=\'" + strNumber + "\'";
        ////    }
        ////    rs = Chaps_Main.Get_Records(strSql, Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
        ////    if (!rs.EOF)
        ////    {
        ////        
        ////        MatchCount = (short)rs.RecordCount;
        ////        if (MatchCount > 1)
        ////        {
        ////            rs = null;
        ////            this.IsValidCardHolder = false;
        ////            return returnValue;
        ////        }
        ////        

        ////        
        ////        
        ////        
        ////        
        ////        
        ////        if (Information.IsDBNull(rs.Fields["Birthdate"].Value))
        ////        {
        ////            mBirthDate = DateAndTime.DateAdd(Microsoft.VisualBasic.DateInterval.Year, System.Convert.ToDouble((-1) * (Policy_Renamed.AgeRestrict + 1)), DateAndTime.Today);
        ////        }
        ////        else
        ////        {
        ////            mBirthDate = System.Convert.ToDateTime(rs.Fields["Birthdate"].Value);
        ////        }
        ////        this.Birthdate = mBirthDate;
        ////        

        ////        this.Name = System.Convert.ToString((Information.IsDBNull(rs.Fields["Name"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["Name"].Value))));
        ////        this.CardholderID = System.Convert.ToString((Information.IsDBNull(rs.Fields["CardholderID"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["CardholderID"].Value))));
        ////        returnValue = true;
        ////        if (IsBarCode)
        ////        {
        ////            if ((string)(rs.Fields["Barcode"].Value) == strNumber)
        ////            {
        ////                this.ValidateMode = (byte)1;
        ////                this.CardNumber = System.Convert.ToString((Information.IsDBNull(rs.Fields["CardNumber"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["CardNumber"].Value))));
        ////            }
        ////            else
        ////            {
        ////                this.ValidateMode = (byte)2;
        ////                this.CardNumber = System.Convert.ToString((Information.IsDBNull(rs.Fields["AltCardNumber"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["AltCardNumber"].Value))));
        ////            }
        ////            this.Barcode = strNumber;
        ////        }
        ////        else
        ////        {
        ////            if ((string)(rs.Fields["CardNumber"].Value) == strNumber)
        ////            {
        ////                this.ValidateMode = (byte)3;
        ////                this.Barcode = System.Convert.ToString((Information.IsDBNull(rs.Fields["Barcode"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["Barcode"].Value))));
        ////            }
        ////            else
        ////            {
        ////                this.ValidateMode = (byte)4;
        ////                this.Barcode = System.Convert.ToString((Information.IsDBNull(rs.Fields["AltBarCode"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rs.Fields["AltBarCode"].Value))));
        ////            }
        ////            this.CardNumber = strNumber;
        ////        }
        ////        this.GasQuota = System.Convert.ToSingle((Information.IsDBNull(rs.Fields["GasQuota"].Value)) ? 0 : (rs.Fields["GasQuota"].Value));
        ////        this.PropaneQuota = System.Convert.ToSingle((Information.IsDBNull(rs.Fields["PropaneQuota"].Value)) ? 0 : (rs.Fields["PropaneQuota"].Value));
        ////        this.TobaccoQuota = System.Convert.ToSingle((Information.IsDBNull(rs.Fields["TobaccoQuota"].Value)) ? 0 : (rs.Fields["TobaccoQuota"].Value));
        ////    }

        ////    this.IsValidCardHolder = System.Convert.ToBoolean(returnValue);

        ////    rs = null;
        //   return returnValue;
        //}

        //public bool ValidTaxExemptCustomer(string CustomerID)
        //{
        //    bool returnValue = false;
        ////    ADODB.Recordset rsCustomer = new ADODB.Recordset();

        ////    returnValue = false;
        ////    rsCustomer = Chaps_Main.Get_Records("Select * From Client where CL_Code = \'" + CustomerID + "\'", Chaps_Main.dbMaster);
        ////    if (rsCustomer.EOF)
        ////    {
        ////        returnValue = false;
        ////    }
        ////    else
        ////    {
        ////        if (Information.IsDBNull(rsCustomer.Fields["TaxExempt"].Value))
        ////        {
        ////            returnValue = false;
        ////        }
        ////        else if (rsCustomer.Fields["TaxExempt"].Value == true)
        ////        {
        ////            returnValue = true;
        ////            this.Name = System.Convert.ToString((Information.IsDBNull(rsCustomer.Fields["Cl_Name"].Value)) ? "" : (Strings.Trim(System.Convert.ToString(rsCustomer.Fields["Cl_Name"].Value))));
        ////            this.Address = System.Convert.ToString((Information.IsDBNull(rsCustomer.Fields["CL_Add1"].Value)) ? "" : (rsCustomer.Fields["CL_Add1"].Value));
        ////            this.City = System.Convert.ToString((Information.IsDBNull(rsCustomer.Fields["CL_City"].Value)) ? "" : (rsCustomer.Fields["CL_City"].Value));
        ////            this.PlateNumber = System.Convert.ToString((Information.IsDBNull(rsCustomer.Fields["PlateNumber"].Value)) ? "" : (rsCustomer.Fields["PlateNumber"].Value));
        ////            this.PostalCode = System.Convert.ToString((Information.IsDBNull(rsCustomer.Fields["CL_Postal"].Value)) ? "" : (rsCustomer.Fields["CL_Postal"].Value));
        ////            this.Note = System.Convert.ToString((Information.IsDBNull(rsCustomer.Fields["CL_Note"].Value)) ? "" : (rsCustomer.Fields["CL_Note"].Value));
        ////            this.CardholderID = CustomerID;
        ////            this.CardNumber = CustomerID;
        ////        }
        ////        else
        ////        {
        ////            returnValue = false;
        ////        }
        ////        this.IsValidCardHolder = System.Convert.ToBoolean(returnValue);
        ////    }
        ////    rsCustomer = null;
        //    return returnValue;
        //}
    }
}
