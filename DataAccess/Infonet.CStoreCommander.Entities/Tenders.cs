using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class Tenders : IEnumerable<Tender>
    {

        private Collection mCol;

        private Tend_Totals mvarTend_Totals;
        private Tender mvarTender;
        private bool mvarBaseCurr_Used;

        //added new properties
        public string Summary1 { get; set; }

        public string Summary2 { get; set; }

        public bool EnableCompletePayment { get; set; }

        public bool EnableCompleteReceipt { get; set; }

        public bool EnableCompleteNoReceipt { get; set; }

        public bool DisplayNoReceiptButton { get; set; }

        public bool Card_Authorized { get; set; }

        public bool EnableRunAway { get; set; }

        public bool EnablePumpTest { get; set; }

        public string Coupon { get; set; }

        public CustomerDisplay CustomerDisplay { get; set; }

        public Tender tender
        {
            get
            {
                Tender returnValue = default(Tender);
                if (mvarTender == null)
                {
                    mvarTender = new Tender();

                }
                returnValue = mvarTender;
                return returnValue;
            }
            set
            {
                mvarTender = value;
            }
        }

        public Tend_Totals Tend_Totals
        {
            get
            {
                Tend_Totals returnValue = default(Tend_Totals);
                returnValue = mvarTend_Totals;
                return returnValue;
            }
            set
            {
                mvarTend_Totals = value;
            }
        }

        public Tender this[object vntIndexKey]
        {
            get
            {
                Tender returnValue = default(Tender);
                returnValue = mCol[vntIndexKey] as Tender;
                return returnValue;
            }
        }

        public int Count
        {
            get
            {
                int returnValue = 0;
                returnValue = mCol.Count;
                return returnValue;
            }
        }

        //Public ReadOnly Property NewEnum() As stdole.IUnknown
        //Get
        // ExcludeSE
        //NewEnum = mCol._NewEnum
        //End Get
        //End Property

        public System.Collections.IEnumerator GetEnumerator()
        {
            //GetEnumerator = mCol.GetEnumerator
            return mCol.GetEnumerator();
        }

        //  

        public bool BaseCurr_Used
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarBaseCurr_Used;
                return returnValue;
            }
            set
            {
                mvarBaseCurr_Used = value;
            }
        }  

        
        private bool IsEkoGiftCert(string strTender)
        {
            bool returnValue = false;
            //Smriti move this code to manager
            //ADODB.Recordset rsSet = default(ADODB.Recordset);
            //ADODB.Recordset rsPComp = default(ADODB.Recordset);

            //returnValue = false;
            //rsPComp = Chaps_Main.Get_Records("select * from P_Comp where P_NAME=\'GiftTender\'", Chaps_Main.dbAdmin, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
            //if (!rsPComp.EOF)
            //{
            //    if ((string)(rsPComp.Fields["P_Set"].Value) == "EKO")
            //    {
            //        returnValue = true;
            //        rsPComp = null;
            //        return returnValue;
            //    }
            //}
            //rsPComp = null;

            //rsSet = Chaps_Main.Get_Records("select * from P_SET where P_NAME=\'GiftTender\' " + " AND P_VALUE=\'" + strTender + "\' AND P_SET=\'EKO\'", Chaps_Main.dbAdmin, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);
            //if (!rsSet.EOF)
            //{
            //    returnValue = true;
            //    rsSet = null;
            //    return returnValue;
            //}
            //rsSet = null;

            return returnValue;
        }
        

        // Nov 02, 2009 Nicolette added Inactive property to tender object
        // but is not used in Add method because Inactive tenders are not loaded at all
        // in the Tenders collection (they are excluded in the SQL statement in Tenders_Load)
        // Inactive property is set using GetTenderInfo function for combined cards
        // because is combined cards system doesn't load each cards but one so not all the
        // properties are set based on cards inside the combined tender

        public Tender Add(string Tender_Name, string Tender_Class, double Exchange_Rate, 
            bool Give_Change, bool Give_As_Refund, bool System_Can_Adjust, short Sequence_Number,
            string Tender_Code, bool Exact_Change, double MaxAmount, double MinAmount, 
            double Smallest_Unit, bool OPEN_DRAWER, double Amount_Entered, short PrintCopies,
            bool AcceptAspayment, bool SignatureLine,string image, string sKey)
        {
            Tender returnValue = default(Tender);
            //SMriti move this code to manager
            //create a new object
            Tender objNewMember = default(Tender);
            objNewMember = new Tender();

            //set the properties passed into the method
            objNewMember.Tender_Name = Tender_Name;
            objNewMember.Tender_Class = Tender_Class;
            objNewMember.Exchange_Rate = Exchange_Rate;
            objNewMember.Give_Change = Give_Change;
            objNewMember.Give_As_Refund = Give_As_Refund; //not using
            objNewMember.System_Can_Adjust = System_Can_Adjust;
            objNewMember.Sequence_Number = Sequence_Number;
            objNewMember.Tender_Code = Tender_Code;
            objNewMember.Exact_Change = Exact_Change; //not using
            objNewMember.MaxAmount = MaxAmount; //not using
            objNewMember.MinAmount = MinAmount; //not using
            objNewMember.Smallest_Unit = Smallest_Unit;
            //objNewMember.Give_As_Change = Give_As_Change
            objNewMember.Open_Drawer = OPEN_DRAWER;
            objNewMember.PrintCopies = PrintCopies;
            objNewMember.AcceptAspayment = AcceptAspayment; //not using
            objNewMember.SignatureLine = SignatureLine;
            objNewMember.Image = image;
            if (sKey.Length == 0)
            {
                mCol.Add(objNewMember, null, null, null);
            }
            else
            {
                mCol.Add(objNewMember, sKey, null, null);
            }

            //  this.Set_Amount_Entered(objNewMember, (decimal)Amount_Entered, -1);
            //return the object created
            returnValue = objNewMember;
            objNewMember = null;


            return returnValue;
        }

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }



        public Tender Append(Tender tender, string sKey)
        {
            Tender returnValue = default(Tender);

            if (sKey.Length == 0)
            {
                mCol.Add(tender, null, null, null);
            }
            else
            {
                mCol.Add(tender, sKey, null, null);
            }

            //return the object created
            returnValue = tender;

            return returnValue;
        }

        internal void Class_Initialize_Renamed()
        {
            mCol = new Collection();
            mvarTend_Totals = new Tend_Totals();
            mvarTender = new Tender();
            mvarBaseCurr_Used = false;
            CustomerDisplay = new CustomerDisplay();
        }

        public Tenders()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mvarTender = null;
            mvarTend_Totals = null;
            mCol = null;
        }

        IEnumerator<Tender> IEnumerable<Tender>.GetEnumerator()
        {
            return new TenderEnumerator(mCol);
        }

        ~Tenders()
        {
            Class_Terminate_Renamed();
        }

        //   end
    }

    public class TenderEnumerator  : IEnumerator<Tender>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private Tender _current;

        public TenderEnumerator(Collection col)
        {
            mCol = col;
        }

        public Tender Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            mCol = null;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Tender;
            }
            else
            {
                _current = null;
            }
            return (_index <= mCol.Count);
        }

        public void Reset()
        {
            _current = null;
            _index = 0;
        }
    }
}
