using System;
using System.Collections.Generic;
using VBCollection = Microsoft.VisualBasic.Collection;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    public class GCTenders : IEnumerable<GCTender> 
    {

        private VBCollection mCol;

        public GCTender this[int index]
        {
            get
            {
                GCTender returnValue = default(GCTender);
                returnValue = mCol[index] as GCTender;
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

        public GCTender AddLine(GCTender oGCLine, string sKey)
        {
            GCTender returnValue = default(GCTender);

            if (sKey.Length == 0)
            {
                mCol.Add(oGCLine, null, null, null);
            }
            else
            {
                mCol.Add(oGCLine, sKey, null, null);
            }

            //return the object created
            returnValue = oGCLine;

            return returnValue;
        }

        public void Remove(int index)
        {
            mCol.Remove(System.Convert.ToString(index));
        }

        private void Class_Initialize()
        {
            mCol = new VBCollection();
        }
        public GCTenders()
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

        ~GCTenders()
        {
            Class_Terminate();
        }

       

        IEnumerator<GCTender> IEnumerable<GCTender>.GetEnumerator()
        {
            return new GCTenderEnum(mCol);
        }
    }

    public class GCTenderEnum : IEnumerator<GCTender>, IDisposable
    {

        private VBCollection mCol;
        private GCTender _current;
        private int _index;

        public GCTenderEnum(VBCollection col)
        {
            mCol = col;
        }
        public GCTender Current
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
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as GCTender;
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
