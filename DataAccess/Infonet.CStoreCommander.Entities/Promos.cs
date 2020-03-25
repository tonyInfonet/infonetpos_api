using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class Promos : IEnumerable<Promo>
    {
        private Collection mCol;

        public Promo Add(Promo objPromo, string sKey)
        {
            Promo returnValue = default(Promo);

            if (sKey.Length == 0)
            {
                mCol.Add(objPromo, null, null, null);
            }
            else
            {
                mCol.Add(objPromo, sKey, null, null);
            }

            //return the object created
            returnValue = objPromo;

            return returnValue;
        }

        public Promo get_Item(object vntIndexKey)
        {
            Promo returnValue = default(Promo);
            returnValue = mCol[vntIndexKey] as Promo;
            return returnValue;
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
        public Promos()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        public void Dispose()
        {

            //base.Finalize();
        }

        ~Promos()
        {
            Class_Terminate_Renamed();
        }

        IEnumerator<Promo> IEnumerable<Promo>.GetEnumerator()
        {
            return new PromoEnum(mCol);
        }
    }


    public class PromoEnum : IEnumerator<Promo>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private Promo _current;

        public PromoEnum(Collection col)
        {
            mCol = col;
        }

        public Promo Current
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
                _current = mCol[_index] as Promo;
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
