using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class SP_Prices : IEnumerable<SP_Price>
    {
        private Collection mCol;
       
        public SP_Price Add(float From_Quantity, float To_Quantity, float Price, object FromDate, object ToDate, string sKey)
        {
            SP_Price returnValue = default(SP_Price);

            //create a new object
            SP_Price objNewMember = default(SP_Price);
            objNewMember = new SP_Price();


            //set the properties passed into the method
            objNewMember.From_Quantity = From_Quantity;
            objNewMember.To_Quantity = To_Quantity;
            objNewMember.Price = Price;
            objNewMember.FromDate = FromDate;
            objNewMember.ToDate = ToDate;

            if (sKey.Length == 0)
            {
                mCol.Add(objNewMember, null, null, null);
            }
            else
            {
                mCol.Add(objNewMember, sKey, null, null);
            }


            //return the object created
            returnValue = objNewMember;
            objNewMember = null;


            return returnValue;
        }

        public SP_Price this[int vntIndexKey]
        {
            get
            {
                SP_Price returnValue = default(SP_Price);
                returnValue = mCol[vntIndexKey] as SP_Price;
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

        public SP_Prices()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        IEnumerator<SP_Price> IEnumerable<SP_Price>.GetEnumerator()
        {
            return new SPPriceENum(mCol);
        }

        public void Dispose()
        {
            
        }

        ~SP_Prices()
        {
            Class_Terminate_Renamed();
        }

        
    }

    public class SPPriceENum : IEnumerator<SP_Price>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private SP_Price _current;

        public SPPriceENum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as SP_Price;
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

        public SP_Price Current
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
