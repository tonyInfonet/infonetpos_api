using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections;
namespace Infonet.CStoreCommander.Entities
{
    public class Till_Close : IEnumerable<Close_Line> 
    {

        private Collection mCol;
        private int _index;
        private Close_Line _current;
        private int mvarClose_Num;
        private DateTime mvarClose_Date;
        private DateTime mvarClose_Time;
        private DateTime mvarOpen_Date;
        private DateTime mvarOpen_Time;
        private DateTime mvarShiftDate;
        private string mvarUser;
        private short mvarTill_Number;
        private int mvarShiftNumber;
        private decimal mvarFloat;
        private decimal mvarOverPay;
        private decimal mvarDraw;
        private decimal mvarDrop;
        private decimal mvarPayments;
        private decimal mvarARPay;
        private decimal mvarPayouts;
        private decimal mvarCredits_Issued;
        private bool mvarAllShifts;
        private bool mvarComplete;
        

        
        public struct FieldAndTable
        {
            public string TableName;
            public string FieldName;
        }
        private FieldAndTable[] AutoIncrementTables;
        //  cash Bonus implementation
        private decimal mvarBonusFloat;
        private decimal mvarBonusDraw;
        private decimal mvarBonusDrop;
        private decimal mvarBonusGiveaway;
        // 
        //   to link Till Close Report to TotalizerHist
        private int mvarGroupNumber;
        private string mvarGroupType = string.Empty;
        //   end
        private int mvarDip_Number; //  
        private decimal mvarPenny_Adj; //  
        private string[] ReturnStringArray;
        private bool SaveToArray;
        private short nH;

        public Close_Line Add(Close_Line CL, string sKey)
        {
            Close_Line returnValue = default(Close_Line);

            //set the properties passed into the method
            if (sKey.Length == 0)
            {
                mCol.Add(CL, null, null, null);
            }
            else
            {
                mCol.Add(CL, sKey, null, null);
            }

            //return the object created
            returnValue = CL;

            return returnValue;
        }

        public Close_Line get_Item(object vntIndexKey)
        {
            Close_Line returnValue = default(Close_Line);
            returnValue = mCol[vntIndexKey] as Close_Line;
            return returnValue;
        }

        public int Count
        {
            get
            {
                return mCol.Count;
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
        

      
        public decimal Credits_Issued
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarCredits_Issued;
                return returnValue;
            }
            set
            {
                mvarCredits_Issued = value;
            }
        }


        public decimal Payouts
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPayouts;
                return returnValue;
            }
            set
            {
                mvarPayouts = value;
            }
        }

        public bool AllShifts
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarAllShifts;
                return returnValue;
            }
            set
            {
                mvarAllShifts = value;
            }
        }

        public decimal ARPay
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarARPay;
                return returnValue;
            }
            set
            {
                mvarARPay = value;
            }
        }

        public decimal Payments
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPayments;
                return returnValue;
            }
            set
            {
                mvarPayments = value;
            }
        }

        public decimal Drop
        {
            get
            {
                return mvarDrop;
            }
            set
            {
                mvarDrop = value;
            }
        }

        public decimal Draw
        {
            get
            {
                return mvarDraw;
            }
            set
            {
                mvarDraw = value;
            }
        }

        public decimal OverPay
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarOverPay;
                return returnValue;
            }
            set
            {
                mvarOverPay = value;
            }
        }

        public decimal Float
        {
            get
            {
                return mvarFloat;
            }
            set
            {
                mvarFloat = value;
            }
        }


        public short Till_Number
        {
            get
            {
                short returnValue = 0;
                returnValue = mvarTill_Number;
                return returnValue;
            }
            set
            {
                mvarTill_Number = value;
            }
        }


        public int ShiftNumber
        {
            get
            {
                return mvarShiftNumber;
            }
            set
            {
                mvarShiftNumber = value;
            }
        }


        public string User
        {
            get
            {
                return mvarUser;
            }
            set
            {
                mvarUser = value;
            }
        }


        public DateTime Open_Time
        {
            get
            {
                return mvarOpen_Time;
            }
            set
            {
                mvarOpen_Time = value;
            }
        }


        public DateTime ShiftDate
        {
            get
            {
                return mvarShiftDate;
            }
            set
            {
                mvarShiftDate = value;
            }
        }


        public DateTime Open_Date
        {
            get
            {
                return mvarOpen_Date;
            }
            set
            {
                mvarOpen_Date = value;
            }
        }


        public DateTime Close_Time
        {
            get
            {
                return mvarClose_Time;
            }
            set
            {
                mvarClose_Time = value;
            }
        }


        public DateTime Close_Date
        {
            get
            {
                return mvarClose_Date;
            }
            set
            {
                mvarClose_Date = value;
            }
        }


        public int Close_Num
        {
            get
            {
                return mvarClose_Num;
            }
            set
            {
                mvarClose_Num = value;
            }
        }
        


        public bool Complete
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarComplete;
                return returnValue;
            }
            set
            {
                mvarComplete = value;
            }
        }

        //Shiny - mar9,2009 - Cash bonus implementation

        public decimal BonusFloat
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusFloat;
                return returnValue;
            }
            set
            {
                mvarBonusFloat = value;
            }
        }
        public decimal BonusDraw
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusDraw;
                return returnValue;
            }
            set
            {
                mvarBonusDraw = value;
            }
        }
        public decimal BonusDrop
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusDrop;
                return returnValue;
            }
            set
            {
                mvarBonusDrop = value;
            }
        }
        public decimal BonusGiveAway
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarBonusGiveaway;
                return returnValue;
            }
            set
            {
                mvarBonusGiveaway = value;
            }
        }
        //shiny end

        //  

        public int GroupNumber
        {
            get
            {
                return mvarGroupNumber;
            }
            set
            {
                mvarGroupNumber = value;
            }
        }


        public string GroupType
        {
            get
            {
                return mvarGroupType;
            }
            set
            {
                mvarGroupType = value;
            }
        }
        //    end

        //   to save Dip_Number in Close_Head table (used in DSR)

        public int Dip_Number
        {
            get
            {
                return mvarDip_Number;
            }
            set
            {
                mvarDip_Number = value;
            }
        }
        //  


        public decimal Penny_Adj
        {
            get
            {
                decimal returnValue = 0;
                returnValue = mvarPenny_Adj;
                return returnValue;
            }
            set
            {
                mvarPenny_Adj = value;
            }
        }

        public List<TillClose> Tenders { get; set; }

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }

        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
            
            SaveToArray = false;
        }
        public Till_Close()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate()
        {
           // mCol = null;
        }

        public void Dispose()
        {
            Class_Terminate();
            //base.Finalize();
        }

        IEnumerator<Close_Line> IEnumerable<Close_Line>.GetEnumerator()
        {
            return new CloseLineEnuerator(mCol);
        }
    }

    public class CloseLineEnuerator : IEnumerator<Close_Line>
    {
        private Collection mCol;
        private int _index;
        private Close_Line _current;

        public CloseLineEnuerator(Collection col)
        {
            mCol = col;
        }

        public Close_Line Current
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
                _current = mCol[_index] as Close_Line;
            }
            else
            {
                _current = null;
            }
            return (_index <= mCol.Count);
        }

        public void Reset()
        {
            _index = 0;
            _current = null;
        }
    }
}
