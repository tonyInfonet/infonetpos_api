using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class TaxCreditLines : IEnumerable<TaxCreditLine> 
    {
        private Collection mCol;

        public TaxCreditLine AddLine(short Line_Num, TaxCreditLine oLine, string sKey)
        {
            TaxCreditLine returnValue = default(TaxCreditLine);

            oLine.Line_Num = Line_Num;

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

        public TaxCreditLine this[object vntIndexKey]
        {
            get
            {
                TaxCreditLine returnValue = default(TaxCreditLine);
                returnValue = mCol[vntIndexKey] as TaxCreditLine;
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

        public IEnumerator GetEnumerator()
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
        public TaxCreditLines()
        {
            Class_Initialize_Renamed();
        }


        
        private void Class_Terminate_Renamed()
        {
            
            mCol = null;
        }

        IEnumerator<TaxCreditLine> IEnumerable<TaxCreditLine>.GetEnumerator()
        {
            return new TaxCreditLineEnum(mCol);
        }

        public void Dispose()
        {
           
        }
        ~TaxCreditLines()
        {
            Class_Terminate_Renamed();
        }

       
    }

    public class TaxCreditLineEnum : IEnumerator<TaxCreditLine>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private TaxCreditLine _current;

        public TaxCreditLineEnum(Collection col)
        {
            mCol = col;
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
                _current = mCol[_index] as TaxCreditLine;
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

        public TaxCreditLine Current
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
