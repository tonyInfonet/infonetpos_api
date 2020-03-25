using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class TaxExemptSaleLines : IEnumerable<TaxExemptSaleLine>
    {
        private Collection mCol;

        public TaxExemptSaleLine AddLine(short IdNum, TaxExemptSaleLine oLine, string sKey)
        {
            TaxExemptSaleLine returnValue = default(TaxExemptSaleLine);

            oLine.ItemID = IdNum;

            if (sKey.Length == 0)
            {
                mCol.Add(oLine, null, null, null);
            }
            else
            {
                mCol.Add(oLine, sKey, null, null);
            }

            //return the object created
            returnValue = oLine;

            return returnValue;
        }

        public TaxExemptSaleLine this[object vntIndexKey]
        {
            get
            {
                TaxExemptSaleLine returnValue = default(TaxExemptSaleLine);
                returnValue = mCol[vntIndexKey] as TaxExemptSaleLine;
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

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }

        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
        }
        public TaxExemptSaleLines()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        public void Dispose()
        {

        }

        ~TaxExemptSaleLines()
        {
            Class_Terminate_Renamed();
        }



        IEnumerator<TaxExemptSaleLine> IEnumerable<TaxExemptSaleLine>.GetEnumerator()
        {
            return new TaxExemptSalelineEnum(mCol);
        }
    }

    public class TaxExemptSalelineEnum : IEnumerator<TaxExemptSaleLine>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private TaxExemptSaleLine _current;

        public TaxExemptSalelineEnum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as TaxExemptSaleLine;
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

        public TaxExemptSaleLine Current
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
