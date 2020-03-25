using System.Collections.Generic;
using VBCollection = Microsoft.VisualBasic.Collection;
using System.Collections;
using System;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Charges : IEnumerable<Charge> 
    {
        private VBCollection _mCol;


        public Charge Add(string chargeCode, string chargeDesc, float chargePrice, Charge_Taxes chargeTaxes, string sKey)
        {
            Charge returnValue = default(Charge);

            //create a new object
            Charge objNewMember = default(Charge);
            objNewMember = new Charge();


            //set the properties passed into the method
            objNewMember.Charge_Code = chargeCode;
            objNewMember.Charge_Desc = chargeDesc;
            objNewMember.Charge_Price = chargePrice;
            objNewMember.Charge_Taxes = chargeTaxes;

            if (sKey.Length == 0)
            {
                _mCol.Add(objNewMember, null, null, null);
            }
            else
            {
                _mCol.Add(objNewMember, sKey, null, null);
            }

            //return the object created
            returnValue = objNewMember;
            objNewMember = null;


            return returnValue;
        }

        public Charge this[int index]
        {
            get
            {
                Charge returnValue = default(Charge);
                returnValue = _mCol[index] as Charge;
                return returnValue;
            }
        }


        public int Count
        {
            get
            {
                int returnValue = 0;
                returnValue = _mCol.Count;
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
            return _mCol.GetEnumerator();
            //_index = 0;
            //return this;
        }


        public void Remove(int index)
        {
            _mCol.Remove(System.Convert.ToString(index));
        }


        private void Class_Initialize()
        {
            _mCol = new VBCollection();
        }
        public Charges()
        {
            Class_Initialize();
        }


        private void Class_Terminate()
        {
            _mCol = null;
        }

        public void Dispose()
        {
            
        }

        ~Charges()
        {
            Class_Terminate();
        }

       
        IEnumerator<Charge> IEnumerable<Charge>.GetEnumerator()
        {
            return new ChargeEnum(_mCol);
        }

    }

    public class ChargeEnum : IEnumerator<Charge>, IDisposable
    {
        private VBCollection mCol;
        private Charge _current;
        private int _index;

        public ChargeEnum(VBCollection col)
        {
            mCol = col;
        }

        public Charge Current
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
                _current = mCol[_index] as Charge;
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
