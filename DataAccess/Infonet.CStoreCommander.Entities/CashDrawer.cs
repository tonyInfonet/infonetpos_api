using System;
namespace Infonet.CStoreCommander.Entities
{
    public class CashDrawer : IDisposable
    {
        private Return_Reason mvarReturnReason;

        public Return_Reason Return_Reason
        {
            get
            {
                Return_Reason returnValue = default(Return_Reason);
                returnValue = mvarReturnReason;
                return returnValue;
            }
            set
            {
                mvarReturnReason = value;
            }
        }

        private void Class_Initialize()
        {
            mvarReturnReason = new Return_Reason();
        }

        public CashDrawer()
        {
            Class_Initialize();
        }

        private void Class_Terminate()
        {
            mvarReturnReason = null;
        }

        public void Dispose()
        {
            
        }
        ~CashDrawer()
        {
            Class_Terminate();
        }

    }
}
