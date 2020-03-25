using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Line_Taxes : IEnumerable<Line_Tax>
    {
        private Collection mCol;


        public Line_Tax Add(string Tax_Name, string Tax_Code, float Tax_Rate, bool Tax_Included, float Tax_Rebate_Rate, decimal Tax_Rebate, string sKey)
        {
            Line_Tax returnValue = default(Line_Tax);

            //create a new object
            Line_Tax objNewMember = default(Line_Tax);
            objNewMember = new Line_Tax();

            //set the properties passed into the method

            objNewMember.Tax_Name = System.Convert.ToString((Information.IsDBNull(Tax_Name)) ? "" : Tax_Name);

            objNewMember.Tax_Code = System.Convert.ToString((Information.IsDBNull(Tax_Code)) ? "" : Tax_Code);
            objNewMember.Tax_Rate = Tax_Rate;
            objNewMember.Tax_Included = Tax_Included;
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

        
        public Line_Tax AddTaxLine(Line_Tax oLine, string sKey)
        {
            Line_Tax returnValue = default(Line_Tax);

            if (sKey.Length == 0)
            {
                mCol.Add(oLine, null, null, null);
            }
            else
            {
                mCol.Add(oLine, sKey, null, null);
            }

            //return the object created
            returnValue = oLine;

            return returnValue;
        }
        

        public Line_Tax this[int index]
        {
            get
            {
                Line_Tax returnValue = default(Line_Tax);
                returnValue = mCol[index] as Line_Tax;
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
        public Line_Taxes()
        {
            Class_Initialize();
        }



        private void Class_Terminate()
        {

            mCol = null;
        }

        IEnumerator<Line_Tax> IEnumerable<Line_Tax>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
           
        }

        ~Line_Taxes()
        {
            Class_Terminate();
        }

       

    }

    public class LineTaxEnumerator : IEnumerator<Line_Tax>, IDisposable
    {
        private Collection _mCol;
        private Line_Tax _current;
        private int _index;

        public LineTaxEnumerator(Collection mCol)
        {
            _mCol = mCol;
        }

        public Line_Tax Current
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
                _current = _mCol[_index] as Line_Tax;
            }
            else
            {
                _current = null;
            }
            return (_index <= _mCol.Count);
        }

        public void Reset()
        {
            _index = 0;
            _current = null;
        }
    }
}
