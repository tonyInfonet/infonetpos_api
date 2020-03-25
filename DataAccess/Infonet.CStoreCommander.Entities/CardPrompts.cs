using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;

namespace Infonet.CStoreCommander.Entities
{
    public class CardPrompts : IEnumerable<CardPrompt> 
    {
        //local variable to hold collection
        private Collection mCol;

        public CardPrompt Add(short MaxLength, short MinLength, string PromptMessage, byte PromptSeq, short PromptID, string PromptAnswer, string sKey)
        {
            CardPrompt returnValue = default(CardPrompt);
            //create a new object
            CardPrompt objNewMember = default(CardPrompt);
            objNewMember = new CardPrompt();


            //set the properties passed into the method
            objNewMember.MaxLength = Convert.ToInt16((Information.IsDBNull(MaxLength)) ? 0 : MaxLength);
            objNewMember.MinLength = Convert.ToInt16((Information.IsDBNull(MinLength)) ? 0 : MinLength);
            objNewMember.PromptMessage = PromptMessage;
            objNewMember.PromptID = PromptID;
            objNewMember.PromptSeq = PromptSeq;
            
            objNewMember.PromptAnswer = Convert.ToString((Information.IsDBNull(PromptAnswer)) ? "" : PromptAnswer);
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

        public CardPrompt this[int index]
        {
            get
            {
                CardPrompt returnValue = default(CardPrompt);
                //used when referencing an element in the collection
                //vntIndexKey contains either the Index or Key to the collection,
                //this is why it is declared as a Variant
                //Syntax: Set foo = x.Item(xyz) or Set foo = x.Item(5)
                returnValue = mCol[index] as CardPrompt;
                return returnValue;
            }
        }

        public int Count
        {
            get
            {
                int returnValue = 0;
                //used when retrieving the number of elements in the
                //collection. Syntax: Debug.Print x.Count
                returnValue = mCol.Count;
                return returnValue;
            }
        }

        //Public ReadOnly Property NewEnum() As stdole.IUnknown
        //Get
        //this property allows you to enumerate
        //this collection with the For...Each syntax
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
            //used when removing an element from the collection
            //vntIndexKey contains either the Index or Key, which is why
            //it is declared as a Variant
            //Syntax: x.Remove(xyz)
            mCol.Remove(System.Convert.ToString(index));
        }

        
        private void Class_Initialize()
        {
            //creates the collection when this class is created
            mCol = new Collection();
        }
        public CardPrompts()
        {
            Class_Initialize();
          
        }

        
        private void Class_Terminate()
        {
            //destroys collection when this class is terminated
            
            mCol = null;
        }

        public void Dispose()
        {
            
        }

        ~CardPrompts()
        {
            Class_Terminate();
        }

        IEnumerator<CardPrompt> IEnumerable<CardPrompt>.GetEnumerator()
        {
            return new CardPromptEnum(mCol);
        }

        public static implicit operator CardPrompts(List<CardPrompt> prompts)
        {
            var cardPrompts = new CardPrompts();
            //  throw new NotImplementedException();
            foreach(var prompt in prompts)
            {
                cardPrompts.Add(prompt.MaxLength, prompt.MinLength, prompt.PromptMessage,prompt.PromptSeq,prompt.PromptID,prompt.PromptAnswer,"");
            }

            return cardPrompts;
        }
        //2013 11 08 - Reji  -End
    }

    public class CardPromptEnum : IEnumerator<CardPrompt>, IDisposable
    {

        private Collection mCol;
        private CardPrompt _current;
        private int _index;

        public CardPromptEnum(Collection col)
        {
            mCol = col;
        }

        public CardPrompt Current
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
                _current = mCol[_index] as CardPrompt;
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
