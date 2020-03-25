using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    public class CardCodes : IEnumerable<CardCode> 
    {
        private Collection mCol;

        public CardCode Add(string LowerLimit, string UpperLimit, string sKey)
        {
            CardCode returnValue = default(CardCode);

            CardCode objNewMember = default(CardCode);
            objNewMember = new CardCode();


            objNewMember.LowerLimit = LowerLimit;
            objNewMember.UpperLimit = UpperLimit;

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

        public CardCode this[int index]
        {
            get
            {
                CardCode returnValue = default(CardCode);
                returnValue = mCol[index] as CardCode;
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
        }


        public void Remove(int index)
        {
            mCol.Remove(System.Convert.ToString(index));
        }


        
        private void Class_Initialize()
        {
            mCol = new Collection();
        }
        public CardCodes()
        {
            Class_Initialize();
        }


        
        private void Class_Terminate()
        {
            
            mCol = null;
        }

        IEnumerator<CardCode> IEnumerable<CardCode>.GetEnumerator()
        {
            return new CardCodeEnum(mCol);
        }

        public void Dispose()
        {
          
        }

        ~CardCodes()
        {
            Class_Terminate();
        }

    }

    public class CardCodeEnum : IEnumerator<CardCode>, IDisposable
    {
        private Collection mCol;
        private CardCode _current;
        private int _index;

        public CardCodeEnum(Collection col)
        {
            mCol = col;
        }

        public CardCode Current
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
                _current = mCol[_index] as CardCode;
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
