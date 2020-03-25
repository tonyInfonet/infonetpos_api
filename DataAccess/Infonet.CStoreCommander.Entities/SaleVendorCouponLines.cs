using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class SaleVendorCouponLines : IEnumerable<SaleVendorCouponLine> 
    {

        private Collection mCol;

        public SaleVendorCouponLine this[object vntIndexKey]
        {
            get
            {
                SaleVendorCouponLine returnValue = default(SaleVendorCouponLine);
                returnValue = mCol[vntIndexKey] as SaleVendorCouponLine;
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

            return mCol.GetEnumerator();
            //_index = 0;
            //return this;
        }

        public SaleVendorCouponLine AddLine(short ItemNum, SaleVendorCouponLine oLine, string sKey)
        {
            SaleVendorCouponLine returnValue = default(SaleVendorCouponLine);
            oLine.ItemNum = ItemNum;
            if (sKey.Length == 0)
            {
               mCol.Add(oLine, oLine.SeqNum.ToString(), null, null);
            }
            else
            {
                mCol.Add(oLine, sKey, null, null);
            }

            //return the object created
            returnValue = oLine;

            return returnValue;
        }

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(Convert.ToString(vntIndexKey));
        }

        
        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
        }
        public SaleVendorCouponLines()
        {
            Class_Initialize_Renamed();
        }

        
        private void Class_Terminate_Renamed()
        {
            
            mCol = null;
        }

        IEnumerator<SaleVendorCouponLine> IEnumerable<SaleVendorCouponLine>.GetEnumerator()
        {
            return new SaleVendorCouponLineEnum(mCol);
        }

        public void Dispose()
        {
           
        }
        ~SaleVendorCouponLines()
        {
            Class_Terminate_Renamed();
        }

       
    }

    public class SaleVendorCouponLineEnum : IEnumerator<SaleVendorCouponLine>, IDisposable
    {

        private Collection mCol;
        private int _index;
        private SaleVendorCouponLine _current;

        public SaleVendorCouponLineEnum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as SaleVendorCouponLine;
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

        public void Dispose()
        {
            mCol = null;
        }

        public SaleVendorCouponLine Current
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

    }
}
