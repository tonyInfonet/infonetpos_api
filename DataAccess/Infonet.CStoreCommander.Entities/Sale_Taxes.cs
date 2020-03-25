using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Sale_Taxes : IEnumerable<Sale_Tax> 
    {
        private Collection mCol;

        public Sale_Tax Add(string Tax_Name, string Tax_Code, float Tax_Rate, decimal Taxable_Amount, decimal Tax_Added_Amount, decimal Tax_Included_Amount, decimal Tax_Included_Total, float Tax_Rebate_Rate, decimal Tax_Rebate, string sKey)
        {
            Sale_Tax returnValue = default(Sale_Tax);

            //create a new object
            Sale_Tax objNewMember = default(Sale_Tax);
            objNewMember = new Sale_Tax();

            //set the properties passed into the method
            objNewMember.Tax_Name = Tax_Name;
            objNewMember.Tax_Code = Tax_Code;
            objNewMember.Taxable_Amount = Taxable_Amount;
            objNewMember.Tax_Rate = Tax_Rate;
            objNewMember.Tax_Added_Amount = Tax_Added_Amount;
            objNewMember.Tax_Included_Amount = Tax_Included_Amount;
            objNewMember.Tax_Included_Total = Tax_Included_Total;
            objNewMember.Tax_Rebate_Rate = Tax_Rebate_Rate;
            objNewMember.Tax_Rebate = Tax_Rebate;

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

        public Sale_Tax this[object vntIndexKey]
        {
            get
            {
                Sale_Tax returnValue = default(Sale_Tax);
                returnValue = mCol[vntIndexKey] as Sale_Tax;
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
            //GetEnumerator = mCol.GetEnumerator
            //_index = 0;
            //return this;
            return this.mCol.GetEnumerator();
        }


        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }


        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
        }
        public Sale_Taxes()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        IEnumerator<Sale_Tax> IEnumerable<Sale_Tax>.GetEnumerator()
        {
            return new SaleTaxEnumerator(mCol);
        }

        public void Dispose()
        {
            
        }
        ~Sale_Taxes()
        {
            Class_Terminate_Renamed();
        }

    }

    public class SaleTaxEnumerator : IEnumerator<Sale_Tax>, IDisposable
    {
        private Collection _mCol;
        private int _index;
        private Sale_Tax _current;

        public SaleTaxEnumerator(Collection mCol)
        {
            _mCol = mCol;
            _index = 0;
        }

        public Sale_Tax Current
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
           // throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= _mCol.Count)
            {
                _current = _mCol[_index] as Sale_Tax;
            }
            else
            {
                _current = null;
            }
            return (_index <= _mCol.Count);
        }

        public void Reset()
        {
            _current = null;
            _index = 0;
        }
    }

}
