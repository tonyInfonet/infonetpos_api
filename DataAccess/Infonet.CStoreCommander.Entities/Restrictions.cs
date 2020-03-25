using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Restrictions : IEnumerable<Restriction> 
    {
        private Collection mCol;

        public Restriction Add(short Code, string Description, bool Confirmed, string sKey)
        {
            Restriction returnValue = default(Restriction);

            //create a new object
            Restriction objNewMember = default(Restriction);
            objNewMember = new Restriction();

            //set the properties passed into the method
            objNewMember.Code = Code;
            objNewMember.Description = Description;
            objNewMember.Confirmed = Confirmed;

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

        public Restriction this[object vntIndexKey]
        {
            get
            {
                Restriction returnValue = default(Restriction);
                returnValue = mCol[vntIndexKey] as Restriction;
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
        public Restrictions()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        IEnumerator<Restriction> IEnumerable<Restriction>.GetEnumerator()
        {
            return new RestrictionEnum(mCol);
        }

        public void Dispose()
        {
           
        }

        ~Restrictions()
        {
            Class_Terminate_Renamed();
        }

      
    }

    public class RestrictionEnum : IEnumerator<Restriction>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private Restriction _current;

        public RestrictionEnum(Collection col)
        {
            mCol = col;
        }

        public Restriction Current
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

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Restriction;
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
