using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    [Serializable]
    public class Return_Reasons : IEnumerable<Return_Reason>
    {
        private Collection mCol;

        public Return_Reason Add(string Reason, string RType, string sKey)
        {
            Return_Reason returnValue = default(Return_Reason);
            //create a new object
            Return_Reason RRN = default(Return_Reason);
            Return_Reason objNewMember = default(Return_Reason);
            objNewMember = new Return_Reason();

            //set the properties passed into the method
            objNewMember.Reason = Reason;
            objNewMember.RType = RType;

            // in a sale line can be only one reason for a type
            // remove the previous reason from collection
            foreach (Return_Reason tempLoopVar_RRN in mCol)
            {
                RRN = tempLoopVar_RRN;
                if (RRN.RType == sKey)
                {
                    mCol.Remove(sKey);
                }
            }

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
            RRN = null;

            return returnValue;
        }

        public Return_Reason this[object vntIndexKey]
        {
            get
            {
                Return_Reason returnValue = default(Return_Reason);
                returnValue = mCol[vntIndexKey] as Return_Reason;
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
        }
        public Return_Reasons()
        {
            Class_Initialize_Renamed();
        }


        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        IEnumerator<Return_Reason> IEnumerable<Return_Reason>.GetEnumerator()
        {
            return new ReturnReasonEnum(mCol);
        }

        public void Dispose()
        {

        }

        ~Return_Reasons()
        {
            Class_Terminate_Renamed();
        }


    }

    public class ReturnReasonEnum : IEnumerator<Return_Reason>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private Return_Reason _current;

        public ReturnReasonEnum(Collection col)
        {
            mCol = col;

        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as Return_Reason;
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

        public Return_Reason Current
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
