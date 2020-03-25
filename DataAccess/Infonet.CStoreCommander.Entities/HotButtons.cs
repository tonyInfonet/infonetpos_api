using System;
using System.Collections.Generic;
using VBCollection = Microsoft.VisualBasic.Collection;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    public class HotButtons : IEnumerable<HotButton> 
    {

        private VBCollection mCol;

        public HotButton Add(short Button_Number, short Page_Number, short Button_Count, string Button_Product, string sKey, float Bottle_Price, bool By_Weight, float Quantity) 
        {
            HotButton returnValue = default(HotButton);
            //DEc8,2008 Shiny added quantity and By_weight for sclae integration

            //create a new object
            HotButton objNewMember = default(HotButton);
            objNewMember = new HotButton();


            //set the properties passed into the method
            objNewMember.Button_Number = Button_Number;
            objNewMember.Page_Number = Page_Number;
            objNewMember.Button_Count = Button_Count;
            objNewMember.Button_Product = Button_Product;
            objNewMember.BottlePrice = Bottle_Price; 
            objNewMember.Quantity = Quantity; //Shiny dec8, 2008
            objNewMember.By_Weight = By_Weight; //Shiny Dec8, 2008
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

        public HotButton this[int index]
        {
            get
            {
                HotButton returnValue = default(HotButton);
                returnValue = mCol[index] as HotButton;
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
        public HotButtons()
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
        ~HotButtons()
        {
            Class_Terminate();
        }

      
        IEnumerator<HotButton> IEnumerable<HotButton>.GetEnumerator()
        {
            return new HotButtonEnum(mCol);
        }

    }

    public class HotButtonEnum : IEnumerator<HotButton>, IDisposable
    {
        private VBCollection mCol;
        private HotButton _current;
        private int _index;

        public HotButtonEnum(VBCollection col)
        {
            mCol = col;
        }

        public HotButton Current
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
                _current = mCol[_index] as HotButton;
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
