using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using VBCollection = Microsoft.VisualBasic.Collection;
using System.Collections;

namespace Infonet.CStoreCommander.Entities
{
    public class CardTypes : IEnumerable<Card>
    {

        private Collection mCol;

        public Card Add(Card Card, string sKey)
        {
            Card returnValue = default(Card);

            Card objNewMember = default(Card);
            objNewMember = Card;

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

        public Card this[int index]
        {
            get
            {
                Card returnValue = default(Card);
                returnValue = mCol[index] as Card;
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


        public void Remove(int index)
        {
            mCol.Remove(System.Convert.ToString(index));
        }

      

        private void Class_Initialize()
        {
            mCol = new VBCollection();
        }
        public CardTypes()
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
        ~CardTypes()
        {
            Class_Terminate();
        }


        IEnumerator<Card> IEnumerable<Card>.GetEnumerator()
        {
            return new CardTypeEnum(mCol);
        }

    }

    public class CardTypeEnum : IEnumerator<Card>, IDisposable
    {

        private Collection mCol;
        private Card _current;
        private int _index;

        public CardTypeEnum(Collection col)
        {
            mCol = col;
        }

        public Card Current
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
                _current = mCol[_index] as Card;
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
