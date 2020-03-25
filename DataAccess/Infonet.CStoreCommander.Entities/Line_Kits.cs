using System;
using System.Collections.Generic;
using VBCollection = Microsoft.VisualBasic.Collection;
using System.Collections;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Line_Kits : IEnumerable<Line_Kit> 
    {
        private VBCollection mCol;

        public Line_Kit Add(string Kit_Item, string Kit_Item_Desc, float Kit_Item_Qty, float Kit_Item_Base, float Kit_Item_Fraction, float Kit_Item_Allocate, string Kit_Item_Serial, K_Charges K_Charges, string sKey)
        {
            Line_Kit returnValue = default(Line_Kit);

            //create a new object
            Line_Kit objNewMember = default(Line_Kit);
            objNewMember = new Line_Kit();


            //set the properties passed into the method
            objNewMember.K_Charges = K_Charges;

            objNewMember.Kit_Item = Kit_Item;
            objNewMember.Kit_Item_Desc = System.Convert.ToString((Information.IsDBNull(Kit_Item_Desc)) ? "" : Kit_Item_Desc);
            objNewMember.Kit_Item_Base = Kit_Item_Base;
            objNewMember.Kit_Item_Fraction = Kit_Item_Fraction;
            objNewMember.Kit_Item_Allocate = Kit_Item_Allocate;
            objNewMember.Kit_Item_Serial = System.Convert.ToString((Information.IsDBNull(Kit_Item_Serial)) ? "" : Kit_Item_Serial);
            objNewMember.Kit_Item_Qty = Kit_Item_Qty;

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

        public Line_Kit this[int index]
        {
            get
            {
                Line_Kit returnValue = default(Line_Kit);
                returnValue = mCol[index] as Line_Kit;
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
        public Line_Kits()
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

        ~Line_Kits()
        {
            Class_Terminate();
        }

        IEnumerator<Line_Kit> IEnumerable<Line_Kit>.GetEnumerator()
        {
            return new LineKitEnum(mCol);
        }

    }

    public class LineKitEnum : IEnumerator<Line_Kit>, IDisposable
    {
        private VBCollection mCol;
        private Line_Kit _current;
        private int _index;

        public LineKitEnum(VBCollection col)
        {
            mCol = col;
        }

        public Line_Kit Current
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
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Line_Kit;
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
