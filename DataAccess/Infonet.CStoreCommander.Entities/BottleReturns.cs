using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class BottleReturns : IEnumerable<BottleReturn>
    {
        private Collection mCol;
        private int _index;
        private BottleReturn _current;

        public BottleReturn this[int index]
        {
            get
            {
                BottleReturn returnValue = default(BottleReturn);
                returnValue = mCol[index] as BottleReturn;
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
            //_index = 0;
            //return this;
            return mCol.GetEnumerator();
        }

        public BottleReturn AddLine(short Line_Num, BottleReturn oLine, string sKey)
        {
            BottleReturn returnValue = default(BottleReturn);

            oLine.LineNumber = Line_Num;

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

        public void Remove(int index)
        {
            mCol.Remove(System.Convert.ToString(index));
        }

        private void Class_Initialize()
        {
            mCol = new Collection();
        }
        public BottleReturns()
        {
            Class_Initialize();
        }


        private void Class_Terminate()
        {
            mCol = null;
        }

        IEnumerator<BottleReturn> IEnumerable<BottleReturn>.GetEnumerator()
        {
            return new BottleReturnEnum(mCol);
        }

        public void Dispose()
        {

        }

        ~BottleReturns()
        {
            Class_Terminate();
        }
    }

    public class BottleReturnEnum : IEnumerator<BottleReturn>, IDisposable
    {

        private Collection mCol;
        private int _index;
        private BottleReturn _current;

        public BottleReturnEnum(Collection col)
        {
            mCol = col;
        }

        public BottleReturn Current
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
                _current = mCol[_index] as BottleReturn;
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

