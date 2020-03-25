using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class Payout_Taxes : IEnumerable<Payout_Tax> 
    {

        private Collection mCol;

        public Payout_Tax Add(string Tax_Name, string Tax_Description, decimal Tax_Amount, bool Tax_Active, string sKey)
        {
            Payout_Tax returnValue = default(Payout_Tax);

            Payout_Tax objNewMember = default(Payout_Tax);
            objNewMember = new Payout_Tax();

            objNewMember.Tax_Name = Tax_Name;
            objNewMember.Tax_Description = Tax_Description;
            objNewMember.Tax_Amount = Tax_Amount;
            objNewMember.Tax_Active = Tax_Active;

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

        public Payout_Tax this[object vntIndexKey]
        {
            get
            {
                Payout_Tax returnValue = default(Payout_Tax);
                returnValue = mCol[vntIndexKey] as Payout_Tax;
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


        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }



        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
            //TODO: Call where class is initialised
            //Load_Taxes();
        }

        public Payout_Taxes()
        {
            Class_Initialize_Renamed();
        }



        private void Class_Terminate_Renamed()
        {

            mCol = null;
        }

        public void Dispose()
        {
            
        }

       
        IEnumerator<Payout_Tax> IEnumerable<Payout_Tax>.GetEnumerator()
        {
            return new PayoutTaxEnum(mCol);
        }
    }

    public class PayoutTaxEnum : IEnumerator<Payout_Tax>, IDisposable
    {

        private Collection mCol;
        private int _index;
        private Payout_Tax _current;

        public PayoutTaxEnum(Collection col)
        {
            mCol = col;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Payout_Tax;
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

        public Payout_Tax Current
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
