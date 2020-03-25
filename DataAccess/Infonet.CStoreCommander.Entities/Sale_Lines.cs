using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Sale_Lines : IEnumerable<Sale_Line>
    {
        private Collection mCol;


        private int mLastItemID; 
        private bool mvarCoupon;

        public Sale_Line AddLine(short Line_Num, Sale_Line oLine, string sKey)
        {
            Sale_Line returnValue = default(Sale_Line);

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

            
            mLastItemID = Line_Num;
            

            return returnValue;
        }

        public Sale_Line this[object vntIndexKey]
        {
            get
            {
                Sale_Line returnValue = default(Sale_Line);
                returnValue = mCol[vntIndexKey] as Sale_Line;
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
            //_index = 0;
            //return this;
            return this.mCol.GetEnumerator();
        }

        

        public int LastItemID
        {
            get
            {
                int returnValue = 0;
                returnValue = mLastItemID;
                return returnValue;
            }
            set
            {
                mLastItemID = value;
            }
        }
        

        //

        public bool blCoupon
        {
            get
            {
                bool returnValue = false;
                returnValue = mvarCoupon;
                return returnValue;
            }
            set
            {
                mvarCoupon = value;
            }
        }    

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(Convert.ToString(vntIndexKey));

            
            if (mCol.Count < Convert.ToInt32(vntIndexKey)) 
            {
                mLastItemID = mCol.Count;
            }
            else
            {
                mLastItemID = Convert.ToInt32(vntIndexKey);
            }
            
        }


        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
        }
        public Sale_Lines()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        IEnumerator<Sale_Line> IEnumerable<Sale_Line>.GetEnumerator()
        {
            return new SaleLineEnumerator(mCol);
        }

        ~Sale_Lines()
        {
            Class_Terminate_Renamed();
        }

        //End - SV
    }

    public class SaleLineEnumerator : IEnumerator<Sale_Line>, IDisposable
    {
        private Collection _mCol;
        private int _index;
        private Sale_Line _current;

        public SaleLineEnumerator(Collection mCol)
        {
            _mCol = mCol;

            _index = 0;
        }

        public Sale_Line Current
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
            //throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= _mCol.Count)
            {
                _current = _mCol[_index] as Sale_Line;
            }
            else
            {
                _current = null;
            }
            return (_index <= _mCol.Count);
        }

        public void Reset()
        {
            _current = null;
            _index = 0;
        }

        ~SaleLineEnumerator()
        {
            Class_Terminate_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            _mCol = null;
        }
    }
}
