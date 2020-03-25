using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class Promo_Lines : IEnumerable<Promo_Line>
    {
        private Collection mCol;

        public Promo_Line AddLine(Promo_Line objPromo_Line, string sKey)
        {
            Promo_Line returnValue = default(Promo_Line);

            if (sKey.Length == 0)
            {
                mCol.Add(objPromo_Line, null, null, null);
            }
            else
            {
                mCol.Add(objPromo_Line, sKey, null, null);
            }

            //return the object created
            returnValue = objPromo_Line;

            return returnValue;
        }

        public Promo_Line get_Item(object vntIndexKey)
        {
            Promo_Line returnValue = default(Promo_Line);
            returnValue = mCol[vntIndexKey] as Promo_Line;
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
        public Promo_Lines()
        {
            Class_Initialize_Renamed();
        }

        
        private void Class_Terminate_Renamed()
        {
            
            mCol = null;
        }

        IEnumerator<Promo_Line> IEnumerable<Promo_Line>.GetEnumerator()
        {
            return new PromoLineEnum(mCol);
        }

        public void Dispose()
        {
            
        }

        ~Promo_Lines()
        {
            Class_Terminate_Renamed();
        }


    }

    public class PromoLineEnum : IEnumerator<Promo_Line>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private Promo_Line _current;

        public PromoLineEnum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Promo_Line;
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

        public Promo_Line Current
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
