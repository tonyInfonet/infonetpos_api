using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    public class TaxExemptReasons : IEnumerable<TaxExemptReason> 
    {

        private Collection mCol;

        public TaxExemptReason this[object vntIndexKey]
        {
            get
            {
                TaxExemptReason returnValue = default(TaxExemptReason);
                returnValue = mCol[vntIndexKey] as TaxExemptReason;
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

        public TaxExemptReason AddReason(TaxExemptReason oReason, string sKey)
        {
            TaxExemptReason returnValue = default(TaxExemptReason);

            if (sKey.Length == 0)
            {
                mCol.Add(oReason, null, null, null);
            }
            else
            {
                mCol.Add(oReason, sKey, null, null);
            }

            //return the object created
            returnValue = oReason;

            return returnValue;
        }

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }

        
        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
        }
        public TaxExemptReasons()
        {
            Class_Initialize_Renamed();
        }

        
        private void Class_Terminate_Renamed()
        {
            
            mCol = null;
        }

        IEnumerator<TaxExemptReason> IEnumerable<TaxExemptReason>.GetEnumerator()
        {
            return new TaxExemptReasonEnum(mCol);
        }

        public void Dispose()
        {
           
        }
        ~TaxExemptReasons()
        {
            Class_Terminate_Renamed();
        }

       
    }

    public class TaxExemptReasonEnum : IEnumerator<TaxExemptReason>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private TaxExemptReason _current;

        public TaxExemptReasonEnum(Collection col)
        {
            mCol = col;
        }
        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as TaxExemptReason;
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

        public TaxExemptReason Current
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
