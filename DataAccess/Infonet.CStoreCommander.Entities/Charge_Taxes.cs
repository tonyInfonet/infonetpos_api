using System.Collections.Generic;
using System.Collections;
using System;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Charge_Taxes : IEnumerable<Charge_Tax>
    {
        private Collection mCol;
        private Charge_Tax _current;
        private int _index;

        public Charge_Tax Add(string Tax_Name, string Tax_Code, float Tax_Rate, bool Tax_Included, string sKey)
        {
            Charge_Tax returnValue = default(Charge_Tax);

            //create a new object
            Charge_Tax objNewMember = default(Charge_Tax);
            objNewMember = new Charge_Tax();

            //set the properties passed into the method
            objNewMember.Tax_Name = Tax_Name;
            objNewMember.Tax_Code = Tax_Code;
            objNewMember.Tax_Rate = Tax_Rate;
            objNewMember.Tax_Included = Tax_Included;

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

        public Charge_Tax this[int index]
        {
            get
            {
                Charge_Tax returnValue = default(Charge_Tax);
                returnValue = mCol[index] as Charge_Tax;
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
            mCol = new Collection();
        }
        public Charge_Taxes()
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

        ~Charge_Taxes()
        {
            Class_Terminate();
        }



        IEnumerator<Charge_Tax> IEnumerable<Charge_Tax>.GetEnumerator()
        {
            return new ChargeTaxEnum(mCol);
        }
    }

    public class ChargeTaxEnum : IEnumerator<Charge_Tax>, IDisposable
    {
        private Collection mCol;
        private Charge_Tax _current;
        private int _index;

        public ChargeTaxEnum(Collection col)
        {
            mCol = col;
        }

        public Charge_Tax Current
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
                _current = mCol[_index] as Charge_Tax;
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
