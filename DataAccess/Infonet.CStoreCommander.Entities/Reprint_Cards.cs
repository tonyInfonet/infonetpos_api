using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class Reprint_Cards : IEnumerable<Card_Reprint> 
    {

        //local variable to hold collection
        private Collection mCol;

        public Card_Reprint Add(Card_Reprint Card_Reprint, string sKey)
        {
            Card_Reprint returnValue = default(Card_Reprint);
            //create a new object
            Card_Reprint objNewMember = default(Card_Reprint);
            objNewMember = Card_Reprint;

            //set the properties passed into the method
            if (sKey.Length == 0)
            {
                mCol.Add(objNewMember, null, null, null);
            }
            else
            {
                mCol.Add(objNewMember, sKey, null, null);
            }

            returnValue = objNewMember;
            objNewMember = null;


            return returnValue;
        }

        public Card_Reprint this[object vntIndexKey]
        {
            get
            {
                Card_Reprint returnValue = default(Card_Reprint);
                returnValue = mCol[vntIndexKey] as Card_Reprint;
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
        public Reprint_Cards()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        IEnumerator<Card_Reprint> IEnumerable<Card_Reprint>.GetEnumerator()
        {
            return new ReprintCardEnum(mCol);
        }

        public void Dispose()
        {
           
        }

        ~Reprint_Cards()
        {
            Class_Terminate_Renamed();
        }

       
    }

    public class ReprintCardEnum : IEnumerator<Card_Reprint>,IDisposable
    {

        private Collection mCol;
        private int _index;
        private Card_Reprint _current;

        public ReprintCardEnum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Card_Reprint;
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

        public Card_Reprint Current
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
