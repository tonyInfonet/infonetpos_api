using System;
using System.Collections.Generic;
using VBCollection = Microsoft.VisualBasic.Collection;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class K_Charges : IEnumerable<K_Charge> 
    {
        private VBCollection mCol;

        public K_Charge Add(double Charge_Price, string Charge_Desc, string Charge_Code, Charge_Taxes Charge_Taxes, string sKey)
        {
            K_Charge returnValue = default(K_Charge);

            //create a new object
            K_Charge objNewMember = default(K_Charge);
            objNewMember = new K_Charge();


            //set the properties passed into the method
            objNewMember.Charge_Code = Charge_Code;
            objNewMember.Charge_Price = Charge_Price;
            objNewMember.Charge_Desc = Charge_Desc;


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

        public K_Charge this[int index]
        {
            get
            {
                K_Charge returnValue = default(K_Charge);
                returnValue = mCol[index] as K_Charge;
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


        public void Remove(int index)
        {
            mCol.Remove(System.Convert.ToString(index));
        }


        private void Class_Initialize()
        {
            mCol = new VBCollection();
        }
        public K_Charges()
        {
            Class_Initialize();
        }


        private void Class_Terminate()
        {
            mCol = null;
        }

        public void Dispose()
        {
           
        }

        ~K_Charges()
        {
            Class_Terminate();
        }

       

        IEnumerator<K_Charge> IEnumerable<K_Charge>.GetEnumerator()
        {
            return new KChargeEnum(mCol);
        }
    }
    
    public class KChargeEnum : IEnumerator<K_Charge>, IDisposable
    {
        private VBCollection mCol;
        private K_Charge _current;
        private int _index;

        public KChargeEnum(VBCollection col)
        {
            mCol = col;
        }
        public K_Charge Current
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
            mCol = null;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as K_Charge;
            }
            else
            {
                _current = null;
            }
            return (_index <= mCol.Count);

        }

        public void Reset()
        {
            _index = 0;
            _current = null;
        }
    }
}
