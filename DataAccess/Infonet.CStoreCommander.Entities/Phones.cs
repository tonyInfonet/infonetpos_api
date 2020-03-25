using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    public class Phones : IEnumerable<Phone>
    {
        private Collection mCol;

        public Phone Add(string PhoneName, string PhoneType, string AreaCode, string Number, string sKey)
        {
            Phone returnValue = default(Phone);

            //create a new object
            Phone objNewMember = default(Phone);
            objNewMember = new Phone();


            //set the properties passed into the method
            objNewMember.PhoneName = PhoneName;
            objNewMember.PhoneType = PhoneType;
            objNewMember.AreaCode = AreaCode;
            objNewMember.Number = Number;

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

        public Phone this[object vntIndexKey]
        {
            get
            {
                Phone returnValue = default(Phone);
                returnValue = mCol[vntIndexKey] as Phone;
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
        public Phones()
        {
            Class_Initialize_Renamed();
        }



        private void Class_Terminate_Renamed()
        {

            mCol = null;
        }

        IEnumerator<Phone> IEnumerable<Phone>.GetEnumerator()
        {
            return new PhoneEnum(mCol);
        }

        public void Dispose()
        {
            
        }

        ~Phones()
        {
            Class_Terminate_Renamed();
        }

        
    }

    public class PhoneEnum : IEnumerator<Phone>
    {
        private Collection mCol;
        private int _index;
        private Phone _current;

        public Phone Current
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

        public PhoneEnum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Phone;
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
    }
}
